using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

#if __ANDROID__ || __IOS__
using TKPrimkitiveType = OpenTK.Graphics.ES20.BeginMode;
#else
using TKPrimkitiveType = OpenTK.Graphics.ES20.PrimitiveType;
#endif
using TKPixelFormat = OpenTK.Graphics.ES20.PixelFormat;
using TKStencilOp = OpenTK.Graphics.ES20.StencilOp;
using TKBlendEquationMode = OpenTK.Graphics.ES20.BlendEquationMode;

namespace Ooogles
{
    /// <summary>
    /// Static class for OpenGL APIs that are not tied to a specific object.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public static class gl
#pragma warning restore IDE1006 // Naming Styles
    {
        #region Enums
        /// <summary>
        /// OpenGL error codes
        /// </summary>
        public enum Error
        {
            /// <summary>
            /// No error has been recorded.
            /// </summary>
            NoError = (int)ErrorCode.NoError,

            /// <summary>
            /// An unacceptable value is specified for an enumerated argument.
            /// The offending command is ignored and has no other side effect than to set the error flag.
            /// </summary>
            InvalidEnum = (int)ErrorCode.InvalidEnum,

            /// <summary>
            /// A numeric argument is out of range.
            /// The offending command is ignored and has no other side effect than to set the error flag.
            /// </summary>
            InvalidValue = (int)ErrorCode.InvalidValue,

            /// <summary>
            /// The specified operation is not allowed in the current state. 
            /// The offending command is ignored and has no other side effect than to set the error flag.
            /// </summary>
            InvalidOperation = (int)ErrorCode.InvalidOperation,

            /// <summary>
            /// The command is trying to render to or read from the framebuffer while the currently bound framebuffer is not framebuffer complete.
            /// The offending command is ignored and has no other side effect than to set the error flag.
            /// </summary>
            InvalidFramebufferOperation = (int)ErrorCode.InvalidFramebufferOperation,

            /// <summary>
            /// There is not enough memory left to execute the command.
            /// The state of the GL is undefined, except for the state of the error flags, after this error is recorded.
            /// </summary>
            OutOfMemory = (int)ErrorCode.OutOfMemory
        }

        /// <summary>
        /// One or more buffers to clear using <see cref="gl.Clear(ClearBuffers)"/>.
        /// </summary>
        [Flags]
        public enum ClearBuffers
        {
            /// <summary>
            /// To clear the depth buffer
            /// </summary>
            Depth = (int)ClearBufferMask.DepthBufferBit,

            /// <summary>
            /// To clear the stencil buffer
            /// </summary>
            Stencil = (int)ClearBufferMask.StencilBufferBit,

            /// <summary>
            /// To clear the color buffer
            /// </summary>
            Color = (int)ClearBufferMask.ColorBufferBit
        }

        /// <summary>
        /// Supported primitive types
        /// </summary>
        public enum PrimitiveType
        {
            /// <summary>
            /// Single points.
            /// </summary>
            Points = (int)TKPrimkitiveType.Points,

            /// <summary>
            /// Single lines.
            /// </summary>
            Lines = (int)TKPrimkitiveType.Lines,

            /// <summary>
            /// Connected lines.
            /// </summary>
            LineStrip = (int)TKPrimkitiveType.LineStrip,

            /// <summary>
            /// Connected lines, closed to form a polygon.
            /// </summary>
            LineLoop = (int)TKPrimkitiveType.LineLoop,

            /// <summary>
            /// Single triangles.
            /// </summary>
            Triangles = (int)TKPrimkitiveType.Triangles,

            /// <summary>
            /// A strip of triangles.
            /// </summary>
            TriangleStrip = (int)TKPrimkitiveType.TriangleStrip,

            /// <summary>
            /// A fan of triangles.
            /// </summary>
            TriangleFan = (int)TKPrimkitiveType.TriangleFan
        }

        /// <summary>
        /// Supported types of indices
        /// </summary>
        public enum IndexType
        {
            /// <summary>
            /// 8-bit unsigned integer.
            /// Corresponds to C#'s byte type.
            /// </summary>
            UnsignedByte = (int)DrawElementsType.UnsignedByte,

            /// <summary>
            /// 16-bit unsigned integer.
            /// Corresponds to C#'s ushort type.
            /// </summary>
            UnsingedShort = (int)DrawElementsType.UnsignedShort
        }

        /// <summary>
        /// Supported pixel data types
        /// </summary>
        public enum PixelDataType
        {
            /// <summary>
            /// Each byte is interpreted as one color component (red, green, blue or alpha).
            /// When converted to floating point, the value is divided by 255.
            /// </summary>
            UnsignedByte = (int)PixelType.UnsignedByte,

            /// <summary>
            /// A single 16-bit integer contains 5 bits for the red component, 6 bits for the green component and 5 bits for the blue component.
            /// When converted to floating point, the red and blue components are divided by 31 and the green component is divided by 63.
            /// </summary>
            UnsignedShort565 = (int)PixelType.UnsignedShort565,

            /// <summary>
            /// A single 16-bit integer contains all components, with 4 bits for each component.
            /// When converted to floating point, every component is divided by 15.
            /// </summary>
            UnsignedShort4444 = (int)PixelType.UnsignedShort4444,

            /// <summary>
            /// A single 16-bit integer contains all components, with 5 bits for the red, green and blue components, and 1 bit for the alpha component.
            /// When converted to floating point, the red, green and blue components are divided by 31 and the alpha component is used as-is.
            /// </summary>
            UnsignedShort5551 = (int)PixelType.UnsignedShort5551
        }

        /// <summary>
        /// Supported pixel formats
        /// </summary>
        public enum PixelFormat
        {
            /// <summary>
            /// Each element is a single alpha component.
            /// The GL converts it to floating point and assembles it into an RGBA element by attaching 0 for red, green, and blue.
            /// Each component is then clamped to the range [0,1].
            /// </summary>
            Alpha = (int)TKPixelFormat.Alpha,

            /// <summary>
            /// Each element is an RGB triple.
            /// The GL converts it to floating point and assembles it into an RGBA element by attaching 1 for alpha.
            /// Each component is then clamped to the range [0,1].
            /// </summary>
            Rgb = (int)TKPixelFormat.Rgb,

            /// <summary>
            /// Each element contains all four components.
            /// The GL converts it to floating point, then each component is clamped to the range [0,1].
            /// </summary>
            Rgba = (int)TKPixelFormat.Rgba,

            /// <summary>
            /// Each element is a single luminance value.
            /// The GL converts it to floating point, then assembles it into an RGBA element by replicating the luminance value three times for red, green, and blue and attaching 1 for alpha.
            /// Each component is then clamped to the range [0,1].
            /// </summary>
            Luminance = (int)TKPixelFormat.Luminance,

            /// <summary>
            /// Each element is a luminance/alpha pair.
            /// The GL converts it to floating point, then assembles it into an RGBA element by replicating the luminance value three times for red, green, and blue.
            /// Each component is then clamped to the range [0,1].
            /// </summary>
            LuminanceAlpha = (int)TKPixelFormat.LuminanceAlpha
        }

        /// <summary>
        /// Faces used for culling and stencilling.
        /// </summary>
        public enum Face
        {
            /// <summary>
            /// Front-facing polygons are culled.
            /// </summary>
            Front = (int)CullFaceMode.Front,

            /// <summary>
            /// Back-facing polygons are culled (the default).
            /// </summary>
            Back = (int)CullFaceMode.Back,

            /// <summary>
            /// Front- and back-facing polygons are culled.
            /// </summary>
            FrontAndBack = (int)CullFaceMode.FrontAndBack
        }

        /// <summary>
        /// Specifies the orientation of front-facing polygons, as used by <see cref="gl.FrontFace"/>.
        /// </summary>
        public enum FaceOrientation
        {
            /// <summary>
            /// Clockwise winding
            /// </summary>
            Clockwise = (int)FrontFaceDirection.Cw,

            /// <summary>
            /// Counter-clockwise winding (default)
            /// </summary>
            CounterClockwise = (int)FrontFaceDirection.Ccw
        }

        /// <summary>
        /// OpenGL capabilities than can be enabled and disabled (using <see cref="Enable"/> and <see cref="Disable"/>)
        /// </summary>
        public enum Capability
        {
            /// <summary>
            /// If enabled, blend the computed fragment color values with the values in the color buffers.
            /// See <see cref="gl.BlendFunc"/>.
            /// </summary>
            Blend = (int)EnableCap.Blend,

            /// <summary>
            /// If enabled, cull polygons based on their winding in window coordinates.
            /// See <see cref="gl.CullFace"/>.
            /// </summary>
            CullFace = (int)EnableCap.CullFace,

            /// <summary>
            /// If enabled, do depth comparisons and update the depth buffer. 
            /// Note that even if the depth buffer exists and the depth mask is non-zero, the depth buffer is not updated if the depth test is disabled.
            /// See <see cref="gl.DepthFunc"/> and <see cref="gl.DepthRange"/>.
            /// </summary>
            DepthTest = (int)EnableCap.DepthTest,

            /// <summary>
            /// If enabled, dither color components or indices before they are written to the color buffer.
            /// </summary>
            Dither = (int)EnableCap.Dither,

            /// <summary>
            /// If enabled, an offset is added to depth values of a polygon's fragments produced by rasterization.
            /// See <see cref="gl.PolygonOffset"/>
            /// </summary>
            PolygonOffsetFill = (int)EnableCap.PolygonOffsetFill,

            /// <summary>
            /// If enabled, compute a temporary coverage value where each bit is determined by the alpha value at the corresponding sample location. 
            /// The temporary coverage value is then ANDed with the fragment coverage value.
            /// </summary>
            SampleAlphaToCoverage = (int)EnableCap.SampleAlphaToCoverage,

            /// <summary>
            /// If enabled, the fragment's coverage is ANDed with the temporary coverage value.
            /// See <see cref="gl.SampleCoverage"/>
            /// </summary>
            SampleCoverage = (int)EnableCap.SampleCoverage,

            /// <summary>
            /// If enabled, discard fragments that are outside the scissor rectangle.
            /// See <see cref="gl.Scissor"/>
            /// </summary>
            ScissorTest = (int)EnableCap.ScissorTest,

            /// <summary>
            /// If enabled, do stencil testing and update the stencil buffer.
            /// See <see cref="gl.StencilFunc"/> and <see cref="gl.StencilOp"/>
            /// </summary>
            StencilTest = (int)EnableCap.StencilTest
        }

        /// <summary>
        /// Pixel storage modes, as used by <see cref="gl.PixelStore"/>.
        /// </summary>
        public enum PixelStoreMode
        {
            /// <summary>
            /// Specifies the packing of pixel data downloading from the GPU into client memory.
            /// </summary>
            PackAlignment = (int)PixelStoreParameter.PackAlignment,

            /// <summary>
            /// Specifies the packing of pixel data when uploading from client memory to the GPU.
            /// </summary>
            UnpackAlignment = (int)PixelStoreParameter.UnpackAlignment
        }

        /// <summary>
        /// Allowed values for TGLPixelStorage mode, as used by <see cref="gl.PixelStore"/>.
        /// </summary>
        public enum PixelStoreValue
        {
            /// <summary>
            /// Pixel data is aligned on a byte boundary
            /// </summary>
            One = 1,

            /// <summary>
            /// Pixel data is aligned on 2-byte boundary
            /// </summary>
            Two = 2,

            /// <summary>
            /// Pixel data is aligned on 4-byte boundary
            /// </summary>
            Four = 4,

            /// <summary>
            /// Pixel data is aligned on 8-byte boundary
            /// </summary>
            Eight = 8
        }

        /// <summary>
        /// Compare functions as used by <see cref="gl.StencilFunc"/> and <see cref="gl.DepthFunc"/>.
        /// </summary>
        public enum CompareFunc
        {
            /// <summary>
            /// The function always fails.
            /// </summary>
            Never = (int)DepthFunction.Never,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) &lt; (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is less than the stored depth value.
            /// </summary>
            Less = (int)DepthFunction.Less,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) &lt;= (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is less than or equal to the stored depth value.
            /// </summary>
            LessOrEqual = (int)DepthFunction.Lequal,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) &gt; (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is greater than the stored depth value.
            /// </summary>
            Greater = (int)DepthFunction.Greater,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) &gt;= (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is greater than or equal to the stored depth value.
            /// </summary>
            GreaterOrEqual = (int)DepthFunction.Gequal,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) = (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is equal to the stored depth value.
            /// </summary>
            Equal = (int)DepthFunction.Equal,

            /// <summary>
            /// For stencil operations, the function passes if (Ref and Mask) != (Stencil and Mask).
            /// For depth operations, the function passes if the incoming depth value is not equal to the stored depth value.
            /// </summary>
            NotEqual = (int)DepthFunction.Notequal,

            /// <summary>
            /// The function always passes.
            /// </summary>
            Always = (int)DepthFunction.Always
        }

        /// <summary>
        /// Stencil operations as used by <see cref="gl.StencilOp"/>.
        /// </summary>
        public enum StencilOp
        {
            /// <summary>
            /// Keeps the current value.
            /// </summary>
            Keep = (int)TKStencilOp.Keep,

            /// <summary>
            /// Sets the stencil buffer value to 0.
            /// </summary>
            Zero = (int)TKStencilOp.Zero,

            /// <summary>
            /// Sets the stencil buffer value to ARef, as specified by <see cref="gl.StencilFunc"/>.
            /// </summary>
            Replace = (int)TKStencilOp.Replace,

            /// <summary>
            /// Increments the current stencil buffer value.
            /// Clamps to the maximum representable unsigned value.
            /// </summary>
            Increment = (int)TKStencilOp.Incr,

            /// <summary>
            /// Increments the current stencil buffer value.
            /// Wraps stencil buffer value to zero when incrementing the maximum representable unsigned value.
            /// </summary>
            IncrementAndWrap = (int)TKStencilOp.IncrWrap,

            /// <summary>
            /// Decrements the current stencil buffer value.
            /// Clamps to 0.
            /// </summary>
            Decrement = (int)TKStencilOp.Decr,

            /// <summary>
            /// Decrements the current stencil buffer value.
            /// Wraps stencil buffer value to the maximum representable unsigned value when decrementing a stencil buffer value of zero.
            /// </summary>
            DecrementAndWrap = (int)TKStencilOp.DecrWrap,

            /// <summary>
            /// Bitwise inverts the current stencil buffer value.
            /// </summary>
            Invert = (int)TKStencilOp.Invert
        }

        /// <summary>
        /// Blend functions as used by <see cref="gl.BlendFunc"/> and <see cref="gl.BlendFuncSeparate"/>.
        /// </summary>
        /// <remarks>
        /// In the descriptions of the items, the following terms are used: 
        /// <list type="bullet">
        ///   <item><description>SColor: source color (R, G and B values)</description></item>
        ///   <item><description>SAlpha: source alpha</description></item>
        ///   <item><description>DColor: destination color</description></item>
        ///   <item><description>DAlpha: destination alpha</description></item>
        ///   <item><description>RColor: resulting color</description></item>
        ///   <item><description>RAlpha: resulting alpha</description></item>
        ///   <item><description>CColor: constant color, as specified using <see cref="gl.BlendColor(Color4)"/></description></item>
        ///   <item><description>CAlpha: constant alpha, as specified using <see cref="gl.BlendColor(Color4)"/></description></item>
        /// </list>
        /// </remarks>
        public enum BlendFactor
        {
            /// <summary>
            /// RColor = 0, RAlpha = 0
            /// </summary>
            Zero = (int)BlendingFactorSrc.Zero,

            /// <summary>
            /// RColor = 1, RAlpha = 1
            /// </summary>
            One = (int)BlendingFactorSrc.One,

            /// <summary>
            /// RColor = SColor, RAlpha = SAlpha
            /// </summary>
            SrcColor = (int)BlendingFactorSrc.SrcColor,

            /// <summary>
            /// RColor = 1 - SColor, RAlpha = 1 - SAlpha
            /// </summary>
            OneMinusSrcColor = (int)BlendingFactorSrc.OneMinusSrcColor,

            /// <summary>
            /// RColor = DColor, RAlpha = DAlpha
            /// </summary>
            DstColor = (int)BlendingFactorSrc.DstColor,

            /// <summary>
            /// RColor = 1 - DColor, RAlpha = 1 - DAlpha
            /// </summary>
            OneMinusDstColor = (int)BlendingFactorSrc.OneMinusDstColor,

            /// <summary>
            /// RColor = SAlpha, RAlpha = SAlpha
            /// </summary>
            SrcAlpha = (int)BlendingFactorSrc.SrcAlpha,

            /// <summary>
            /// RColor = 1 - SAlpha, RAlpha = 1 - SAlpha
            /// </summary>
            OneMinusSrcAlpha = (int)BlendingFactorSrc.OneMinusSrcAlpha,

            /// <summary>
            /// RColor = DAlpha, RAlpha = DAlpha
            /// </summary>
            DstAlpha = (int)BlendingFactorSrc.DstAlpha,

            /// <summary>
            /// RColor = 1 - DAlpha, RAlpha = 1 - DAlpha
            /// </summary>
            OneMinusDstAlpha = (int)BlendingFactorSrc.OneMinusDstAlpha,

            /// <summary>
            /// RColor = CColor, RAlpha = CAlpha
            /// </summary>
            ConstantColor = (int)BlendingFactorSrc.ConstantColor,

            /// <summary>
            /// RColor = 1 - CColor, RAlpha = 1 - CAlpha
            /// </summary>
            OneMinusConstantColor = (int)BlendingFactorSrc.OneMinusConstantColor,

            /// <summary>
            /// RColor = CAlpha, RAlpha = CAlpha
            /// </summary>
            ConstantAlpha = (int)BlendingFactorSrc.ConstantAlpha,

            /// <summary>
            /// RColor = 1 - CAlpha, RAlpha = 1 - CAlpha
            /// </summary>
            OneMinusConstantAlpha = (int)BlendingFactorSrc.OneMinusConstantAlpha,

            /// <summary>
            /// RColor = Min(SAlpha, 1 - DAlpha), RAlpha = 1
            /// </summary>
            SrcAlphaSaturate = (int)BlendingFactorSrc.SrcAlphaSaturate
        }

        /// <summary>
        /// Blend equations as used by <see cref="gl.BlendEquation"/> and <see cref="gl.BlendEquationSeparate"/>.
        /// </summary>
        public enum BlendEquationMode
        {
            /// <summary>
            /// Result = (SrcColor * SrcWeigth) + (DstColor * DstWeight)
            /// </summary>
            Add = (int)TKBlendEquationMode.FuncAdd,

            /// <summary>
            /// Result = (SrcColor * SrcWeigth) - (DstColor * DstWeight)
            /// </summary>
            Subtract = (int)TKBlendEquationMode.FuncSubtract,

            /// <summary>
            /// Result = (DstColor * DstWeight) - (SrcColor * SrcWeigth)
            /// </summary>
            ReverseSubtract = (int)TKBlendEquationMode.FuncReverseSubtract
        }
        #endregion

        #region Errors
        /// <summary>
        /// Returns the last error that happened.
        /// </summary>
        /// <remarks>
        /// You usually don't need to call this method yourself since all OpenGL calls are error checked automatically when compiling in DEBUG mode with Assertions enabled.
        ///
        /// <para> <b>OpenGL API</b>: glGetError</para>
        /// </remarks>
        /// <returns>The last error that happened.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error GetError()
        {
#if __ANDROID__ || __IOS__
            return (Error)GL.GetErrorCode();
#else
            return (Error)GL.GetError();
#endif
        }
        #endregion

        #region Coordinate Transformations
        /// <summary>
        /// Set the viewport.
        /// </summary>
        /// <remarks>
        /// When a GL context is first attached to a window, width and height are set to the dimensions of that window.
        /// 
        /// <para>This method specifies the affine transformation of X and Y from normalized device coordinates to window coordinates.</para>
        /// 
        /// <para>Viewport width and height are silently clamped to a range that depends on the implementation.
        /// To query this range, call <see cref="GetMaxViewportDimensions"/>.</para>
        /// 
        /// <para><b>OpenGL API</b>: glViewport</para>
        /// </remarks>
        /// <param name="left">X-coordinate of the lower left corner of the viewport rectangle, in pixels.</param>
        /// <param name="bottom">Y-coordinate of the lower left corner of the viewport rectangle, in pixels.</param>
        /// <param name="width">width of the viewport.</param>
        /// <param name="height">height of the viewport.</param>
        /// <exception cref="GLException">InvalidValue if either <paramref name="width"/> or <paramref name="height"/> is negative.</exception>
        /// <seealso cref="GetViewport"/>
        /// <seealso cref="GetMaxViewportDimensions"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Viewport(int left, int bottom, int width, int height)
        {
            GL.Viewport(left, bottom, width, height);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Set the viewport.
        /// </summary>
        /// <remarks>
        /// When a GL context is first attached to a window, width and height are set to the dimensions of that window.
        /// 
        /// <para>This method specifies the affine transformation of X and Y from normalized device coordinates to window coordinates.</para>
        /// 
        /// <para>Viewport width and height are silently clamped to a range that depends on the implementation.
        /// To query this range, call <see cref="GetMaxViewportDimensions"/>.</para>
        /// 
        /// <para><b>OpenGL API</b>: glViewport</para>
        /// </remarks>
        /// <param name="width">width of the viewport.</param>
        /// <param name="height">height of the viewport.</param>
        /// <exception cref="GLException">InvalidValue if either <paramref name="width"/> or <paramref name="height"/> is negative.</exception>
        /// <seealso cref="GetViewport"/>
        /// <seealso cref="GetMaxViewportDimensions"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Viewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Returns the current viewport.
        /// </summary>
        /// <param name="left">Is set to the left coordinate of the viewport.</param>
        /// <param name="bottom">Is set to the bottom coordinate of the viewport.</param>
        /// <param name="width">Is set to the width of the viewport.</param>
        /// <param name="height">Is set to the height of the viewport.</param>
        /// <remarks>
        /// Initially the <paramref name="left"/> and <paramref name="bottom"/> window coordinates are both set to 0, 
        /// and <paramref name="width"/> and <paramref name="height"/> are set to the width and height of the window into which the GL will do its rendering.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_VIEWPORT)</para>
        /// </remarks>
        /// <seealso cref="Viewport(int, int)"/>
        /// <seealso cref="Viewport(int, int, int, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetViewport(out int left, out int bottom, out int width, out int height)
        {
            int[] data = new int[4];
            GL.GetInteger(GetPName.Viewport, data);
            glUtils.Check("gl");
            left = data[0];
            bottom = data[1];
            width = data[2];
            height = data[3];
        }

        /// <summary>
        /// Returns the maximum supported width and height of the viewport.
        /// </summary>
        /// <param name="maxWidth">Is set to the maximum width of the viewport.</param>
        /// <param name="maxHeight">Is set to the maximum height of the viewport.</param>
        /// <remarks>
        /// These must be at least as large as the visible dimensions of the display being rendered to.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_VIEWPORT_DIMS)</para>
        /// </remarks>
        /// <seealso cref="Viewport(int, int)"/>
        /// <seealso cref="Viewport(int, int, int, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetMaxViewportDimensions(out int maxWidth, out int maxHeight)
        {
            int[] data = new int[2];
            GL.GetInteger(GetPName.MaxViewportDims, data);
            glUtils.Check("gl");
            maxWidth = data[0];
            maxHeight = data[1];
        }

        /// <summary>
        /// Specify mapping of depth values from normalized device coordinates to window coordinates.
        /// </summary>
        /// <remarks>
        /// After clipping and division by W, depth coordinates range from -1 to 1, corresponding to the near and far clipping planes.
        /// This method specifies a linear mapping of the normalized depth coordinates in this range to window depth coordinates.
        /// Regardless of the actual depth buffer implementation, window coordinate depth values are treated as though they range from 0 through 1 (like color components).
        /// Thus, the values accepted by <c>DepthRange</c> are both clamped to this range before they are accepted.
        ///
        /// <para>The setting of (0, 1) maps the near plane to 0 and the far plane to 1.
        /// With this mapping, the depth buffer range is fully utilized.</para>
        ///
        /// <para><b>Note</b>: it is not necessary that <paramref name="nearVal"/> be less than <paramref name="farVal"/>.
        /// Reverse mappings such as <paramref name="nearVal"/> = 1, and <paramref name="farVal"/> = 0 are acceptable.</para>
        ///
        /// <para><b>OpenGL API</b>: glDepthRangef</para>
        /// </remarks>
        /// <param name="nearVal">specifies the mapping of the near clipping plane to window coordinates. The initial value is 0.</param>
        /// <param name="farVal">specifies the mapping of the far clipping plane to window coordinates. The initial value is 1.</param>
        /// <seealso cref="GetDepthRange"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="PolygonOffset"/>
        /// <seealso cref="Viewport(int, int, int, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DepthRange(float nearVal, float farVal)
        {
            GL.DepthRange(nearVal, farVal);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the near and far clipping planes.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_DEPTH_RANGE)
        /// </remarks>
        /// <param name="nearVal">is set to the mapping of the near clipping plane.</param>
        /// <param name="farVal">is set to the mapping of the far clipping plane.</param>
        /// <seealso cref="DepthRange"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetDepthRange(out float nearVal, out float farVal)
        {
            float[] data = new float[2];
            GL.GetFloat(GetPName.DepthRange, data);
            glUtils.Check("gl");
            nearVal = data[0];
            farVal = data[1];
        }
#endregion

#region Whole Framebuffer Operations
        /// <summary>
        /// Specify clear values used by <see cref="gl.Clear"/> to clear the color buffers.
        /// </summary>
        /// <remarks>
        /// All values are clamped to the range 0..1.
        ///
        /// <para><b>OpenGL API</b>: glClearColor</para>
        /// </remarks>
        /// <param name="red">red component.</param>
        /// <param name="green">green component.</param>
        /// <param name="blue">blue component.</param>
        /// <param name="alpha">alpha component.</param>
        /// <seealso cref="Clear"/>
        /// <seealso cref="GetClearColor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            GL.ClearColor(red, green, blue, alpha);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Specify clear values used by <see cref="gl.Clear"/> to clear the color buffers.
        /// </summary>
        /// <remarks>
        /// The R, G, B and A values in AColor are clamped to the range 0..1.
        ///
        /// <para><b>OpenGL API</b>: glClearColor</para>
        /// </remarks>
        /// <param name="color">the clear color.</param>
        /// <seealso cref="Clear"/>
        /// <seealso cref="GetClearColor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearColor(Color4 color)
        {
            GL.ClearColor(color);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Returns the current color used to clear the color buffers.
        /// </summary>
        /// <returns>The clear color</returns>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_COLOR_CLEAR_VALUE)
        /// </remarks>
        /// <seealso cref="ClearColor(float, float, float, float)"/>
        /// <seealso cref="ClearColor(Color4)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color4 GetClearColor()
        {
            float[] data = new float[4];
            GL.GetFloat(GetPName.ColorClearValue, data);
            glUtils.Check("gl");
            return new Color4(data[0], data[1], data[2], data[3]);
        }

        /// <summary>
        /// Enable and disable writing of frame buffer color components.
        /// </summary>
        /// <remarks>
        /// The initial values of the parameters are all True, indicating that the color components can be written.
        ///
        /// <para>This method specifies whether the individual color components in the frame buffer can or cannot be written.
        /// If <paramref name="red"/> is False, for example, no change is made to the red component of any pixel in any of the color buffers, regardless of the drawing operation attempted.</para>
        ///
        /// <para>Changes to individual bits of components cannot be controlled.
        /// Rather, changes are either enabled or disabled for entire color components.</para>
        ///
        /// <para><b>OpenGL API</b>: glColorMask</para>
        /// </remarks>
        /// <param name="red">whether red can or cannot be written into the frame buffer.</param>
        /// <param name="green">whether green can or cannot be written into the frame buffer.</param>
        /// <param name="blue">whether blue can or cannot be written into the frame buffer.</param>
        /// <param name="alpha">whether alpha can or cannot be written into the frame buffer.</param>
        /// <seealso cref="GetColorMask"/>
        /// <seealso cref="Clear"/>
        /// <seealso cref="DepthMask"/>
        /// <seealso cref="StencilMask"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            GL.ColorMask(red, green, blue, alpha);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get whether writing of frame buffer color components is enabled.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_COLOR_WRITEMASK)
        /// </remarks>
        /// <param name="red">is set to True of red can be written. False otherwise.</param>
        /// <param name="green">is set to True of green can be written. False otherwise.</param>
        /// <param name="blue">is set to True of blue can be written. False otherwise.</param>
        /// <param name="alpha">is set to True of alpha can be written. False otherwise.</param>
        /// <seealso cref="ColorMask"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetColorMask(out bool red, out bool green, out bool blue, out bool alpha)
        {
            int[] data = new int[4];
            GL.GetInteger(GetPName.ColorWritemask, data);
            glUtils.Check("gl");
            red = (data[0] == 1);
            green = (data[1] == 1);
            blue = (data[2] == 1);
            alpha = (data[3] == 1);
        }

        /// <summary>
        /// Specify the clear value used by <see cref="gl.Clear"/> to clear the depth buffer.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glClearDepthf/glClearDepth
        /// </remarks>
        /// <param name="depth">the depth value used when the depth buffer is cleared. The initial value is 1. 
        /// The value is clamped to the range 0..1.</param>
        /// <seealso cref="Clear"/>
        /// <seealso cref="GetClearDepth"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDepth(float depth)
        {
            GL.ClearDepth(depth);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current clear depth value.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_DEPTH_CLEAR_VALUE)
        /// </remarks>
        /// <returns>The current clear depth value.</returns>
        /// <seealso cref="ClearDepth"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetClearDepth()
        {
            GL.GetFloat(GetPName.DepthClearValue, out float value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Enable or disable writing into the depth buffer.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glDepthMask
        /// </remarks>
        /// <param name="enable">specifies whether the depth buffer is enabled for writing. 
        /// If False, depth buffer writing is disabled. 
        /// Otherwise, it is enabled. Initially, depth buffer writing is enabled.</param>
        /// <seealso cref="GetDepthMask"/>
        /// <seealso cref="ColorMask"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="DepthRange"/>
        /// <seealso cref="StencilMask"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DepthMask(bool enable)
        {
            GL.DepthMask(enable);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Checks whether writing into the depth buffer is enabled.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_DEPTH_WRITEMASK)
        /// </remarks>
        /// <returns>True if writing into the depth buffer is enabled. False otherwise.</returns>
        /// <seealso cref="DepthMask"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetDepthMask()
        {
            GL.GetInteger(GetPName.DepthWritemask, out int value);
            glUtils.Check("gl");
            return (value == 1);
        }

        /// <summary>
        /// Specify the index value used by <see cref="gl.Clear"/> to clear the stencil buffer.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glClearStencil
        /// </remarks>
        /// <param name="index">the index used when the stencil buffer is cleared. The initial value is 0. 
        /// Only the lowest <see cref="Framebuffer.StencilBits"/> bits of the index are used.</param>
        /// <seealso cref="Clear"/>
        /// <seealso cref="GetClearStencil"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearStencil(int index)
        {
            GL.ClearStencil(index);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Returns the index to which the stencil bitplanes are cleared.
        /// </summary>
        /// <remarks>
        /// The initial value is 0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_CLEAR_VALUE)</para>
        /// </remarks>
        /// <returns>The stencil index</returns>
        /// <seealso cref="ClearStencil"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetClearStencil()
        {
            GL.GetInteger(GetPName.StencilClearValue, out int value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Control the front and back writing of individual bits in the stencil planes.
        /// </summary>
        /// <remarks>
        /// <c>StencilMask</c> controls the writing of individual bits in the stencil planes.
        /// The least significant n bits of AMask, where n is the number of bits in the stencil buffer, specify a mask.
        /// Where a 1 appears in the mask, it's possible to write to the corresponding bit in the stencil buffer.
        /// Where a 0 appears, the corresponding bit is write-protected.
        /// Initially, all bits are enabled for writing.
        ///
        /// <para>There can be two separate mask writemasks; one affects back-facing polygons, 
        /// and the other affects front-facing polygons as well as other non-polygon primitives.
        /// <c>StencilMask</c> sets both front and back stencil writemasks to the same values.
        /// Use <see cref="StencilMaskSeparate"/> to set front and back stencil writemasks to different values.</para>
        ///
        /// <para><b>Note</b>: <c>StencilMask</c> is the same as calling <see cref="gl.StencilMaskSeparate"/> with face set to FrontAndBack.</para>
        ///
        /// <para><b>OpenGL API</b>: glStencilMask</para>
        /// </remarks>
        /// <param name="mask">specifies a bit mask to enable and disable> writing of individual bits in the stencil planes. 
        /// Initially, the mask is all 1's.</param>
        /// <seealso cref="GetStencilWriteMask"/>
        /// <seealso cref="GetStencilBackWriteMask"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="ColorMask"/>
        /// <seealso cref="DepthMask"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilMask(uint mask)
        {
            GL.StencilMask(mask);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Control the front and/or back writing of individual bits in the stencil planes.
        /// </summary>
        /// <remarks>
        /// See <see cref="StencilMask"/> for more details.
        ///
        /// <para><b>OpenGL API</b>: glStencilMaskSeparate</para>
        /// </remarks>
        /// <param name="face">specifies whether front and/or back stencil writemask is updated.</param>
        /// <param name="mask">specifies a bit mask to enable and disable writing of individual bits in the stencil planes. 
        /// Initially, the mask is all 1's.</param>
        /// <seealso cref="GetStencilWriteMask"/>
        /// <seealso cref="GetStencilBackWriteMask"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="ColorMask"/>
        /// <seealso cref="DepthMask"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilMaskSeparate(Face face, uint mask)
        {
#if __ANDROID__ || __IOS__
            GL.StencilMaskSeparate((CullFaceMode)face, mask);
#else
            GL.StencilMaskSeparate((StencilFace)face, mask);
#endif
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get the mask that controls writing of the stencil bitplanes for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is all 1's.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_WRITEMASK)</para>
        /// </remarks>
        /// <returns>The stencil write mask.</returns>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetStencilWriteMask()
        {
            GL.GetInteger(GetPName.StencilWritemask, out int value);
            glUtils.Check("gl");
            return (uint)value;
        }

        /// <summary>
        /// Get the mask that controls writing of the stencil bitplanes for back-facing polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is all 1's.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_WRITEMASK)</para>
        /// </remarks>
        /// <returns>The stencil write mask.</returns>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetStencilBackWriteMask()
        {
            GL.GetInteger(GetPName.StencilBackWritemask, out int value);
            glUtils.Check("gl");
            return (uint)value;
        }

        /// <summary>
        /// <c>Clear</c> buffers to preset values.
        /// </summary>
        /// <remarks>
        /// The value to which each buffer is cleared depends on the setting of the <c>clear</c> value for that buffer, 
        /// as set by <see cref="ClearColor(Color4)"/>, <see cref="ClearStencil"/> and <see cref="ClearDepth"/>.
        ///
        /// <para>The pixel ownership test, the scissor test, dithering, and the buffer writemasks affect the operation of <c>Clear</c>.
        /// The scissor box bounds the cleared region.
        /// Blend function, stenciling, fragment shading, and depth-buffering are ignored by <c>Clear</c>.</para>
        ///
        /// <para><b>OpenGL API</b>: glClear</para>
        /// </remarks>
        /// <param name="buffers">the buffers to be cleared.</param>
        /// <seealso cref="ClearColor(float, float, float, float)"/>
        /// <seealso cref="GetClearColor"/>
        /// <seealso cref="ClearStencil"/>
        /// <seealso cref="GetClearStencil"/>
        /// <seealso cref="ClearDepth(float)"/>
        /// <seealso cref="GetClearDepth"/>
        /// <seealso cref="ColorMask"/>
        /// <seealso cref="DepthMask"/>
        /// <seealso cref="Scissor"/>
        /// <seealso cref="StencilMask"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(ClearBuffers buffers)
        {
            GL.Clear((ClearBufferMask)buffers);
            glUtils.Check("gl");
        }
#endregion

#region Per-Fragment operations
        /// <summary>
        /// Specify pixel arithmetic using blend functions.
        /// </summary>
        /// <remarks>
        /// Pixels can be drawn using a function that blends the incoming (source) RGBA values with the RGBA values that are already in the frame buffer (the destination values).
        /// Blending is initially disabled.
        /// Use <see cref="Enable"/> and <see cref="Disable"/> with argument <see cref="Capability.Blend"/> to enable and disable blending.
        ///
        /// <para><c>BlendFunc</c> defines the operation of blending when it is enabled.
        /// ASrcFactor specifies which method is used to scale the source color components.
        /// ADstFactor specifies which method is used to scale the destination color components.
        /// See <see cref="BlendFactor"/> for a description of the possible operations.</para>
        ///
        /// <para><b>Note</b>: incoming (source) alpha is correctly thought of as a material opacity, ranging from 1.0, 
        /// representing complete opacity, to 0.0, representing complete transparency.</para>
        ///
        /// <para><b>Note</b>: transparency is best implemented using blend function (SrcAlpha, OneMinusSrcAlpha) with primitives sorted from farthest to nearest.
        /// Note that this transparency calculation does not require the presence of alpha bitplanes in the frame buffer.</para>
        ///
        /// <para><b>OpenGL API</b>: glBlendFunc</para>
        /// </remarks>
        /// <param name="srcFactor">specifies how the red, green, blue, and alpha source blending factors are computed. The initial value is One.</param>
        /// <param name="dstFactor">specifies how the red, green, blue, and alpha destination blending factors are computed. The initial value is Zero.</param>
        /// <seealso cref="GetBlendSrcRgb"/>
        /// <seealso cref="GetBlendSrcAlpha"/>
        /// <seealso cref="GetBlendDstRgb"/>
        /// <seealso cref="GetBlendDstAlpha"/>
        /// <seealso cref="BlendColor(Color4)"/>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendEquationSeparate"/>
        /// <seealso cref="Clear"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendFunc(BlendFactor srcFactor, BlendFactor dstFactor)
        {
            GL.BlendFunc((BlendingFactorSrc)srcFactor, (BlendingFactorDest)dstFactor);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Specify pixel arithmetic for RGB and alpha components separately.
        /// </summary>
        /// <remarks>
        /// Pixels can be drawn using a function that blends the incoming (source) RGBA values with the RGBA values that are already in the frame buffer (the destination values).
        /// Blending is initially disabled.
        /// Use <see cref="Enable"/> and <see cref="Disable"/> with argument <see cref="Capability.Blend"/> to enable and disable blending.
        ///
        /// <para><c>BlendFuncSeparate</c> defines the operation of blending when it is enabled.
        /// ASrcRgb specifies which method is used to scale the source RGB-color components.
        /// ADstRGB specifies which method is used to scale the destination RGB-color components.
        /// Likewise, ASrcAlpha specifies which method is used to scale the source alpha color component, 
        /// and ADstAlpha specifies which method is used to scale the destination alpha component.
        /// See <see cref="BlendFactor"/> for a description of the possible operations.</para>
        ///
        /// <para><b>Note</b>: incoming (source) alpha is correctly thought of as a material opacity, ranging from 1.0, 
        /// representing complete opacity, to 0.0, representing complete transparency.</para>
        ///
        /// <para><b>OpenGL API</b>: glBlendFuncSeparate</para>
        /// </remarks>
        /// <param name="srcRgb">specifies how the red, green and blue source blending factors are computed. The initial value is One.</param>
        /// <param name="dstRgb">specifies how the red, green and blue destination blending factors are computed. The initial value is Zero.</param>
        /// <param name="srcAlpha">specifies how the alpha source blending factor is computed. The initial value is One.</param>
        /// <param name="dstAlpha">specifies how the alpha destination blending factor is computed. The initial value is Zero.</param>
        /// <seealso cref="GetBlendSrcRgb"/>
        /// <seealso cref="GetBlendSrcAlpha"/>
        /// <seealso cref="GetBlendDstRgb"/>
        /// <seealso cref="GetBlendDstAlpha"/>
        /// <seealso cref="BlendColor(Color4)"/>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendEquationSeparate"/>
        /// <seealso cref="Clear"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="BlendFunc"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendFuncSeparate(BlendFactor srcRgb, BlendFactor dstRgb, BlendFactor srcAlpha, BlendFactor dstAlpha)
        {
            GL.BlendFuncSeparate((BlendingFactorSrc)srcRgb, (BlendingFactorDest)dstRgb, (BlendingFactorSrc)srcAlpha, (BlendingFactorDest)dstAlpha);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current RGB source blend function.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_SRC_RGB)
        /// </remarks>
        /// <returns>The blend function.</returns>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendFactor GetBlendSrcRgb()
        {
            GL.GetInteger(GetPName.BlendSrcRgb, out int value);
            glUtils.Check("gl");
            return (BlendFactor)value;
        }

        /// <summary>
        /// Gets the current alpha source blend function.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_SRC_ALPHA)
        /// </remarks>
        /// <returns>The blend function.</returns>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendFactor GetBlendSrcAlpha()
        {
            GL.GetInteger(GetPName.BlendSrcAlpha, out int value);
            glUtils.Check("gl");
            return (BlendFactor)value;
        }

        /// <summary>
        /// Gets the current RGB destination blend function.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_DST_RGB)
        /// </remarks>
        /// <returns>The blend function.</returns>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendFactor GetBlendDstRgb()
        {
            GL.GetInteger(GetPName.BlendDstRgb, out int value);
            glUtils.Check("gl");
            return (BlendFactor)value;
        }

        /// <summary>
        /// Gets the current alpha destination blend function.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_DST_ALPHA)
        /// </remarks>
        /// <returns>The blend function.</returns>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendFactor GetBlendDstAlpha()
        {
            GL.GetInteger(GetPName.BlendSrcAlpha, out int value);
            glUtils.Check("gl");
            return (BlendFactor)value;
        }

        /// <summary>
        /// Specify the equation used for both the RGB blend equation and the Alpha blend equation.
        /// </summary>
        /// <remarks>
        /// The blend equations determine how a new pixel (the 'source' color) is combined with a pixel already in the framebuffer (the 'destination' color).
        /// This function sets both the RGB blend equation and the alpha blend equation to a single equation.
        ///
        /// <para>These equations use the source and destination blend factors specified by either <see cref="BlendFunc"/> or <see cref="BlendFuncSeparate"/>.
        /// See <see cref="BlendFunc"/> or <see cref="BlendFuncSeparate"/> for a description of the various blend factors.</para>
        ///
        /// <para>See <see cref="BlendEquationMode"/> for a description of the available options.
        /// The results of these equations are clamped to the range 0..1.</para>
        ///
        /// <para>The Add equation is useful for anti-aliasing and transparency, among other things.</para>
        ///
        /// <para>Initially, both the RGB blend equation and the alpha blend equation are set to Add.</para>
        ///
        /// <para><b>OpenGL API</b>: glBlendEquation</para>
        /// </remarks>
        /// <param name="equation">specifies how source and destination colors are combined.</param>
        /// <seealso cref="GetBlendEquationRgb"/>
        /// <seealso cref="GetBlendEquationAlpha"/>
        /// <seealso cref="BlendColor(Color4)"/>
        /// <seealso cref="BlendEquationSeparate"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendEquation(BlendEquationMode equation)
        {
            GL.BlendEquation((TKBlendEquationMode)equation);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Set the RGB blend equation and the alpha blend equation separately.
        /// </summary>
        /// <remarks>
        /// The blend equations determine how a new pixel (the 'source' color) is combined with a pixel already in the framebuffer (the 'destination' color).
        /// This function specifies one blend equation for the RGB-color components and one blend equation for the alpha component.
        ///
        /// <para>These equations use the source and destination blend factors specified by either <see cref="BlendFunc"/> or <see cref="BlendFuncSeparate"/>.
        /// See <see cref="BlendFunc"/> or <see cref="BlendFuncSeparate"/> for a description of the various blend factors.</para>
        ///
        /// <para>See <see cref="BlendEquationMode"/> for a description of the available options.
        /// The results of these equations are clamped to the range 0..1.</para>
        ///
        /// <para>The Add equation is useful for anti-aliasing and transparency, among other things.</para>
        ///
        /// <para>Initially, both the RGB blend equation and the alpha blend equation are set to Add.</para>
        ///
        /// <para><b>OpenGL API</b>: glBlendEquation</para>
        /// </remarks>
        /// <param name="equationRgb">specifies how the red, green, and blue components of the source and destination colors are combined.</param>
        /// <param name="equationAlpha">specifies how the alpha component of the source and destination colors are combined</param>
        /// <seealso cref="GetBlendEquationRgb"/>
        /// <seealso cref="GetBlendEquationAlpha"/>
        /// <seealso cref="BlendColor(Color4)"/>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="BlendFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendEquationSeparate(BlendEquationMode equationRgb, BlendEquationMode equationAlpha)
        {
            GL.BlendEquationSeparate((TKBlendEquationMode)equationRgb, (TKBlendEquationMode)equationAlpha);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current RGB blend equation.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_EQUATION_RGB)
        /// </remarks>
        /// <returns>The blend equation.</returns>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendEquationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendEquationMode GetBlendEquationRgb()
        {
            GL.GetInteger(GetPName.BlendEquationRgb, out int value);
            glUtils.Check("gl");
            return (BlendEquationMode)value;
        }

        /// <summary>
        /// Gets the current alpha blend equation.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_BLEND_EQUATION_ALPHA)
        /// </remarks>
        /// <returns>The blend equation.</returns>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendEquationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlendEquationMode GetBlendEquationAlpha()
        {
            GL.GetInteger(GetPName.BlendEquationAlpha, out int value);
            glUtils.Check("gl");
            return (BlendEquationMode)value;
        }

        /// <summary>
        /// Set the blend color.
        /// </summary>
        /// <remarks>
        /// The blend color may be used to calculate the source and destination blending factors.
        /// The color components are clamped to the range 0..1 before being stored.
        /// See <see cref="BlendFunc"/> for a complete description of the blending operations.
        /// Initially the blend color is set to (0, 0, 0, 0).
        ///
        /// <para><b>OpenGL API</b>: glBlendColor</para>
        /// </remarks>
        /// <param name="red">red component.</param>
        /// <param name="green">green component.</param>
        /// <param name="blue">blue component.</param>
        /// <param name="alpha">alpha component.</param>
        /// <seealso cref="GetBlendColor"/>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendFunc"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendColor(float red, float green, float blue, float alpha)
        {
            GL.BlendColor(red, green, blue, alpha);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Set the blend color.
        /// </summary>
        /// <remarks>
        /// The blend color may be used to calculate the source and destination blending factors.
        /// The color components are clamped to the range 0..1 before being stored.
        /// See <see cref="BlendFunc"/> for a complete description of the blending operations.
        /// Initially the blend color is set to (0, 0, 0, 0).
        ///
        /// <para><b>OpenGL API</b>: glBlendColor</para>
        /// </remarks>
        /// <param name="color">the blend color.</param>
        /// <seealso cref="GetBlendColor"/>
        /// <seealso cref="BlendEquation"/>
        /// <seealso cref="BlendFunc"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlendColor(Color4 color)
        {
            GL.BlendColor(color);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current blend color.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_BLEND_COLOR)
        /// </remarks>
        /// <returns>The blend color.</returns>
        /// <seealso cref="BlendColor(Color4)"/>
        /// <seealso cref="BlendColor(float, float, float, float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color4 GetBlendColor()
        {
            float[] data = new float[4];
            GL.GetFloat(GetPName.Blend, data);
            glUtils.Check("gl");
            return new Color4(data[0], data[1], data[2], data[3]);
        }

        /// <summary>
        /// Set front and back function and reference value for stencil testing.
        /// </summary>
        /// <remarks>
        /// Stenciling, like depth-buffering, enables and disables drawing on a per-pixel basis.
        /// Stencil planes are first drawn into using GL drawing primitives, then geometry and images are rendered using the stencil planes to mask out portions of the screen.
        /// Stenciling is typically used in multi-pass rendering algorithms to achieve special effects, such as decals, outlining, 
        /// and constructive solid geometry rendering.
        ///
        /// <para>The stencil test conditionally eliminates a pixel based on the outcome of a comparison between the reference value and the value in the stencil buffer.
        /// To enable and disable the test, call <see cref="gl.Enable"/> and <see cref="gl.Disable"/> with argument <see cref="Capability.StencilTest"/>.
        /// To specify actions based on the outcome of the stencil test, call <see cref="gl.StencilOperation"/> or <see cref="gl.StencilOperationSeparate"/>.</para>
        ///
        /// <para>There can be two separate sets of <paramref name="func"/>, <paramref name="reference"/>, and <paramref name="mask"/> parameters; 
        /// one affects back-facing polygons, and the other affects front-facing polygons as well as other non-polygon primitives.
        /// <c>StencilFunc</c> sets both front and back stencil state to the same values.
        /// Use <see cref="StencilFuncSeparate"/> to set front and back stencil state to different values.</para>
        ///
        /// <para><paramref name="func"/> is an enum that determines the stencil comparison function.
        /// It accepts one of eight values (see <see cref="CompareFunc"/>).</para>
        ///
        /// <para><paramref name="reference"/> is an integer reference value that is used in the stencil comparison.
        /// It is clamped to the range [0, 2n - 1], where n is the number of bitplanes in the stencil buffer.</para>
        ///
        /// <para><paramref name="mask"/> is bitwise ANDed with both the reference value and the stored stencil value, 
        /// with the ANDed values participating in the comparison.</para>
        ///
        /// <para><b>Note</b>: initially, the stencil test is disabled.
        /// If there is no stencil buffer, no stencil modification can occur and it is as if the stencil test always passes.</para>
        ///
        /// <para><b>Note</b>: <c>StencilFunc</c> is the same as calling <see cref="StencilFuncSeparate"/> with <c>face</c> set to <see cref="Face.FrontAndBack"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glStencilFunc</para>
        /// </remarks>
        /// <param name="func">the test function. Initial value is <see cref="CompareFunc.Always"/>.</param>
        /// <param name="reference">(optional) value that specifies the reference value for the stencil test. 
        /// <c>reference</c> is clamped to the range [0, 2n - 1], where n is the number of bitplanes in the stencil buffer. 
        /// The initial value is 0.</param>
        /// <param name="mask">(optional) value that specifies a mask that is ANDed with both the reference value and the stored stencil value when the test is done.
        /// The initial value is all 1's.</param>
        /// <seealso cref="GetStencilFunc"/>
        /// <seealso cref="GetStencilValueMask"/>
        /// <seealso cref="GetStencilRef"/>
        /// <seealso cref="GetStencilBackFunc"/>
        /// <seealso cref="GetStencilBackValueMask"/>
        /// <seealso cref="GetStencilBackRef"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilFunc(CompareFunc func, int reference = 0, uint mask = 0xffffffff)
        {
            GL.StencilFunc((StencilFunction)func, reference, mask);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Set front and/or back function and reference value for stencil testing.
        /// </summary>
        /// <remarks>
        /// See <see cref="StencilFunc"/> for more details.
        ///
        /// <para><b>OpenGL API</b>: glStencilFuncSeparate</para>
        /// </remarks>
        /// <param name="face">specifies whether front and/or back stencil state is updated.</param>
        /// <param name="func">the test function. Initial value is <see cref="CompareFunc.Always"/>.</param>
        /// <param name="reference">(optional) value that specifies the reference value for the stencil test. 
        /// <c>reference</c> is clamped to the range [0, 2n - 1], where n is the number of bitplanes in the stencil buffer. 
        /// The initial value is 0.</param>
        /// <param name="mask">(optional) value that specifies a mask that is ANDed with both the reference value and the stored stencil value when the test is done.
        /// The initial value is all 1's.</param>
        /// <seealso cref="GetStencilFunc"/>
        /// <seealso cref="GetStencilValueMask"/>
        /// <seealso cref="GetStencilRef"/>
        /// <seealso cref="GetStencilBackFunc"/>
        /// <seealso cref="GetStencilBackValueMask"/>
        /// <seealso cref="GetStencilBackRef"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilFuncSeparate(Face face, CompareFunc func, int reference = 0, uint mask = 0xffffffff)
        {
#if __ANDROID__ || __IOS__
            GL.StencilFuncSeparate((CullFaceMode)face, (StencilFunction)func, reference, mask);
#else
            GL.StencilFuncSeparate((StencilFace)face, (StencilFunction)func, reference, mask);
#endif
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get what function is used to compare the stencil reference value with the stencil buffer value for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is <see cref="CompareFunc.Always"/>.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_FUNC)</para>
        /// </remarks>
        /// <returns>The compare function</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompareFunc GetStencilFunc()
        {
            GL.GetInteger(GetPName.StencilFunc, out int value);
            glUtils.Check("gl");
            return (CompareFunc)value;
        }

        /// <summary>
        /// Get the mask that is used to mask both the stencil reference value and the stencil buffer value before they are compared for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is all 1's.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_VALUE_MASK)</para>
        /// </remarks>
        /// <returns>The mask value</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetStencilValueMask()
        {
            GL.GetInteger(GetPName.StencilValueMask, out int value);
            glUtils.Check("gl");
            return (uint)value;
        }

        /// <summary>
        /// Get the reference value that is compared with the contents of the stencil buffer for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is 0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_REF)</para>
        /// </remarks>
        /// <returns>The reference value</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetStencilRef()
        {
            GL.GetInteger(GetPName.StencilRef, out int value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get what function is used for back-facing polygons to compare the stencil reference value with the stencil buffer value.
        /// </summary>
        /// <remarks>
        /// The initial value is <see cref="CompareFunc.Always"/>.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_FUNC)</para>
        /// </remarks>
        /// <returns>The compare function</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompareFunc GetStencilBackFunc()
        {
            GL.GetInteger(GetPName.StencilBackFunc, out int value);
            glUtils.Check("gl");
            return (CompareFunc)value;
        }

        /// <summary>
        /// Get the mask that is used for back-facing polygons to mask both the stencil reference value and the stencil buffer value before they are compared.
        /// </summary>
        /// <remarks>
        /// The initial value is all 1's.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_VALUE_MASK)</para>
        /// </remarks>
        /// <returns>The mask</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetStencilBackValueMask()
        {
            GL.GetInteger(GetPName.StencilBackValueMask, out int value);
            glUtils.Check("gl");
            return (uint)value;
        }

        /// <summary>
        /// Get the reference value that is compared with the contents of the stencil buffer for back-facing polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is 0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_REF)</para>
        /// </remarks>
        /// <returns>The reference value</returns>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetStencilBackRef()
        {
            GL.GetInteger(GetPName.StencilBackRef, out int value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Set front and back stencil test actions.
        /// </summary>
        /// <remarks>
        /// Stenciling, like depth-buffering, enables and disables drawing on a per-pixel basis.
        /// You draw into the stencil planes using GL drawing primitives, then render geometry and images, 
        /// using the stencil planes to mask out portions of the screen.
        /// Stenciling is typically used in multi-pass rendering algorithms to achieve special effects, 
        /// such as decals, outlining, and constructive solid geometry rendering.
        ///
        /// <para>The stencil test conditionally eliminates a pixel based on the outcome of a comparison between the value in the stencil buffer and a reference value.
        /// To enable and disable the test, call <see cref="gl.Enable"/> and <see cref="gl.Disable"/> with argument <see cref="Capability.StencilTest"/>; 
        /// to control it, call <see cref="gl.StencilFunc"/> or <see cref="gl.StencilFuncSeparate"/>.</para>
        ///
        /// <para>There can be two separate sets of <paramref name="stencilFail"/>, <paramref name="depthFail"/> and <paramref name="bothPass"/> parameters; 
        /// one affects back-facing polygons, and the other affects front-facing polygons as well as other non-polygon primitives.
        /// <c>StencilOperation</c> sets both front and back stencil state to the same values.
        /// Use <see cref="gl.StencilOperationSeparate"/> to set front and back stencil state to different values.</para>
        ///
        /// <para><c>StencilOperation</c> takes three arguments that indicate what happens to the stored stencil value while stenciling is enabled.
        /// If the stencil test fails, no change is made to the pixel's color or depth buffers, and <paramref name="stencilFail"/> specifies what happens to the stencil buffer contents.</para>
        ///
        /// <para>Stencil buffer values are treated as unsigned integers.
        /// When incremented and decremented, values are clamped to 0 and 2n - 1, where n is the value returned by <see cref="Framebuffer.StencilBits"/>.</para>
        ///
        /// <para>The other two arguments to <c>StencilOperation</c> specify stencil buffer actions that depend on whether subsequent depth buffer tests 
        /// succeed (<paramref name="bothPass"/>) or fail (<paramref name="depthFail"/>) (see <see cref="DepthFunc"/>).
        /// Note that <paramref name="depthFail"/> is ignored when there is no depth buffer, or when the depth buffer is not enabled.
        /// In these cases, <paramref name="stencilFail"/> and <paramref name="bothPass"/> specify stencil action when the stencil test fails and passes, respectively.</para>
        ///
        /// <para><b>Note</b>: initially the stencil test is disabled.
        /// If there is no stencil buffer, no stencil modification can occur and it is as if the stencil tests always pass, 
        /// regardless of any call to <c>StencilOperation</c>.</para>
        ///
        /// <para><b>Note</b>: <c>StencilOperation</c> is the same as calling <see cref="gl.StencilOperationSeparate"/> 
        /// with <c>face</c> set to <see cref="Face.FrontAndBack"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glStencilOp</para>
        /// </remarks>
        /// <param name="stencilFail">(optional) value that specifies the action to take when the stencil test fails. The initial value is Keep.</param>
        /// <param name="depthFail">(optional) value that specifies the action to take when the stencil test passes, but the depth test fails. The initial value is Keep.</param>
        /// <param name="bothPass">(optional) value that specifies the action to take when both the stencil test and depth test pass, 
        /// or when the stencil test passes and either there is no depth buffer or depth testing is not enabled. The initial value is Keep.</param>
        /// <seealso cref="GetStencilFail"/>
        /// <seealso cref="GetStencilPassDepthPass"/>
        /// <seealso cref="GetStencilPassDepthFail"/>
        /// <seealso cref="GetStencilBackFail"/>
        /// <seealso cref="GetStencilBackPassDepthPass"/>
        /// <seealso cref="GetStencilBackPassDepthFail"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilOperation(StencilOp stencilFail = StencilOp.Keep, StencilOp depthFail = StencilOp.Keep, StencilOp bothPass = StencilOp.Keep)
        {
            GL.StencilOp((TKStencilOp)stencilFail, (TKStencilOp)depthFail, (TKStencilOp)bothPass);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Set front and/or back stencil test actions.
        /// </summary>
        /// <remarks>
        /// See <see cref="StencilOperation"/> for more details.
        ///
        /// <para><b>OpenGL API</b>: glStencilOpSeparate</para>
        /// </remarks>
        /// <param name="face">specifies whether front and/or back stencil state is updated.</param>
        /// <param name="stencilFail">(optional) value that specifies the action to take when the stencil test fails. The initial value is Keep.</param>
        /// <param name="depthFail">(optional) value that specifies the action to take when the stencil test passes, but the depth test fails. The initial value is Keep.</param>
        /// <param name="bothPass">(optional) value that specifies the action to take when both the stencil test and depth test pass, 
        /// or when the stencil test passes and either there is no depth buffer or depth testing is not enabled. The initial value is Keep.</param>
        /// <seealso cref="GetStencilFail"/>
        /// <seealso cref="GetStencilPassDepthPass"/>
        /// <seealso cref="GetStencilPassDepthFail"/>
        /// <seealso cref="GetStencilBackFail"/>
        /// <seealso cref="GetStencilBackPassDepthPass"/>
        /// <seealso cref="GetStencilBackPassDepthFail"/>
        /// <seealso cref="Framebuffer.StencilBits"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilFuncSeparate"/>
        /// <seealso cref="StencilMask"/>
        /// <seealso cref="StencilMaskSeparate"/>
        /// <seealso cref="StencilOperation"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StencilOperationSeparate(Face face, StencilOp stencilFail = StencilOp.Keep, StencilOp depthFail = StencilOp.Keep, StencilOp bothPass = StencilOp.Keep)
        {
#if __ANDROID__ || __IOS__
            GL.StencilOpSeparate((CullFaceMode)face, (TKStencilOp)stencilFail, (TKStencilOp)depthFail, (TKStencilOp)bothPass);
#else
            GL.StencilOpSeparate((StencilFace)face, (TKStencilOp)stencilFail, (TKStencilOp)depthFail, (TKStencilOp)bothPass);
#endif
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get what action is taken when the stencil test fails for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_FAIL)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilFail()
        {
            GL.GetInteger(GetPName.StencilFail, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Get what action is taken when the stencil test passes and the depth test passes for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_PASS_DEPTH_PASS)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilPassDepthPass()
        {
            GL.GetInteger(GetPName.StencilPassDepthPass, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Get what action is taken when the stencil test passes, but the depth test fails for front-facing polygons and non-polygons.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_PASS_DEPTH_FAIL)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilPassDepthFail()
        {
            GL.GetInteger(GetPName.StencilPassDepthFail, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Get what action is taken for back-facing polygons when the stencil test fails.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_FAIL)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilBackFail()
        {
            GL.GetInteger(GetPName.StencilBackFail, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Get what action is taken for back-facing polygons when the stencil test passes and the depth test passes.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_PASS_DEPTH_PASS)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilBackPassDepthPass()
        {
            GL.GetInteger(GetPName.StencilBackPassDepthPass, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Get what action is taken for back-facing polygons when the stencil test passes, but the depth test fails.
        /// </summary>
        /// <remarks>
        /// The initial value is Keep.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BACK_PASS_DEPTH_FAIL)</para>
        /// </remarks>
        /// <returns>The stencil operation</returns>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="StencilOperationSeparate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StencilOp GetStencilBackPassDepthFail()
        {
            GL.GetInteger(GetPName.StencilBackPassDepthFail, out int value);
            glUtils.Check("gl");
            return (StencilOp)value;
        }

        /// <summary>
        /// Specify the value used for depth buffer comparisons.
        /// </summary>
        /// <remarks>
        /// This method specifies the function used to compare each incoming pixel depth value with the depth value present in the depth buffer.
        /// The comparison is performed only if depth testing is enabled.
        /// (See <see cref="gl.Enable"/> and <see cref="gl.Disable"/> of <see cref="Capability.DepthTest"/>.)
        ///
        /// <para>Initially, depth testing is disabled.
        /// If depth testing is disabled or no depth buffer exists, it is as if the depth test always passes.</para>
        ///
        /// <para><b>Note</b>: even if the depth buffer exists and the depth mask is non-zero, 
        /// the depth buffer is not updated if the depth test is disabled.</para>
        ///
        /// <para><b>OpenGL API</b>: glDepthFunc</para>
        /// </remarks>
        /// <param name="func">specifies the depth comparison function. The initial value is <see cref="CompareFunc.Less"/>.</param>
        /// <seealso cref="GetDepthFunc"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="DepthRange"/>
        /// <seealso cref="PolygonOffset"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DepthFunc(CompareFunc func)
        {
            GL.DepthFunc((DepthFunction)func);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current depth function.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_DEPTH_FUNC)
        /// </remarks>
        /// <returns>The current depth function.</returns>
        /// <seealso cref="DepthFunc"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompareFunc GetDepthFunc()
        {
            GL.GetInteger(GetPName.DepthFunc, out int value);
            glUtils.Check("gl");
            return (CompareFunc)value;
        }

        /// <summary>
        /// Define the scissor box.
        /// </summary>
        /// <remarks>
        /// When a GL" context is first attached to a window, the width and height are set to the dimensions of that window.
        ///
        /// <para>This method defines a rectangle, called the scissor box, in window coordinates.
        /// The first two arguments, <paramref name="left"/> and <paramref name="bottom"/>, specify the lower left corner of the box.
        /// <paramref name="width"/> and <paramref name="height"/> specify the width and height of the box.</para>
        ///
        /// <para>To enable and disable the scissor test, call <see cref="Enable"/> and <see cref="Disable"/> with <see cref="Capability.ScissorTest"/>.
        /// The test is initially disabled.
        /// While the test is enabled, only pixels that lie within the scissor box can be modified by drawing commands.
        /// Window coordinates have integer values at the shared corners of frame buffer pixels.
        /// <c>Scissor(0, 0, 1, 1)</c> allows modification of only the lower left pixel in the window, 
        /// and <c>Scissor(0, 0, 0, 0)</c> doesn't allow modification of any pixels in the window.</para>
        ///
        /// <para>When the scissor test is disabled, it is as though the scissor box includes the entire window.</para>
        ///
        /// <para><b>OpenGL API</b>: glScissor</para>
        /// </remarks>
        /// <param name="left">X-coordinate of the lower left corner of the scissor box. The initial value is 0.</param>
        /// <param name="bottom">Y-coordinate of the lower left corner of the scissor box. The initial value is 0.</param>
        /// <param name="width">width of the scissor box.</param>
        /// <param name="height">height of the scissor box.</param>
        /// <exception cref="GLException">InvalidValue if either <paramref name="width"/> or <paramref name="height"/> is negative.</exception>
        /// <seealso cref="GetScissor"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Viewport(int, int, int, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scissor(int left, int bottom, int width, int height)
        {
            GL.Scissor(left, bottom, width, height);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get the current scissor box.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_SCISSOR_BOX)
        /// </remarks>
        /// <param name="left">is set to the X-coordinate of the lower left corner of the scissor box.</param>
        /// <param name="bottom">is set to the Y-coordinate of the lower left corner of the scissor box.</param>
        /// <param name="width">is set to the width of the scissor box.</param>
        /// <param name="height">is set to the height of the scissor box.</param>
        /// <seealso cref="Scissor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetScissor(out int left, out int bottom, out int width, out int height)
        {
            int[] data = new int[4];
            GL.GetInteger(GetPName.ScissorBox, data);
            glUtils.Check("gl");
            left = data[0];
            bottom = data[1];
            width = data[2];
            height = data[3];
        }

        /// <summary>
        /// Specify multisample coverage parameters.
        /// </summary>
        /// <remarks>
        /// Multisampling samples a pixel multiple times at various implementation-dependent subpixel locations to generate antialiasing effects.
        /// Multisampling transparently antialiases points, lines, and polygons if it is enabled.
        ///
        /// <para><paramref name="value"/> is used in constructing a temporary mask used in determining which samples will be used in resolving the final fragment color.
        /// This mask is bitwise-anded with the coverage mask generated from the multisampling computation.
        /// If the <paramref name="invert"/> flag is set, the temporary mask is inverted (all bits flipped) and then the bitwise-and is computed.</para>
        ///
        /// <para>If an implementation does not have any multisample buffers available, or multisampling is disabled, 
        /// rasterization occurs with only a single sample computing a pixel's final RGB color.</para>
        ///
        /// <para>Provided an implementation supports multisample buffers, and multisampling is enabled, 
        /// then a pixel's final color is generated by combining several samples per pixel.
        /// Each sample contains color, depth, and stencil information, allowing those operations to be performed on each sample.</para>
        ///
        /// <para><b>OpenGL API</b>: glSampleCoverage</para>
        /// </remarks>
        /// <param name="value">sample coverage value. The value is clamped to the range 0..1. The initial value is 1.0.</param>
        /// <param name="invert">(optional) value representing if the coverage masks should be inverted. Defaults to False.</param>
        /// <seealso cref="GetSampleCoverageValue"/>
        /// <seealso cref="GetSampleCoverageInvert"/>
        /// <seealso cref="GetSampleAlphaToCoverage"/>
        /// <seealso cref="GetSampleCoverage"/>
        /// <seealso cref="Framebuffer.SampleBuffers"/>
        /// <seealso cref="Framebuffer.Samples"/>
        /// <seealso cref="Enable"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SampleCoverage(float value, bool invert = false)
        {
            GL.SampleCoverage(value, invert);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get the current sample coverage value.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_SAMPLE_COVERAGE_VALUE)
        /// </remarks>
        /// <returns>The current sample coverage value.</returns>
        /// <seealso cref="SampleCoverage"/>
        /// <seealso cref="GetSampleCoverageInvert"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSampleCoverageValue()
        {
            GL.GetFloat(GetPName.SampleCoverageValue, out float value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get the current sample coverage invert flag.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_SAMPLE_COVERAGE_INVERT)
        /// </remarks>
        /// <returns>The current sample coverage invert flag.</returns>
        /// <seealso cref="SampleCoverage"/>
        /// <seealso cref="GetSampleCoverageValue"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetSampleCoverageInvert()
        {
            GL.GetInteger(GetPName.SampleCoverageInvert, out int value);
            glUtils.Check("gl");
            return (value == 1);
        }

        /// <summary>
        /// Get a boolean value indicating if the fragment coverage value should be ANDed with a temporary coverage value based on the current sample coverage value.
        /// </summary>
        /// <remarks>
        /// The initial value is False.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_SAMPLE_COVERAGE)</para>
        /// </remarks>
        /// <returns>The coverage flag</returns>
        /// <seealso cref="SampleCoverage"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetSampleCoverage()
        {
            GL.GetInteger(GetPName.SampleCoverage, out int value);
            glUtils.Check("gl");
            return (value == 1);
        }
        
        /// <summary>
        /// Get a boolean value indicating if the fragment coverage value should be ANDed with a temporary coverage value based on the fragment's alpha value.
        /// </summary>
        /// <remarks>
        /// The initial value is False.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_SAMPLE_ALPHA_TO_COVERAGE)</para>
        /// </remarks>
        /// <returns>The coverage flag</returns>
        /// <seealso cref="SampleCoverage"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetSampleAlphaToCoverage()
        {
            GL.GetInteger(GetPName.SampleAlphaToCoverage, out int value);
            glUtils.Check("gl");
            return (value == 1);
        }
#endregion

#region Vertex Arrays
        /// <summary>
        /// Render primitives from array data, <b>without</b> using indices.
        /// </summary>
        /// <remarks>
        /// DrawArrays specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL procedure to pass each individual vertex attribute, 
        /// you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertices, normals, 
        /// and colors and use them to construct a sequence of primitives with a single call to DrawArrays.
        ///
        /// <para>When DrawArrays is called, it uses <paramref name="count"/> sequential elements from each enabled array 
        /// to construct a sequence of geometric primitives, beginning with the first element.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct those primitives.</para>
        ///
        /// <para>To enable and disable generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para><b>OpenGL API</b>: glDrawArrays</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="count">the number of elements to be rendered.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="count"/> is negative.</exception>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawElements(PrimitiveType, ushort[], int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawArrays(PrimitiveType type, int count)
        {
            GL.DrawArrays((TKPrimkitiveType)type, 0, count);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, <b>without</b> using indices.
        /// </summary>
        /// <remarks>
        /// DrawArrays specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL procedure to pass each individual vertex attribute, 
        /// you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertices, normals, 
        /// and colors and use them to construct a sequence of primitives with a single call to DrawArrays.
        ///
        /// <para>When DrawArrays is called, it uses <paramref name="count"/> sequential elements from each enabled array 
        /// to construct a sequence of geometric primitives, beginning with element <paramref name="first"/>.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct those primitives.</para>
        ///
        /// <para>To enable and disable generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para><b>OpenGL API</b>: glDrawArrays</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="first">(optional) the starting index in the enabled arrays.</param>
        /// <param name="count">the number of elements to be rendered.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="count"/> is negative.</exception>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawElements(PrimitiveType, ushort[], int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawArrays(PrimitiveType type, int first, int count)
        {
            GL.DrawArrays((TKPrimkitiveType)type, first, count);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, using supplied indices.
        /// </summary>
        /// <remarks>
        /// DrawElements specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL function to pass each vertex attribute, you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertex attributes 
        /// and use them to construct a sequence of primitives with a single call to DrawElements.
        ///
        /// <para>When DrawElements is called, it uses <paramref name="indices"/> to locate vertices in the vertex array.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct these primitives.
        /// If more than one array is enabled, each is used.</para>
        ///
        /// <para>To enable and disable a generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para> <b>OpenGL API</b>: glDrawElements</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="indices">array of indices (of type byte).</param>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawArrays(PrimitiveType, int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawElements(PrimitiveType type, byte[] indices)
        {
            Debug.Assert(indices != null);
            GL.DrawElements((TKPrimkitiveType)type, indices.Length, DrawElementsType.UnsignedByte, indices);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, using supplied indices.
        /// </summary>
        /// <remarks>
        /// DrawElements specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL function to pass each vertex attribute, you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertex attributes 
        /// and use them to construct a sequence of primitives with a single call to DrawElements.
        ///
        /// <para>When DrawElements is called, it uses <paramref name="indices"/> to locate vertices in the vertex array.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct these primitives.
        /// If more than one array is enabled, each is used.</para>
        ///
        /// <para>To enable and disable a generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para> <b>OpenGL API</b>: glDrawElements</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="indices">array of indices (of type byte).</param>
        /// <param name="first">index to the first index in <paramref name="indices"/> to use.</param>
        /// <param name="count">number of indices in <paramref name="indices"/> to use, starting at <paramref name="count"/>.</param>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawArrays(PrimitiveType, int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void DrawElements(PrimitiveType type, byte[] indices, int first, int count)
        {
            Debug.Assert(indices != null);
            if (count == 0)
                count = indices.Length - first;

            fixed (byte* idx = &indices[first])
            {
                GL.DrawElements((TKPrimkitiveType)type, count, DrawElementsType.UnsignedByte, (IntPtr)idx);
            }

            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, using supplied indices.
        /// </summary>
        /// <remarks>
        /// DrawElements specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL function to pass each vertex attribute, you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertex attributes 
        /// and use them to construct a sequence of primitives with a single call to DrawElements.
        ///
        /// <para>When DrawElements is called, it uses <paramref name="indices"/> to locate vertices in the vertex array.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct these primitives.
        /// If more than one array is enabled, each is used.</para>
        ///
        /// <para>To enable and disable a generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para> <b>OpenGL API</b>: glDrawElements</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="indices">array of indices (of type ushort).</param>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawArrays(PrimitiveType, int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawElements(PrimitiveType type, ushort[] indices)
        {
            Debug.Assert(indices != null);
            GL.DrawElements((TKPrimkitiveType)type, indices.Length, DrawElementsType.UnsignedShort, indices);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, using supplied indices.
        /// </summary>
        /// <remarks>
        /// DrawElements specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL function to pass each vertex attribute, you can use <see cref="VertexAttribute"/> to prespecify separate arrays of vertex attributes 
        /// and use them to construct a sequence of primitives with a single call to DrawElements.
        ///
        /// <para>When DrawElements is called, it uses <paramref name="indices"/> to locate vertices in the vertex array.
        /// <paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct these primitives.
        /// If more than one array is enabled, each is used.</para>
        ///
        /// <para>To enable and disable a generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para> <b>OpenGL API</b>: glDrawElements</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="indices">array of indices (of type ushort).</param>
        /// <param name="first">index to the first index in <paramref name="indices"/> to use.</param>
        /// <param name="count">number of indices in <paramref name="indices"/> to use, starting at <paramref name="count"/>.</param>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawArrays(PrimitiveType, int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void DrawElements(PrimitiveType type, ushort[] indices, int first, int count)
        {
            Debug.Assert(indices != null);
            if (count == 0)
                count = indices.Length - first;

            fixed (ushort* idx = &indices[first])
            {
                GL.DrawElements((TKPrimkitiveType)type, count, DrawElementsType.UnsignedShort, (IntPtr)idx);
            }

            glUtils.Check("gl");
        }

        /// <summary>
        /// Render primitives from array data, using indices from a bound index buffer.
        /// </summary>
        /// <remarks>
        /// DrawElements specifies multiple geometric primitives with very few subroutine calls.
        /// Instead of calling a GL function to pass each vertex attribute, you can use <see cref="VertexAttribute"/> to prespecify 
        /// separate arrays of vertex attributes and use them to construct a sequence of primitives with a single call to DrawElements.
        ///
        /// <para><paramref name="type"/> specifies what kind of primitives are constructed and how the array elements construct these primitives.
        /// If more than one array is enabled, each is used.</para>
        ///
        /// <para>To enable and disable a generic vertex attribute array, call <see cref="VertexAttribute.Enable"/> and <see cref="VertexAttribute.Disable"/>.</para>
        ///
        /// <para><b>Note</b>: if the current program object, as set by <see cref="Program.Use"/>, is invalid, rendering results are undefined.
        /// However, no error is generated for this case.</para>
        ///
        /// <para> <b>OpenGL API</b>: glDrawElements</para>
        /// </remarks>
        /// <param name="type">specifies what kind of primitives to render.</param>
        /// <param name="count">the number of indices used to render.</param>
        /// <param name="indexType">the type of the indices in the bound index buffer. Only 8-bit (UnsignedByte) and 16-bit (UnsignedShort) indices are supported.</param>
        /// <exception cref="GLException">InvalidFramebufferOperation if the currently bound framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="Framebuffer.Status"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="VertexAttribute.Enable"/>
        /// <seealso cref="VertexAttribute.Disable"/>
        /// <seealso cref="DrawArrays(PrimitiveType, int, int)"/>
        /// <seealso cref="Program.Use"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawElements(PrimitiveType type, int count, IndexType indexType)
        {
            GL.DrawElements((TKPrimkitiveType)type, count, (DrawElementsType)indexType, IntPtr.Zero);
            glUtils.Check("gl");
        }
#endregion

#region Rasterization
        /// <summary>
        /// Specify whether front- or back-facing polygons can be culled.
        /// </summary>
        /// <remarks>
        /// This method specifies whether front- or back-facing polygons are culled (as specified by <paramref name="mode"/>) when polygon culling is enabled.
        /// Polygon culling is initially disabled.
        /// To enable and disable polygon culling, call the <see cref="gl.Enable"/> and <see cref="gl.Disable"/> methods with the argument <see cref="Capability.CullFace"/>.
        ///
        /// <para><see cref="gl.FrontFace"/> specifies which of the clockwise and counterclockwise polygons are front-facing and back-facing.</para>
        ///
        /// <para><b>Note</b>: if mode is <see cref="Face.FrontAndBack"/>, no polygons are drawn, 
        /// but other primitives such as points and lines are drawn.</para>
        ///
        /// <para><b>OpenGL API</b>: glCullFace</para>
        /// </remarks>
        /// <param name="mode">whether front- or back-facing polygons are candidates for culling.</param>
        /// <seealso cref="GetCullFace"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="FrontFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CullFace(Face mode)
        {
            GL.CullFace((CullFaceMode)mode);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current cull face mode.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_CULL_FACE_MODE)
        /// </remarks>
        /// <returns>The current cull face mode.</returns>
        /// <seealso cref="CullFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Face GetCullFace()
        {
            GL.GetInteger(GetPName.CullFaceMode, out int value);
            glUtils.Check("gl");
            return (Face)value;
        }

        /// <summary>
        /// Define front- and back-facing polygons.
        /// </summary>
        /// <remarks>
        /// In a scene composed entirely of opaque closed surfaces, back-facing polygons are never visible.
        /// Eliminating these invisible polygons has the obvious benefit of speeding up the rendering of the image.
        /// To enable and disable elimination of back-facing polygons, call <see cref="gl.Enable"/> and <see cref="gl.Disable"/> with argument <see cref="Capability.CullFace"/>.
        ///
        /// <para>The projection of a polygon to window coordinates is said to have clockwise winding if an imaginary object following the path from its first vertex, 
        /// its second vertex, and so on, to its last vertex, and finally back to its first vertex, 
        /// moves in a clockwise direction about the interior of the polygon.
        /// The polygon's winding is said to be counterclockwise if the imaginary object following 
        /// the same path moves in a counterclockwise direction about the interior of the polygon.
        /// glFrontFace specifies whether polygons with clockwise winding in window coordinates, 
        /// or counterclockwise winding in window coordinates, are taken to be front-facing.
        /// Passing <see cref="FaceOrientation.CounterClockwise"/> selects counterclockwise polygons as front-facing; 
        /// <see cref="FaceOrientation.Clockwise"/> selects clockwise polygons as front-facing.
        /// By default, counterclockwise polygons are taken to be front-facing.</para>
        ///
        /// <para><b>OpenGL API</b>: glFrontFace</para>
        /// </remarks>
        /// <param name="orientation">the orientation of front-facing polygons. Initial value is <see cref="FaceOrientation.CounterClockwise"/>.</param>
        /// <seealso cref="GetFrontFace"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="CullFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FrontFace(FaceOrientation orientation)
        {
            GL.FrontFace((FrontFaceDirection)orientation);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current front face orientation.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_FRONT_FACE)
        /// </remarks>
        /// <returns>The current front face orientation.</returns>
        /// <seealso cref="FrontFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FaceOrientation GetFrontFace()
        {
            GL.GetInteger(GetPName.FrontFace, out int value);
            glUtils.Check("gl");
            return (FaceOrientation)value;
        }

        /// <summary>
        /// Specify the width of rasterized lines.
        /// </summary>
        /// <remarks>
        /// The actual width is determined by rounding the supplied width to the nearest integer.
        /// (If the rounding results in the value 0, it is as if the line width were 1.)
        ///
        /// <para>There is a range of supported line widths.
        /// Only width 1 is guaranteed to be supported; others depend on the implementation.
        /// To query the range of supported widths, call <see cref="GetAliasedLineWidthRange"/>.</para>
        ///
        /// <para><b>Note</b>: the line width specified by <c>LineWidth</c> is always returned when <see cref="GetLineWidth"/> is queried.
        /// Clamping and rounding have no effect on the specified value.</para>
        ///
        /// <para><b>OpenGL API</b>: glLineWidth</para>
        /// </remarks>
        /// <param name="width">the width of rasterized lines. The initial value is 1.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="width"/> is less than or equal to 0.</exception>
        /// <seealso cref="GetLineWidth"/>
        /// <seealso cref="GetAliasedLineWidthRange"/>
        /// <seealso cref="Enable"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LineWidth(float width)
        {
            GL.LineWidth(width);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Get the current line width.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_LINE_WIDTH)
        /// </remarks>
        /// <returns>The current line width.</returns>
        /// <seealso cref="LineWidth"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLineWidth()
        {
            GL.GetFloat(GetPName.LineWidth, out float value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Set the scale and units used to calculate polygon depth values.
        /// </summary>
        /// <remarks>
        /// When <see cref="Capability.PolygonOffsetFill"/> is enabled, each fragment's depth value will be offset after it is 
        /// interpolated from the depth values of the appropriate vertices.
        /// The value of the offset is (<paramref name="factor"/> × DZ) + (r × <paramref name="units"/>), 
        /// where DZ is a measurement of the change in depth relative to the screen area of the polygon, 
        /// and r is the smallest value that is guaranteed to produce a resolvable offset for a given implementation.
        /// The offset is added before the depth test is performed and before the value is written into the depth buffer.
        ///
        /// <para><c>PolygonOffset</c> is useful for rendering hidden-line images, for applying decals to surfaces, 
        /// and for rendering solids with highlighted edges.</para>
        ///
        /// <para><b>OpenGL API</b>: glPolygonOffset</para>
        /// </remarks>
        /// <param name="factor">a scale factor that is used to create a variable depth offset for each polygon. The initial value is 0.</param>
        /// <param name="units">is multiplied by an implementation-specific value to create a constant depth offset. The initial value is 0.</param>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="GetPolygonOffsetFactor"/>
        /// <seealso cref="GetPolygonOffsetUnits"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolygonOffset(float factor, float units)
        {
            GL.PolygonOffset(factor, units);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Gets the current polygon offset factor.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_POLYGON_OFFSET_FACTOR)
        /// </remarks>
        /// <returns>The polygon offset factor.</returns>
        /// <seealso cref="PolygonOffset"/>
        /// <seealso cref="GetPolygonOffsetUnits"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPolygonOffsetFactor()
        {
            GL.GetFloat(GetPName.PolygonOffsetFactor, out float value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Gets the current polygon offset units.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetFloatv(GL_POLYGON_OFFSET_UNITS)
        /// </remarks>
        /// <returns>The polygon offset units.</returns>
        /// <seealso cref="PolygonOffset"/>
        /// <seealso cref="GetPolygonOffsetFactor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPolygonOffsetUnits()
        {
            GL.GetFloat(GetPName.PolygonOffsetUnits, out float value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Set pixel storage alignment.
        /// </summary>
        /// <remarks>
        /// This method sets pixel storage modes that affect the operation of subsequent <see cref="Framebuffer.ReadPixels"/> 
        /// as well as the unpacking of texture patterns (see <see cref="Texture.Upload{T}(PixelFormat, int, int, T[], int, PixelDataType, int)"/> 
        /// and <see cref="Texture.SubUpload{T}(PixelFormat, int, int, int, int, T[], int, PixelDataType, int)"/>).
        ///
        /// <para>The PackAlignment mode affects how pixel data is downloaded from the GPU into client memory.
        /// The UnpackAlignment mode affects how pixel data is uploaded from client memory to the GPU.</para>
        ///
        /// <para><b>OpenGL API</b>: glPixelStorei</para>
        /// </remarks>
        /// <param name="mode">the mode to set.</param>
        /// <param name="value">the alignment value to set for the mode.</param>
        /// <seealso cref="Framebuffer.ReadPixels"/>
        /// <seealso cref="Texture.Upload{T}(PixelFormat, int, int, T[], int, PixelDataType, int)"/>
        /// <seealso cref="Texture.SubUpload{T}(PixelFormat, int, int, int, int, T[], int, PixelDataType, int)"/>
        /// <seealso cref="GetPixelStore"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PixelStore(PixelStoreMode mode, PixelStoreValue value)
        {
            GL.PixelStore((PixelStoreParameter)mode, (int)value);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Returns to current pixel storage alignment.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetIntegerv(GL_PACK_ALIGNMENT/GL_UNPACK_ALIGNMENT)
        /// </remarks>
        /// <param name="mode">the mode for which to return the storage value.</param>
        /// <returns>The storage alignment for the given node.</returns>
        /// <seealso cref="PixelStore"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelStoreValue GetPixelStore(PixelStoreMode mode)
        {
            GL.GetInteger((GetPName)mode, out int value);
            glUtils.Check("gl");
            return (PixelStoreValue)value;
        }

        /// <summary>
        /// Gets the smallest and largest supported widths for aliased lines.
        /// </summary>
        /// <remarks>
        /// The returned range always includes value 1.0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_ALIASED_LINE_WIDTH_RANGE)</para>
        /// </remarks>
        /// <param name="min">is set to the smallest supported line width.</param>
        /// <param name="max">is set to the largest supported line width.</param>
        /// <seealso cref="LineWidth"/>
        /// <seealso cref="GetLineWidth"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetAliasedLineWidthRange(out float min, out float max)
        {
            float[] values = new float[2];
            GL.GetFloat(GetPName.AliasedLineWidthRange, values);
            glUtils.Check("gl");
            min = values[0];
            max = values[1];
        }

        /// <summary>
        /// Gets the smallest and largest supported sizes for aliased points.
        /// </summary>
        /// <remarks>
        /// The returned range always includes value 1.0.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_ALIASED_POINT_SIZE_RANGE)</para>
        /// </remarks>
        /// <param name="min">is set to the smallest supported point size.</param>
        /// <param name="max">is set to the largest supported point size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetAliasedPointSizeRange(out float min, out float max)
        {
            float[] values = new float[2];
            GL.GetFloat(GetPName.AliasedPointSizeRange, values);
            glUtils.Check("gl");
            min = values[0];
            max = values[1];
        }
#endregion

#region State
        /// <summary>
        /// Enable a server-side GL capability.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IsEnabled"/> to determine the current setting of any capability.
        /// The initial value for each capability with the exception of <see cref="Capability.Dither"/> is False.
        /// The initial value for <see cref="Capability.Dither"/> is True.
        ///
        /// <para><b>OpenGL API</b>: glEnable</para>
        /// </remarks>
        /// <param name="capability">the GL capability to enable.</param>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="Texture.BindToTextureUnit"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="CullFace"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="DepthRange"/>
        /// <seealso cref="LineWidth"/>
        /// <seealso cref="PolygonOffset"/>
        /// <seealso cref="Scissor"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="Texture"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Enable(Capability capability)
        {
            GL.Enable((EnableCap)capability);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Disable a server-side GL capability.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IsEnabled"/> to determine the current setting of any capability.
        /// The initial value for each capability with the exception of <see cref="Capability.Dither"/> is False.
        /// The initial value for <see cref="Capability.Dither"/> is True.
        ///
        /// <para><b>OpenGL API</b>: glDisable</para>
        /// </remarks>
        /// <param name="capability">the GL capability to disable.</param>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="Texture.BindToTextureUnit"/>
        /// <seealso cref="BlendFunc"/>
        /// <seealso cref="CullFace"/>
        /// <seealso cref="DepthFunc"/>
        /// <seealso cref="DepthRange"/>
        /// <seealso cref="LineWidth"/>
        /// <seealso cref="PolygonOffset"/>
        /// <seealso cref="Scissor"/>
        /// <seealso cref="StencilFunc"/>
        /// <seealso cref="StencilOperation"/>
        /// <seealso cref="Texture"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disable(Capability capability)
        {
            GL.Disable((EnableCap)capability);
            glUtils.Check("gl");
        }

        /// <summary>
        /// Checks if a server-side GL capability is enabled.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glIsEnabled
        /// </remarks>
        /// <param name="capability">the GL capability to check.</param>
        /// <returns>True if <paramref name="capability"/> is currently enabled. False otherwise.</returns>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(Capability capability)
        {
            bool result = GL.IsEnabled((EnableCap)capability);
            glUtils.Check("gl");
            return result;
        }
#endregion

#region Special Functions
        /// <summary>
        /// Block until all GL execution is complete.
        /// </summary>
        /// <remarks>
        /// This method does not return until the effects of all previously called GL commands are complete.
        /// Such effects include all changes to GLstate, all changes to connection state, and all changes to the frame buffer contents.
        ///
        /// <para><b>Note</b>: <c>Finish</c> requires a round trip to the server.</para>
        ///
        /// <para><b>OpenGL API</b>: glFinish</para>
        /// </remarks>
        /// <seealso cref="Flush"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Finish()
        {
            GL.Finish();
            glUtils.Check("gl");
        }

        /// <summary>
        /// Force execution of GL commands in finite time.
        /// </summary>
        /// <remarks>
        /// Different GL implementations buffer commands in several different locations, 
        /// including network buffers and the graphics accelerator itself.
        /// This method empties all of these buffers, causing all issued commands to be executed as quickly 
        /// as they are accepted by the actual rendering engine.
        /// Though this execution may not be completed in any particular time period, it does complete in finite time.
        ///
        /// <para>Because any GL program might be executed over a network, or on an accelerator that buffers commands, 
        /// all programs should call <c>Flush</c> whenever they count on having all of their previously issued commands completed.
        /// For example, call <c>Flush</c> before waiting for user input that depends on the generated image.</para>
        ///
        /// <para><b>Note</b>: <c>Flush</c> can return at any time.
        /// It does not wait until the execution of all previously issued GL commands is complete.</para>
        ///
        /// <para><b>OpenGL API</b>: glFlush</para>
        /// </remarks>
        /// <seealso cref="Finish"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Flush()
        {
            GL.Finish();
            glUtils.Check("gl");
        }
#endregion

#region Information Functions
        /// <summary>
        /// Get an estimate of the number of bits of subpixel resolution that are used to position rasterized geometry in window coordinates.
        /// </summary>
        /// <remarks>
        /// The value must be at least 4.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_SUBPIXEL_BITS)</para>
        /// </remarks>
        /// <returns>The number of subpixel bits</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSubpixelBits()
        {
            GL.GetInteger(GetPName.SubpixelBits, out int value);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get the company responsible for this GL implementation.
        /// </summary>
        /// <remarks>
        /// This name does not change from release to release.
        ///
        /// <para>Because the GL does not include queries for the performance characteristics of an implementation, 
        /// some applications are written to recognize known platforms and modify their GL usage based on known 
        /// performance characteristics of these platforms.
        /// <c>GetVendor</c> and <see cref="GetRenderer"/> together uniquely specify a platform.
        /// They do not change from release to release and should be used by platform-recognition algorithms.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetString(GL_VENDOR)</para>
        /// </remarks>
        /// <returns>The vendor</returns>
        /// <seealso cref="GetRenderer"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetVendor()
        {
            string value = GL.GetString(StringName.Vendor);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get the name of the renderer.
        /// </summary>
        /// <remarks>
        /// This name is typically specific to a particular configuration of a hardware platform.
        /// It does not change from release to release.
        ///
        /// <para>Because the GL does not include queries for the performance characteristics of an implementation, 
        /// some applications are written to recognize known platforms and modify their GL usage based on known 
        /// performance characteristics of these platforms.
        /// <see cref="GetVendor"/> and <c>GetRenderer</c> together uniquely specify a platform.
        /// They do not change from release to release and should be used by platform-recognition algorithms.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetString(GL_RENDERER)</para>
        /// </remarks>
        /// <returns>The renderer</returns>
        /// <seealso cref="GetVendor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRenderer()
        {
            string value = GL.GetString(StringName.Renderer);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get a version or release number of the form OpenGL[space]ES[space][version number][space][vendor-specific information].
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetString(GL_VERSION)
        /// </remarks>
        /// <returns>The GL version</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetVersion()
        {
            string value = GL.GetString(StringName.Version);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get a version or release number for the shading language of the form OpenGL[space]ES[space]GLSL[space]ES[space][version number][space][vendor-specific information].
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetString(GL_SHADING_LANGUAGE_VERSION)
        /// </remarks>
        /// <returns>The GLSL version</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetShadingLanguageVersion()
        {
            string value = GL.GetString(StringName.ShadingLanguageVersion);
            glUtils.Check("gl");
            return value;
        }

        /// <summary>
        /// Get a space-separated list of supported extensions to GL.
        /// </summary>
        /// <remarks>
        /// Some applications want to make use of features that are not part of the standard GL.
        /// These features may be implemented as extensions to the standard GL.
        /// This method returns a space-separated list of supported GL extensions.
        /// (Extension names never contain a space character.)
        ///
        /// <para><b>OpenGL API</b>: glGetString(GL_EXTENSIONS)</para>
        /// </remarks>
        /// <returns>The supported extensions</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetExtensions()
        {
            string value = GL.GetString(StringName.Extensions);
            glUtils.Check("gl");
            return value;
        }
#endregion
    }
}