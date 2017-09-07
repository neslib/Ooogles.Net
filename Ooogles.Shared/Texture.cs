using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.ES20;

using TKPixelFormat = OpenTK.Graphics.ES20.PixelFormat;

#if __ANDROID__ || __IOS__
using GetTextureParameterName = OpenTK.Graphics.ES20.GetTextureParameter;
#endif

namespace Ooogles
{
    /// <summary>
    /// A texture
    /// </summary>
    public sealed class Texture : GLObject
    {
        /// <summary>
        /// Supported <see cref="Texture"/> types
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A 2D texture
            /// </summary>
            TwoD = (int)TextureTarget.Texture2D,

            /// <summary>
            /// A cube map texture
            /// </summary>
            CubeMap = (int)TextureTarget.TextureCubeMap
        }

        /// <summary>
        /// Texture minification filters
        /// </summary>
        public enum MinFilter
        {
            /// <summary>
            /// Returns the value of the texture element that is nearest (in Manhattan distance) to the center of the pixel being textured.
            /// This is usually the fastest method.
            /// </summary>
            Nearest = (int)TextureMinFilter.Nearest,

            /// <summary>
            /// Returns the weighted average of the four texture elements that are closest to the center of the pixel being textured.
            /// This usually provides better quality than Nearest.
            /// </summary>
            Linear = (int)TextureMinFilter.Linear,

            /// <summary>
            /// Chooses the mipmap that most closely matches the size of the pixel being textured and uses the Nearest criterion (the texture element nearest to the center of the pixel) to produce a texture value.
            /// This is usually the fastest method when using mipmapping.
            /// </summary>
            NearestMipmapNearest = (int)TextureMinFilter.NearestMipmapNearest,

            /// <summary>
            /// Chooses the mipmap that most closely matches the size of the pixel being textured and uses the Linear criterion (a weighted average of the four texture elements that are closest to the center of the pixel) to produce a texture value.
            /// </summary>
            LinearMipmapNearest = (int)TextureMinFilter.LinearMipmapNearest,

            /// <summary>
            /// Chooses the two mipmaps that most closely match the size of the pixel being textured and uses the Nearest criterion (the texture element nearest to the center of the pixel) to produce a texture value from each mipmap.
            /// The final texture value is a weighted average of those two values.
            /// </summary>
            NearestMipmapLinear = (int)TextureMinFilter.NearestMipmapLinear,

            /// <summary>
            /// Chooses the two mipmaps that most closely match the size of the pixel being textured and uses the Linear criterion (a weighted average of the four texture elements that are closest to the center of the pixel) to produce a texture value from each mipmap.
            /// The final texture value is a weighted average of those two values.
            /// This is usually the slowest (but highest quality) method when using mipmapping.
            /// </summary>
            LinearMipmapLinear = (int)TextureMinFilter.LinearMipmapLinear
        }

        /// <summary>
        /// Texture magnification filters
        /// </summary>
        public enum MagFilter
        {
            /// <summary>
            /// Returns the value of the texture element that is nearest (in Manhattan distance) to the center of the pixel being textured.
            /// This is usually the fastest method.
            /// </summary>
            Nearest = (int)TextureMagFilter.Nearest,

            /// <summary>
            /// Returns the weighted average of the four texture elements that are closest to the center of the pixel being textured.
            /// This usually provides better quality.
            /// </summary>
            Linear = (int)TextureMagFilter.Linear
        }

        /// <summary>
        /// Texture wrapping modes
        /// </summary>
        public enum WrapMode
        {
            /// <summary>
            /// Repeats the texture.
            /// Causes the integer part of the texture coordinate to be ignored; 
            /// the GL uses only the fractional part, thereby creating a repeating pattern.
            /// </summary>
            Repeat = (int)TextureWrapMode.Repeat,

            /// <summary>
            /// Repeats and mirrors the texture.
            /// Causes the final texture coordinate (Dst) to be set to the fractional part of the original texture coordinate (Src) 
            /// if the integer part of Src is even; if the integer part of Src is odd, then Dst is set to 1 - Frac(Src), 
            /// where Frac(Src) represents the fractional part of Src.
            /// </summary>
            MirroredRepeat = 0x8370,

            /// <summary>
            /// Clamps the texture to its edges.
            /// Causes the texture coordinate to be clamped to the size of the texture in the direction of clamping.
            /// </summary>
            ClampToEdge = (int)TextureWrapMode.ClampToEdge
        }

        /// <summary>
        /// Mipmap hinting options, as used by <see cref="Texture.MipmapHint"/>
        /// </summary>
        public enum Hint
        {
            /// <summary>
            /// The most efficient option should be chosen.
            /// </summary>
            Fastest = (int)HintMode.Fastest,

            /// <summary>
            /// The most correct, or highest quality, option should be chosen.
            /// </summary>
            Nicest = (int)HintMode.Nicest,

            /// <summary>
            /// No preference.
            /// </summary>
            DontCare = (int)HintMode.DontCare
        }

        private TextureTarget _type;

        /// <summary>
        /// Checks if this texture is currently bound.
        /// </summary>
        /// <value>True if this is the currently bound texture, False otherwise.</value>
        public bool IsBound
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(glUtils.GetTargetBinding((All)_type), out int currentlyBoundTexture);
                glUtils.Check(this);
                return (currentlyBoundTexture == Handle);
            }
        }

        /// <summary>
        /// Gets or sets the minification filter for this texture.
        /// </summary>
        /// <remarks>
        /// The texture minifying function is used whenever the pixel being textured maps to an area greater than one texture element.
        /// There are six defined minifying functions.
        /// Two of them use the nearest one or nearest four texture elements to compute the texture value.
        /// The other four use mipmaps.
        ///
        /// <para>A mipmap is an ordered set of arrays representing the same image at progressively lower resolutions.
        /// If the texture has dimensions W × H, there are Floor(Log2(Max(W, H)) + 1) mipmap levels.
        /// The first mipmap level is the original texture, with dimensions W × H.
        /// Each subsequent mipmap level has half the dimensions of the previous level, until the final mipmap is reached, 
        /// which has dimension 1 × 1.</para>
        ///
        /// <para>To define the mipmap levels, call <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>, 
        /// <see cref="UploadCompressed"/> or <see cref="Copy"/> with 
        /// the <c>level</c> argument indicating the order of the mipmaps.
        /// Level 0 is the original texture; level Floor(Log2(Max(W, H))) is the final 1 × 1 mipmap.</para>
        ///
        /// <para>As more texture elements are sampled in the minification process, fewer aliasing artifacts will be apparent.
        /// While the Nearest and Linear minification functions can be faster than the other four, 
        /// they sample only one or four texture elements to determine the texture value of the pixel being rendered 
        /// and can produce moire patterns or ragged transitions.
        /// The initial value of the minification filter is NearestMipmapLinear.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexParameteri(GL_TEXTURE_MIN_FILTER), glGetTexParameteriv(GL_TEXTURE_MIN_FILTER)</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MagnificationFilter"/>
        public MinFilter MinificationFilter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetTexParameter(_type, GetTextureParameterName.TextureMinFilter, out int value);
                glUtils.Check(this);
                return (MinFilter)value;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.TexParameter(_type, TextureParameterName.TextureMinFilter, (int)value);
                glUtils.Check(this);
            }
        }

        /// <summary>
        /// Gets or sets the magnification filter for this texture.
        /// </summary>
        /// <remarks>
        /// The texture magnification function is used when the pixel being textured maps to an area less than or equal to one texture element.
        /// It sets the texture magnification function to either Nearest or Linear.
        /// Nearest is generally faster than Linear, but it can produce textured images with sharper edges because the transition between texture elements is not as smooth.
        /// The initial value of the magnification filter is Linear.
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexParameteri(GL_TEXTURE_MAG_FILTER), glGetTexParameteriv(GL_TEXTURE_MAG_FILTER)</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MinificationFilter"/>
        public MagFilter MagnificationFilter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetTexParameter(_type, GetTextureParameterName.TextureMagFilter, out int value);
                glUtils.Check(this);
                return (MagFilter)value;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.TexParameter(_type, TextureParameterName.TextureMagFilter, (int)value);
                glUtils.Check(this);
            }
        }

        /// <summary>
        /// Gets or sets the wrap mode for texture coordinate S (horizontal).
        /// </summary>
        /// <remarks>
        /// See <see cref="WrapMode"/> for options and their effects.
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexParameteri(GL_TEXTURE_WRAP_S), glGetTexParameteriv(GL_TEXTURE_WRAP_S)</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        /// <seealso cref="WrapT"/>
        public WrapMode WrapS
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetTexParameter(_type, GetTextureParameterName.TextureWrapS, out int value);
                glUtils.Check(this);
                return (WrapMode)value;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.TexParameter(_type, TextureParameterName.TextureWrapS, (int)value);
                glUtils.Check(this);
            }
        }

        /// <summary>
        /// Gets or sets the wrap mode for texture coordinate T (vertical).
        /// </summary>
        /// <remarks>
        /// See <see cref="WrapMode"/> for options and their effects.
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexParameteri(GL_TEXTURE_WRAP_T), glGetTexParameteriv(GL_TEXTURE_WRAP_T)</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        /// <seealso cref="WrapS"/>
        public WrapMode WrapT
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetTexParameter(_type, GetTextureParameterName.TextureWrapT, out int value);
                glUtils.Check(this);
                return (WrapMode)value;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.TexParameter(_type, TextureParameterName.TextureWrapT, (int)value);
                glUtils.Check(this);
            }
        }

        /// <summary>
        /// Get the 2D texture that is currently bound for the active multitexture unit.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_TEXTURE_BINDING_2D)
        /// </remarks>
        /// <value>The currently bound 2D texture, or null if there is no 2D texture bound.</value>
        /// <seealso cref="Bind"/>
        /// <seealso cref="CurrentCubemapTexture"/>
        public static Texture Current2DTexture
        {
            get
            {
                GL.GetInteger(GetPName.TextureBinding2D, out int value);
                glUtils.Check("Texture");
                if (value == 0)
                    return null;
                return new Texture(value, Type.TwoD);
            }
        }

        /// <summary>
        /// Get the cubemap texture that is currently bound for the active multitexture unit.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_TEXTURE_BINDING_CUBE_MAP)
        /// </remarks>
        /// <value>The currently bound cubemap texture, or null if there is no cubemap texture bound.</value>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Current2DTexture"/>
        public static Texture CurrentCubemapTexture
        {
            get
            {
                GL.GetInteger(GetPName.TextureBindingCubeMap, out int value);
                glUtils.Check("Texture");
                if (value == 0)
                    return null;
                return new Texture(value, Type.CubeMap);
            }
        }

        /// <summary>
        /// Gets the active texture unit.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_ACTIVE_TEXTURE)
        /// </remarks>
        /// <value>The index active multitexture unit, ranging from 0 to <see cref="MaxTextureUnits"/> - 1.</value>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="MaxTextureUnits"/>
        /// <seealso cref="MaxCombinedTextureUnits"/>
        public static int ActiveTextureUnit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.ActiveTexture, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets the maximum supported texture image units that can be used to access texture maps from the fragment shader.
        /// </summary>
        /// <remarks>
        /// The value must be at least 8.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_TEXTURE_IMAGE_UNITS)</para>
        /// </remarks>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="MaxCombinedTextureUnits"/>
        /// <seealso cref="MaxVertexTextureUnits"/>
        public static int MaxTextureUnits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.MaxTextureImageUnits, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets the maximum supported texture image units that can be used to access texture maps from the vertex shader and the fragment processor combined.
        /// </summary>
        /// <remarks>
        /// If both the vertex shader and the fragment processing stage access the same texture image unit, 
        /// then that counts as using two texture image units against this limit.
        /// The value must be at least 8.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS)</para>
        /// </remarks>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="MaxTextureUnits"/>
        /// <seealso cref="MaxVertexTextureUnits"/>
        public static int MaxCombinedTextureUnits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets the maximum supported texture image units that can be used to access texture maps from the vertex shader.
        /// </summary>
        /// <remarks>
        /// The value may be 0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS)</para>
        /// </remarks>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="MaxTextureUnits"/>
        /// <seealso cref="MaxCombinedTextureUnits"/>
        public static int MaxVertexTextureUnits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets a rough estimate of the largest texture that the GL can handle.
        /// </summary>
        /// <remarks>
        /// The value must be at least 64.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_TEXTURE_SIZE)</para>
        /// </remarks>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        public static int MaxTextureSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.MaxTextureSize, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets a rough estimate of the largest cube-map texture that the GL can handle.
        /// </summary>
        /// <remarks>
        /// The value must be at least 16.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_CUBE_MAP_TEXTURE_SIZE)</para>
        /// </remarks>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        public static int MaxCubeMapTextureSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.MaxCubeMapTextureSize, out int value);
                glUtils.Check("Texture");
                return value;
            }
        }

        /// <summary>
        /// Gets a list of symbolic constants indicating which compressed texture formats are available.
        /// </summary>
        /// <remarks>
        /// May be empty.
        ///
        /// <para> <b>OpenGL API</b>: glGetIntegerv(GL_NUM_COMPRESSED_TEXTURE_FORMATS/GL_COMPRESSED_TEXTURE_FORMATS)</para>
        /// </remarks>
        /// <value>A list of compressed texture formats.</value>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUploadCompressed"/>
        public static int[] CompressedTextureFormats
        {
            get
            {
                GL.GetInteger(GetPName.NumCompressedTextureFormats, out int count);
                glUtils.Check("Texture");
                int[] result = new int[count];

                if (count > 0)
                {
                    GL.GetInteger(GetPName.CompressedTextureFormats, result);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets an implementation-specific hint for generating mipmaps.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: this is a global setting that effects all textures.
        ///
        /// <para><b>OpenGL API</b>: glHint, glGetIntegerv(GL_GENERATE_MIPMAP_HINT)</para>
        /// </remarks>
        /// <seealso cref="GenerateMipmap"/>
        public static Hint MipmapHint 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(GetPName.GenerateMipmapHint, out int value);
                glUtils.Check("Texture");
                return (Hint)value;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                GL.Hint(HintTarget.GenerateMipmapHint, (HintMode)value);
                glUtils.Check("Texture");
            }
        }

        internal Texture(int handle, Type type)
        {
            Handle = handle;
            _type = (TextureTarget)type;
        }

        /// <summary>
        /// Creates a texture.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGenTextures
        /// </remarks>
        /// <param name="type">(optional) type of texture to create. Defaults to a 2D texture.</param>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="Copy"/>
        public Texture(Type type = Type.TwoD)
        {
            Handle = GL.GenTexture();
            glUtils.Check(this);
            _type = (TextureTarget)type;
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <remarks>
        /// Lets you create or use a named texture.
        /// Binds the texture name to the target of the current active texture unit.
        /// When a texture is bound to a target, the previous binding for that target is automatically broken.
        ///
        /// <para>When a texture is first bound, it assumes the specified target: 
        /// A first bound texture of type <see cref="Type.TwoD"/> becomes a two-dimensional texture and a first bound texture of 
        /// type <see cref="Type.CubeMap"/> becomes a cube-mapped texture.
        /// The state of a two-dimensional texture immediately after it is first bound is equivalent to the state of the default texture at GL initialization.</para>
        ///
        /// <para>While a texture is bound, GL operations on the target to which it is bound affect the bound texture, 
        /// and queries of the target to which it is bound return state from the bound texture.
        /// In effect, the texture targets become aliases for the textures currently bound to them, 
        /// and the texture name zero refers to the default textures that were bound to them at initialization.</para>
        ///
        /// <para>A texture binding remains active until a different texture is bound to the same target, 
        /// or until the bound texture is deleted.</para>
        ///
        /// <para>Once created, a named texture may be re-bound to its same original target as often as needed.
        /// It is usually much faster to use <c>Bind</c> to bind an existing named texture to one of the texture targets 
        /// than it is to reload the texture image using <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glBindTexture</para>
        /// </remarks>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="IsBound"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind()
        {
            GL.BindTexture(_type, Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Unbinds the texture.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glBindTexture
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="IsBound"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unbind()
        {
            GL.BindTexture(_type, 0);
            glUtils.Check(this);
        }

        /// <summary>
        /// Actives a texture unit and binds this texture to that unit.
        /// </summary>
        /// <remarks>
        /// Once the texture unit is active, it binds the texture by calling <see cref="Bind"/>.
        ///
        /// <para><b>OpenGL API</b>: glActiveTexture, glBindTexture</para>
        /// </remarks>
        /// <param name="textureUnit">index of the texture unit to make active. 
        /// The number of texture units is implementation dependent, but must be at least 8.</param>
        /// <exception cref="GLException">InvalidEnum if <paramref name="textureUnit"/> is greater than the number of supported texture units.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="IsBound"/>
        /// <seealso cref="UnbindFromTextureUnit"/>
        /// <seealso cref="MaxTextureUnits"/>
        /// <seealso cref="MaxCombinedTextureUnits"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindToTextureUnit(int textureUnit)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + textureUnit));
            glUtils.Check(this);
            GL.BindTexture(_type, Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Actives a texture unit and unbinds this texture from that unit.
        /// </summary>
        /// <remarks>
        /// Once the texture unit is active, it unbinds the texture by calling <see cref="Unbind"/>.
        ///
        /// <para><b>OpenGL API</b>: glActiveTexture, glBindTexture</para>
        /// </remarks>
        /// <param name="textureUnit">index of the texture unit to make active. 
        /// The number of texture units is implementation dependent, but must be at least 8.</param>
        /// <exception cref="GLException">InvalidEnum if <paramref name="textureUnit"/> is greater than the number of supported texture units.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="IsBound"/>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="MaxTextureUnits"/>
        /// <seealso cref="MaxCombinedTextureUnits"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnbindFromTextureUnit(int textureUnit)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + textureUnit));
            glUtils.Check(this);
            GL.BindTexture(_type, 0);
            glUtils.Check(this);
        }

        /// <summary>
        /// Generate a complete set of mipmaps for this texture object.
        /// </summary>
        /// <remarks>
        /// Computes a complete set of mipmap arrays derived from the zero level array.
        /// Array levels up to and including the 1x1 dimension texture image are replaced with the derived arrays, 
        /// regardless of previous contents.
        /// The zero level texture image is left unchanged.
        ///
        /// <para>The internal formats of the derived mipmap arrays all match those of the zero level texture image.
        /// The dimensions of the derived arrays are computed by halving the width and height of the zero level texture image, 
        /// then in turn halving the dimensions of each array level until the 1x1 dimension texture image is reached.</para>
        ///
        /// <para>The contents of the derived arrays are computed by repeated filtered reduction of the zero level array.
        /// No particular filter algorithm is required, though a box filter is recommended.
        /// <see cref="MipmapHint"/> may be called to express a preference for speed or quality of filtering.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glGenerateMipmap</para>
        /// </remarks>
        /// <exception cref="GLException">InvalidOperation if this is a cube map texture, but its six faces do not share indentical widths, heights, formats, and types.</exception>
        /// <exception cref="GLException">InvalidOperation if either the width or height of the zero level array is not a power of two.</exception>
        /// <exception cref="GLException">InvalidOperation if the zero level array is stored in a compressed internal format.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="Framebuffer.AttachTexture"/>
        /// <seealso cref="MipmapHint"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GenerateMipmap()
        {
            glUtils.CheckBinding((All)_type, Handle, this);
            GL.GenerateMipmap(_type);
            glUtils.Check(this);
        }

        /// <summary>
        /// Reserve memory for a texture of given dimensions.
        /// </summary>
        /// <remarks>
        /// This creates the texture in memory, but does not set the image data yet.
        ///
        /// <para>Texturing maps a portion of a specified texture image onto each graphical primitive for which texturing is active.
        /// Texturing is active when the current fragment shader or vertex shader makes use of built-in texture lookup functions.</para>
        ///
        /// <para>To reserve memory for texture images, call <c>Reserve</c>.
        /// The arguments describe the parameters of the texture image, such as height, width, level-of-detail number (see <see cref="MinificationFilter"/>), and format.
        /// The other arguments describe how the image is represented in memory.</para>       
        ///
        /// <para><b>Note</b>: This method specifies a two-dimensional or cube-map texture for the current texture unit, specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexImage2D</para>
        /// </remarks>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="width">the width of the texture image. All implementations support 2D texture images that are at least 64 texels wide and cube-mapped texture images that are at least 16 texels wide.</param>
        /// <param name="height">the height of the texture image. All implementations support 2D texture images that are at least 64 texels high and cube-mapped texture images that are at least 16 texels high.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="type">(optional) data type the texel data. Defaults to UnsignedByte.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if this is a cube map texture and the <paramref name="width"/> and <paramref name="height"/> parameters are not equal.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0 or greater than the maximum texture size.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort565 and <paramref name="format"/> is not Rgb.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not Rgba.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(gl.PixelFormat format, int width, int height, int level = 0, gl.PixelDataType type = gl.PixelDataType.UnsignedByte, int cubeTarget = 0) 
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.TexImage2D(GetTarget(cubeTarget), level, (PixelInternalFormat)format, width, height, 0, (TKPixelFormat)format, (PixelType)type, IntPtr.Zero);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads an image to the texture.
        /// </summary>
        /// <remarks>
        /// This creates the texture in memory.
        ///
        /// <para>Texturing maps a portion of a specified texture image onto each graphical primitive for which texturing is active.
        /// Texturing is active when the current fragment shader or vertex shader makes use of built-in texture lookup functions.</para>
        ///
        /// <para>To define texture images, call <c>Upload</c>.
        /// The arguments describe the parameters of the texture image, such as height, width, level-of-detail number (see <see cref="MinificationFilter"/>), and format.
        /// The other arguments describe how the image is represented in memory.</para>
        ///
        /// <para>Data is read from <paramref name="data"/> as a sequence of unsigned bytes or shorts, depending on <paramref name="type"/>.
        /// Color components are converted to floating point based on the <paramref name="type"/>.</para>
        ///
        /// <para><paramref name="width"/> × <paramref name="height"/> texels are read from memory, starting at location <paramref name="data"/>.
        /// By default, these texels are taken from adjacent memory locations, except that after all width texels are read, 
        /// the read pointer is advanced to the next four-byte boundary.
        /// The four-byte row alignment is specified by <see cref="gl.PixelStore"/> with argument UnpackAlignment, 
        /// and it can be set to one, two, four, or eight bytes.</para>
        ///
        /// <para>The first element corresponds to the lower left corner of the texture image.
        /// Subsequent elements progress left-to-right through the remaining texels in the lowest row of the texture image, 
        /// and then in successively higher rows of the texture image.
        /// The final element corresponds to the upper right corner of the texture image.</para>
        ///
        /// <para><b>Note</b>: to reserve memory for the texture without specifying texture data, use <see cref="Reserve(gl.PixelFormat, int, int, int, gl.PixelDataType, int)"/> instead.
        /// You can then <c>upload</c> subtextures to initialize this texture memory.
        /// The image is undefined if the user tries to apply an uninitialized portion of the texture image to a primitive.</para>
        ///
        /// <para><b>Note</b>: This method specifies a two-dimensional or cube-map texture for the current texture unit, specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexImage2D</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="width">the width of the texture image. All implementations support 2D texture images that are at least 64 texels wide and cube-mapped texture images that are at least 16 texels wide.</param>
        /// <param name="height">the height of the texture image. All implementations support 2D texture images that are at least 64 texels high and cube-mapped texture images that are at least 16 texels high.</param>
        /// <param name="data">one-dimensional array with the image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="type">(optional) data type the texel data. Defaults to UnsignedByte.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if this is a cube map texture and the <paramref name="width"/> and <paramref name="height"/> parameters are not equal.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0 or greater than the maximum texture size.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort565 and <paramref name="format"/> is not Rgb.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not Rgba.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        /// <seealso cref="Reserve"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Upload<T>(gl.PixelFormat format, int width, int height, T[] data, int level = 0, gl.PixelDataType type = gl.PixelDataType.UnsignedByte, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.TexImage2D(GetTarget(cubeTarget), level, (PixelInternalFormat)format, width, height, 0, (TKPixelFormat)format, (PixelType)type, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads an image to the texture.
        /// </summary>
        /// <remarks>
        /// This creates the texture in memory.
        ///
        /// <para>Texturing maps a portion of a specified texture image onto each graphical primitive for which texturing is active.
        /// Texturing is active when the current fragment shader or vertex shader makes use of built-in texture lookup functions.</para>
        ///
        /// <para>To define texture images, call <c>Upload</c>.
        /// The arguments describe the parameters of the texture image, such as height, width, level-of-detail number (see <see cref="MinificationFilter"/>), and format.
        /// The other arguments describe how the image is represented in memory.</para>
        ///
        /// <para>Data is read from <paramref name="data"/> as a sequence of unsigned bytes or shorts, depending on <paramref name="type"/>.
        /// Color components are converted to floating point based on the <paramref name="type"/>.</para>
        ///
        /// <para><paramref name="width"/> × <paramref name="height"/> texels are read from memory, starting at location <paramref name="data"/>.
        /// By default, these texels are taken from adjacent memory locations, except that after all width texels are read, 
        /// the read pointer is advanced to the next four-byte boundary.
        /// The four-byte row alignment is specified by <see cref="gl.PixelStore"/> with argument UnpackAlignment, 
        /// and it can be set to one, two, four, or eight bytes.</para>
        ///
        /// <para>The first element corresponds to the lower left corner of the texture image.
        /// Subsequent elements progress left-to-right through the remaining texels in the lowest row of the texture image, 
        /// and then in successively higher rows of the texture image.
        /// The final element corresponds to the upper right corner of the texture image.</para>
        ///
        /// <para><b>Note</b>: to reserve memory for the texture without specifying texture data, use <see cref="Reserve(gl.PixelFormat, int, int, int, gl.PixelDataType, int)"/> instead.
        /// You can then <c>upload</c> subtextures to initialize this texture memory.
        /// The image is undefined if the user tries to apply an uninitialized portion of the texture image to a primitive.</para>
        ///
        /// <para><b>Note</b>: This method specifies a two-dimensional or cube-map texture for the current texture unit, specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexImage2D</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="width">the width of the texture image. All implementations support 2D texture images that are at least 64 texels wide and cube-mapped texture images that are at least 16 texels wide.</param>
        /// <param name="height">the height of the texture image. All implementations support 2D texture images that are at least 64 texels high and cube-mapped texture images that are at least 16 texels high.</param>
        /// <param name="data">two-dimensional array with the image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="type">(optional) data type the texel data. Defaults to UnsignedByte.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if this is a cube map texture and the <paramref name="width"/> and <paramref name="height"/> parameters are not equal.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0 or greater than the maximum texture size.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort565 and <paramref name="format"/> is not Rgb.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not Rgba.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        /// <seealso cref="Reserve"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Upload<T>(gl.PixelFormat format, int width, int height, T[,] data, int level = 0, gl.PixelDataType type = gl.PixelDataType.UnsignedByte, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.TexImage2D(GetTarget(cubeTarget), level, (PixelInternalFormat)format, width, height, 0, (TKPixelFormat)format, (PixelType)type, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads a part of an image to the texture.
        /// </summary>
        /// <remarks>
        /// This updates the texture in memory.
        ///
        /// <para>This method redefines a contiguous subregion of an existing two-dimensional texture image.
        /// The texels referenced by data replace the portion of the existing texture array with X indices <paramref name="xOffset"/> and <paramref name="xOffset"/> + <paramref name="width"/> - 1, 
        /// inclusive, and Y indices <paramref name="yOffset"/> and <paramref name="yOffset"/> + <paramref name="height"/> - 1, inclusive.
        /// This region may not include any texels outside the range of the texture array as it was originally specified.
        /// It is not an error to specify a subtexture with zero width or height, but such a specification has no effect.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexSubImage2D</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="xOffset">texel offset in the X direction within the texture array.</param>
        /// <param name="yOffset">texel offset in the Y direction within the texture array.</param>
        /// <param name="width">the width of the texture subimage.</param>
        /// <param name="height">the height of the texture subimage.</param>
        /// <param name="data">one-dimensional array with the image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="type">(optional) data type the texel data. Defaults to UnsignedByte.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="xOffset"/> &lt; 0 or <paramref name="xOffset"/> + <paramref name="width"/> is greater than the width of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="yOffset"/> &lt; 0 or <paramref name="yOffset"/> + <paramref name="height"/> is greater than the height of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0.</exception>
        /// <exception cref="GLException">InvalidOperation if the texture array has not been defined by a previous <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>, 
        /// <see cref="Reserve"/> or <see cref="Copy"/> operation 
        /// whose format matches the <paramref name="format"/> parameter of this method.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort565 and <paramref name="format"/> is not Rgb.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not Rgba.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubUpload<T>(gl.PixelFormat format, int xOffset, int yOffset, int width, int height, T[] data, int level = 0, gl.PixelDataType type = gl.PixelDataType.UnsignedByte, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.TexSubImage2D(GetTarget(cubeTarget), level, xOffset, yOffset, width, height, (TKPixelFormat)format, (PixelType)type, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads a part of an image to the texture.
        /// </summary>
        /// <remarks>
        /// This updates the texture in memory.
        ///
        /// <para>This method redefines a contiguous subregion of an existing two-dimensional texture image.
        /// The texels referenced by data replace the portion of the existing texture array with X indices <paramref name="xOffset"/> and <paramref name="xOffset"/> + <paramref name="width"/> - 1, 
        /// inclusive, and Y indices <paramref name="yOffset"/> and <paramref name="yOffset"/> + <paramref name="height"/> - 1, inclusive.
        /// This region may not include any texels outside the range of the texture array as it was originally specified.
        /// It is not an error to specify a subtexture with zero width or height, but such a specification has no effect.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glTexSubImage2D</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="xOffset">texel offset in the X direction within the texture array.</param>
        /// <param name="yOffset">texel offset in the Y direction within the texture array.</param>
        /// <param name="width">the width of the texture subimage.</param>
        /// <param name="height">the height of the texture subimage.</param>
        /// <param name="data">two-dimensional array with the image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="type">(optional) data type the texel data. Defaults to UnsignedByte.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="xOffset"/> &lt; 0 or <paramref name="xOffset"/> + <paramref name="width"/> is greater than the width of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="yOffset"/> &lt; 0 or <paramref name="yOffset"/> + <paramref name="height"/> is greater than the height of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0.</exception>
        /// <exception cref="GLException">InvalidOperation if the texture array has not been defined by a previous <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>, 
        /// <see cref="Reserve"/> or <see cref="Copy"/> operation 
        /// whose format matches the <paramref name="format"/> parameter of this method.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort565 and <paramref name="format"/> is not Rgb.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="type"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not Rgba.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubUpload<T>(gl.PixelFormat format, int xOffset, int yOffset, int width, int height, T[,] data, int level = 0, gl.PixelDataType type = gl.PixelDataType.UnsignedByte, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.TexSubImage2D(GetTarget(cubeTarget), level, xOffset, yOffset, width, height, (TKPixelFormat)format, (PixelType)type, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads a compressed image to the texture.
        /// </summary>
        /// <remarks>
        /// This creates the texture in memory.
        ///
        /// <para>Texturing maps a portion of a specified texture image onto each graphical primitive for which texturing is active.
        /// Texturing is active when the current fragment shader or vertex shader makes use of built-in texture lookup functions.</para>
        ///
        /// <para>This method defines a two-dimensional texture image or cube-map texture image using compressed image data from client memory.
        /// The texture image is decoded according to the extension specification defining the specified <paramref name="format"/>.
        /// OpenGL ES defines no specific compressed texture formats, but does provide a mechanism to obtain symbolic constants for such formats provided by extensions.
        /// The list of specific compressed texture formats supported can be obtained by <see cref="CompressedTextureFormats"/>.</para>
        ///
        /// <para><b>Note</b>: a GL implementation may choose to store the texture array at any internal resolution it chooses.</para>
        ///
        /// <para><b>Note</b>: this method specifies a two-dimensional or cube-map texture for the current texture unit, specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>Note</b>: Undefined results, including abnormal program termination, are generated if data is not encoded in a manner consistent with the extension specification defining the internal compression format.</para>
        /// 
        /// <para><b>OpenGL API</b>: glCompressedTexImage2D</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="format">the compressed format of the texel data.</param>
        /// <param name="width">the width of the texture image. All implementations support 2D texture images that are at least 64 texels wide and cube-mapped texture images that are at least 16 texels wide.</param>
        /// <param name="height">the height of the texture image. All implementations support 2D texture images that are at least 64 texels high and cube-mapped texture images that are at least 16 texels high.</param>
        /// <param name="data">array containing the compressed image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidEnum if <paramref name="format"/> is not a supported format returned in <see cref="CompressedTextureFormats"/>.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0 or greater than the maximum texture size.</exception>
        /// <exception cref="GLException">InvalidValue if the length of <paramref name="data"/> is not consistent with the format, dimensions, and contents of the specified compressed image data.</exception>
        /// <exception cref="GLException">InvalidOperation if parameter combinations are not supported by the specific compressed internal format as specified in the specific texture compression extension.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        /// <seealso cref="CompressedTextureFormats"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadCompressed<T>(int format, int width, int height, T[] data, int level = 0, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.CompressedTexImage2D(GetTarget(cubeTarget), level, (PixelInternalFormat)format, width, height, 0, data.Length, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Uploads a part of a compressed image to the texture.
        /// </summary>
        /// <remarks>
        /// This updates the texture in memory.
        ///
        /// <para>Texturing maps a portion of a specified texture image onto each graphical primitive for which texturing is active.
        /// Texturing is active when the current fragment shader or vertex shader makes use of built-in texture lookup functions.</para>
        ///
        /// <para>This method redefines a contiguous subregion of an existing two-dimensional texture image.
        /// The texels referenced by <paramref name="data"/> replace the portion of the existing texture array with X indices <paramref name="xOffset"/> and <paramref name="xOffset"/> + <paramref name="width"/> - 1, 
        /// and the Y indices <paramref name="yOffset"/> and <paramref name="yOffset"/> + <paramref name="height"/> - 1, inclusive.
        /// This region may not include any texels outside the range of the texture array as it was originally specified.
        /// It is not an error to specify a subtexture with width of 0, but such a specification has no effect.</para>
        ///
        /// <para><paramref name="format"/> must be the same extension-specified compressed-texture format previously specified by <see cref="UploadCompressed"/>.</para>
        ///
        /// <para><b>Note</b>: this method specifies a two-dimensional or cube-map texture for the current texture unit, specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>Note</b>: Undefined results, including abnormal program termination, are generated if data is not encoded in a manner consistent with the extension specification defining the internal compression format.</para>
        ///
        /// <para><b>OpenGL API</b>: glCompressedTexSubImage2D</para>
        /// </remarks>
        /// <typeparam name="T">The type of data in the <paramref name="data"/> array.</typeparam>
        /// <param name="format">the compressed format of the texel data.</param>
        /// <param name="xOffset">texel offset in the X direction within the texture array.</param>
        /// <param name="yOffset">texel offset in the Y direction within the texture array.</param>
        /// <param name="width">the width of the texture subimage.</param>
        /// <param name="height">the height of the texture subimage.</param>
        /// <param name="data">array containing the compressed image data in memory.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidEnum if <paramref name="format"/> is not a supported format returned in <see cref="CompressedTextureFormats"/>.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="xOffset"/> &lt; 0 or <paramref name="xOffset"/> + <paramref name="width"/> is greater than the width of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="yOffset"/> &lt; 0 or <paramref name="yOffset"/> + <paramref name="height"/> is greater than the height of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0.</exception>
        /// <exception cref="GLException">InvalidValue if the length of <paramref name="data"/> is not consistent with the format, dimensions, and contents of the specified compressed image data.</exception>
        /// <exception cref="GLException">InvalidOperation if the texture array has not been defined by a previous <see cref="UploadCompressed"/> operation whose format matches the <paramref name="format"/> of this method.</exception>
        /// <exception cref="GLException">InvalidOperation if parameter combinations are not supported by the specific compressed internal format as specified in the specific texture compression extension.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        /// <seealso cref="CompressedTextureFormats"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubUploadCompressed<T>(int format, int xOffset, int yOffset, int width, int height, T[] data, int level = 0, int cubeTarget = 0) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.CompressedTexSubImage2D(GetTarget(cubeTarget), level, xOffset, yOffset, width, height, (TKPixelFormat)format, data.Length, data);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Copies pixels from the current framebuffer into the texture.
        /// </summary>
        /// <remarks>
        /// This method defines a two-dimensional texture image or cube-map texture image with pixels from the current framebuffer
        /// (rather than from client memory, as is the case for <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>).
        ///
        /// <para>The screen-aligned pixel rectangle with lower left corner at (<paramref name="left"/>, <paramref name="bottom"/>) 
        /// and with a width of <paramref name="width"/> and a height of <paramref name="height"/> 
        /// defines the texture array at the mipmap level specified by <paramref name="level"/>.
        /// <paramref name="format"/> specifies the internal format of the texture array.</para>
        ///
        /// <para>The pixels in the rectangle are processed exactly as if <see cref="Framebuffer.ReadPixels"/> had been called 
        /// with format set to Rgba, but the process stops just after conversion of Rgba values.
        /// Subsequent processing is identical to that described for <see cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>, 
        /// beginning with the clamping of the R, G, B, and A values to the range 0-1 and then conversion to the texture's internal format for storage in the texel array.</para>
        ///
        /// <para>The components required for <paramref name="format"/> must be a subset of those present in the framebuffer's format.
        /// For example, a Rgba framebuffer can be used to supply components for any format.
        /// However, a Rgb framebuffer can only be used to supply components for Rgb or Luminance base internal format textures, 
        /// not Alpha, LuminanceAlpha or Rgba textures.</para>
        ///
        /// <para>Pixel ordering is such that lower <paramref name="left"/> and <paramref name="bottom"/> screen coordinates correspond to lower S and T texture coordinates.</para>
        ///
        /// <para>If any of the pixels within the specified rectangle are outside the framebuffer associated with the current rendering context, 
        /// then the values obtained for those pixels are undefined.</para>
        ///
        /// <para><b>Note</b>: a GL implementation may choose to store the texture array at any internal resolution it chooses.</para>
        ///
        /// <para><b>Note</b>: an image with height or width of 0 indicates a NULL texture.</para>
        ///
        /// <para><b>Note</b>: This method specifies a two-dimensional or cube-map texture for the current texture unit, 
        /// specified with <see cref="BindToTextureUnit"/>.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glCopyTexImage2D</para>
        /// </remarks>
        /// <param name="format">the format of the texel data.</param>
        /// <param name="left">window coordinate of the left corner of the rectangular region of pixels to be copied.</param>
        /// <param name="bottom">window coordinate of the bottom corner of the rectangular region of pixels to be copied.</param>
        /// <param name="width">the width of the texture image. All implementations support 2D texture images that are at least 64 texels wide and cube-mapped texture images that are at least 16 texels wide.</param>
        /// <param name="height">the height of the texture image. All implementations support 2D texture images that are at least 64 texels high and cube-mapped texture images that are at least 16 texels high.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if this is a cube map texture and the <paramref name="width"/> and <paramref name="height"/> parameters are not equal.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0 or greater than then maximum texture size.</exception>
        /// <exception cref="GLException">InvalidOperation if the currently bound framebuffer's format does not contain a superset of the components required by the base format of <paramref name="format"/>.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Framebuffer.CompletenessStatus"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="SubCopy"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(gl.PixelFormat format, int left, int bottom, int width, int height, int level = 0, int cubeTarget = 0)
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.CopyTexImage2D(GetTarget(cubeTarget), level, (PixelInternalFormat)format, left, bottom, width, height, 0);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        /// <summary>
        /// Copies pixels from a part of the current framebuffer into the texture.
        /// </summary>
        /// <remarks>
        /// This method replaces a rectangular portion of a two-dimensional texture image or cube-map texture image with pixels from the current framebuffer
        /// (rather than from client memory, as is the case for <see cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>).
        ///
        /// <para>The screen-aligned pixel rectangle with lower left corner at <paramref name="left"/>, <paramref name="bottom"/> and with width <paramref name="width"/> and height <paramref name="height"/>
        /// replaces the portion of the texture array with X indices <paramref name="xOffset"/> through <paramref name="xOffset"/> + <paramref name="width"/> - 1, inclusive, 
        /// and Y indices <paramref name="yOffset"/> through <paramref name="yOffset"/> + <paramref name="height"/> - 1, inclusive, 
        /// at the mipmap level specified by <paramref name="level"/>.</para>
        ///
        /// <para>The pixels in the rectangle are processed exactly as if <see cref="Framebuffer.ReadPixels"/> had been called with format set to Rgba, 
        /// but the process stops just after conversion of Rgba values.
        /// Subsequent processing is identical to that described for <see cref="SubUpload{T}(gl.PixelFormat, int, int, int, int, T[], int, gl.PixelDataType, int)"/>, 
        /// beginning with the clamping of the R, G, B, and A values to the range 0-1 and then 
        /// conversion to the texture's internal format for storage in the texel array.</para>
        ///
        /// <para>The destination rectangle in the texture array may not include any texels outside the texture array as it was originally specified.
        /// It is not an error to specify a subtexture with zero width or height, but such a specification has no effect.</para>
        ///
        /// <para>If any of the pixels within the specified rectangle are outside the framebuffer associated with the current rendering context, 
        /// then the values obtained for those pixels are undefined.</para>
        ///
        /// <para>No change is made to the internal format, width, or height parameters of the texture array or to texel values outside the specified subregion.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this texture is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glCopyTexSubImage2D</para>
        /// </remarks>
        /// <param name="xOffset">texel offset in the X direction within the texture array.</param>
        /// <param name="yOffset">texel offset in the Y direction within the texture array.</param>
        /// <param name="left">window coordinate of the left corner of the rectangular region of pixels to be copied.</param>
        /// <param name="bottom">window coordinate of the bottom corner of the rectangular region of pixels to be copied.</param>
        /// <param name="width">the width of the texture subimage.</param>
        /// <param name="height">the height of the texture subimage.</param>
        /// <param name="level">(optional) level-of-detail number if updating separate mipmap levels. Level 0 (default) is the base image level. Level N is the Nth mipmap reduction image.</param>
        /// <param name="cubeTarget">(optional) if this texture is a cube texture, then this parameter specifies which of the 6 cube faces to update. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="level"/> is less than 0 or greater than the maximum level.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="xOffset"/> &lt; 0 or <paramref name="xOffset"/> + <paramref name="width"/> is greater than the width of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="yOffset"/> &lt; 0 or <paramref name="yOffset"/> + <paramref name="height"/> is greater than the height of this texture.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> or <paramref name="height"/> is less than 0.</exception>
        /// <exception cref="GLException">InvalidOperation if the currently bound framebuffer's format does not contain a superset of the components required by the base format.</exception>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="BindToTextureUnit"/>
        /// <seealso cref="Framebuffer.CompletenessStatus"/>
        /// <seealso cref="Reserve"/>
        /// <seealso cref="Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        /// <seealso cref="UploadCompressed"/>
        /// <seealso cref="SubUploadCompressed"/>
        /// <seealso cref="Copy"/>
        /// <seealso cref="MaxTextureSize"/>
        /// <seealso cref="MaxCubeMapTextureSize"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubCopy(int xOffset, int yOffset, int left, int bottom, int width, int height, int level = 0, int cubeTarget = 0)
        {
            glUtils.CheckBinding((All)_type, Handle, this);
#pragma warning disable CS0618 // Type or member is obsolete
            GL.CopyTexSubImage2D(GetTarget(cubeTarget), level, xOffset, yOffset, left, bottom, width, height);
#pragma warning restore CS0618 // Type or member is obsolete
            glUtils.Check(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TextureTarget GetTarget(int cubeTarget)
        {
            if (_type == TextureTarget.TextureCubeMap)
                return (TextureTarget)((int)TextureTarget.TextureCubeMapPositiveX + cubeTarget);
            return _type;
        }

        /// <summary>
        /// Deletes the texture.
        /// </summary>
        /// <remarks>
        /// After a texture is deleted, it has no contents or dimensionality.
        /// If a texture that is currently bound is deleted, the binding reverts to 0 (the default texture).
        ///
        /// <para><b>OpenGL API</b>: glDeleteTextures</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        protected override void DisposeHandle()
        {
            GL.DeleteTexture(Handle);
            glUtils.Check(this);
        }
    }
}
