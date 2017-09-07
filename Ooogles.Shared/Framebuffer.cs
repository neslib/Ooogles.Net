using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.ES20;

#if __ANDROID__ || __IOS__
using TKAttachment = OpenTK.Graphics.ES20.FramebufferSlot;
#else
using TKAttachment = OpenTK.Graphics.ES20.All;
#endif

namespace Ooogles
{
    /// <summary>
    /// A framebuffer
    /// </summary>
    public class Framebuffer : GLObject
    {
        /// <summary>
        /// Completeness status of a <see cref="Framebuffer"/>.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// The framebuffer is complete
            /// </summary>
            Complete = (int)All.FramebufferComplete,

            /// <summary>
            /// Not all framebuffer attachment points are framebuffer attachment complete.
            /// This means that at least one attachment point with a renderbuffer or texture attached has its attached object no longer in existence 
            /// or has an attached image with a width or height of zero, or the color attachment point has a non-color-renderable image attached, 
            /// or the depth attachment point has a non-depth-renderable image attached, or the stencil attachment point has a non-stencil-renderable image attached.
            /// </summary>
            IncompleteAttachment = (int)All.FramebufferIncompleteAttachment,
            
            /// <summary>
            /// Not all attached images have the same width and height.
            /// </summary>
            IncompleteDimensions = (int)All.FramebufferIncompleteDimensions,

            /// <summary>
            /// No images are attached to the framebuffer.
            /// </summary>
            MissingAttachment = (int)All.FramebufferIncompleteMissingAttachment,

            /// <summary>
            /// The combination of internal formats of the attached images violates an implementation-dependent set of restrictions.
            /// </summary>
            Unsupported = (int)All.FramebufferUnsupported
        }

        /// <summary>
        /// Attachment points of a <see cref="Framebuffer"/>
        /// </summary>
        public enum Attachment
        {
            /// <summary>
            /// Attachment point for a color buffer
            /// </summary>
            Color = (int)All.ColorAttachment0,

            /// <summary>
            /// Attachment point for a depth buffer
            /// </summary>
            Depth = (int)All.DepthAttachment,

            /// <summary>
            /// Attachment point for a stencil buffer
            /// </summary>
            Stencil = (int)All.StencilAttachment
        }

        /// <summary>
        /// Types of objects attached to a <see cref="Framebuffer"/>
        /// </summary>
        public enum AttachmentType
        {
            /// <summary>
            /// No renderbuffer or texture is attached.
            /// </summary>
            None = (int)All.None,

            /// <summary>
            /// The attachment is a renderbuffer
            /// </summary>
            Renderbuffer = (int)All.Renderbuffer,
            
            /// <summary>
            /// The attachment is a texture
            /// </summary>
            Texture = (int)All.Texture
        }

        /// <summary>
        /// Returns the current framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: do <b>not</b> dispose this framebuffer.
        ///
        /// <para><b>Note</b>: if you access Current <b>before</b> you create any custom framebuffers, 
        /// then the returned value can be regarded as the default framebuffer.
        /// Note that this is not always the 'system' framebuffer, since you are not allowed to access that framebuffer on some devices.</para>
        /// </remarks>
        public static Framebuffer Current
        {
            get
            {
                // NOTE: The default framebuffer is not always 0. For example, on iOS, you
                // cannot write to framebuffer 0, and you must always create a framebuffer and
                // attach it to a view. So the default framebuffer for our purposes, would be
                // the one attached to that view.
                GL.GetInteger(GetPName.FramebufferBinding, out int value);
                glUtils.Check("Framebuffer");
                return new Framebuffer(value);
            }
        }

        /// <summary>
        /// Checks if this framebuffer is currently bound.
        /// </summary>
        /// <value>True if this is the currently bound framebuffer, False otherwise.</value>
        public bool IsBound
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(glUtils.GetTargetBinding(All.Framebuffer), out int currentlyBoundBuffer);
                glUtils.Check(this);
                return (currentlyBoundBuffer == Handle);
            }
        }

        /// <summary>
        /// Return the framebuffer completeness status.
        /// </summary>
        /// <remarks>
        /// The returned value identifies whether or not the currently bound framebuffer is framebuffer complete, 
        /// and if not, which of the rules of framebuffer completeness is violated.
        ///
        /// <para>If the currently bound framebuffer is not framebuffer complete, 
        /// then it is an error to attempt to use the framebuffer for writing or reading.
        /// This means that rendering commands (<see cref="gl.Clear"/>, <see cref="gl.DrawArrays(gl.PrimitiveType, int)"/>, 
        /// and <see cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>) as well as commands that read the framebuffer (<see cref="ReadPixels"/>, 
        /// <see cref="Texture.Copy"/>, and <see cref="Texture.SubCopy"/>) will generate the error <see cref="ErrorCode.InvalidFramebufferOperation"/>
        /// if called while the framebuffer is not framebuffer complete.</para>
        ///
        /// <para><b>Note</b>: it is strongly advised, thought not required, that an application call <c>Status</c> to see if the framebuffer is complete prior to rendering.
        /// This is because some implementations may not support rendering to particular combinations of internal formats.</para>
        ///
        /// <para><b>Note</b>: the default window-system-provided framebuffer is always framebuffer complete, 
        /// and thus <see cref="Status.Complete"/> is returned when there is no bound application created framebuffer.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glCheckFramebufferStatus</para>
        /// </remarks>
        /// <returns>The completeness <c>status</c>.</returns>
        /// <seealso cref="Renderbuffer.Bind"/>
        /// <seealso cref="Texture.Copy"/>
        /// <seealso cref="Texture.SubCopy"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="ReadPixels"/>
        /// <seealso cref="Renderbuffer.Storage"/>
        public Status CompletenessStatus
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                glUtils.Check(this);
                return (Status)status;
            }
        }

        /// <summary>
        /// Get the number of red bitplanes in the color buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_RED_BITS)</para>
        /// </remarks>
        /// <value>The number of red bitplanes.</value>
        /// <seealso cref="GreenBits"/>
        /// <seealso cref="BlueBits"/>
        /// <seealso cref="AlphaBits"/>
        /// <seealso cref="DepthBits"/>
        /// <seealso cref="StencilBits"/>
        public int RedBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.RedBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of green bitplanes in the color buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_GREEN_BITS)</para>
        /// </remarks>
        /// <value>The number of green bitplanes.</value>
        /// <seealso cref="RedBits"/>
        /// <seealso cref="BlueBits"/>
        /// <seealso cref="AlphaBits"/>
        /// <seealso cref="DepthBits"/>
        /// <seealso cref="StencilBits"/>
        public int GreenBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.GreenBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of blue bitplanes in the color buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_BLUE_BITS)</para>
        /// </remarks>
        /// <value>The number of blue bitplanes.</value>
        /// <seealso cref="GreenBits"/>
        /// <seealso cref="RedBits"/>
        /// <seealso cref="AlphaBits"/>
        /// <seealso cref="DepthBits"/>
        /// <seealso cref="StencilBits"/>
        public int BlueBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.BlueBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of alpha bitplanes in the color buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_ALPHA_BITS)</para>
        /// </remarks>
        /// <value>The number of alpha bitplanes.</value>
        /// <seealso cref="GreenBits"/>
        /// <seealso cref="BlueBits"/>
        /// <seealso cref="RedBits"/>
        /// <seealso cref="DepthBits"/>
        /// <seealso cref="StencilBits"/>
        public int AlphaBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.AlphaBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of bitplanes in the depth buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_DEPTH_BITS)</para>
        /// </remarks>
        /// <value>The number of bitplanes in the depth buffer.</value>
        /// <seealso cref="GreenBits"/>
        /// <seealso cref="BlueBits"/>
        /// <seealso cref="AlphaBits"/>
        /// <seealso cref="RedBits"/>
        /// <seealso cref="StencilBits"/>
        public int DepthBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.DepthBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of bitplanes in the stencil buffer of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_STENCIL_BITS)</para>
        /// </remarks>
        /// <value>The number of bitplanes in the stencil buffer.</value>
        /// <seealso cref="GreenBits"/>
        /// <seealso cref="BlueBits"/>
        /// <seealso cref="AlphaBits"/>
        /// <seealso cref="RedBits"/>
        /// <seealso cref="RedBits"/>
        public int StencilBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.StencilBits, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of sample buffers associated with the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_SAMPLE_BUFFERS)</para>
        /// </remarks>
        /// <seealso cref="gl.SampleCoverage"/>
        public int SampleBuffers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.SampleBuffers, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the coverage mask size of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_SAMPLES)</para>
        /// </remarks>
        /// <seealso cref="gl.SampleCoverage"/>
        public int Samples
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.Samples, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the format chosen by the implementation in which pixels may be read from the color buffer of the 
        /// currently bound framebuffer in conjunction with <see cref="ColorReadType"/>.
        /// </summary>
        /// <remarks>
        /// In addition to this implementation-dependent format/type pair, 
        /// format RGBA in conjunction with type UnsingedByte is always allowed by every implementation, 
        /// regardless of the currently bound render surface.
        ///
        /// <para><b>Note</b>: this is a global value that affects all framebuffers.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_IMPLEMENTATION_COLOR_READ_FORMAT)</para>
        /// </remarks>
        /// <seealso cref="ReadPixels"/>
        /// <seealso cref="ColorReadType"/>
        public gl.PixelFormat ColorReadFormat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.ImplementationColorReadFormat, out int value);
                glUtils.Check(this);
                return (gl.PixelFormat)value;
            }
        }

        /// <summary>
        /// Get the type chosen by the implementation with which pixels may be read from the color buffer of the 
        /// currently bound framebuffer in conjunction with <see cref="ColorReadFormat"/>.
        /// </summary>
        /// <remarks>
        /// In addition to this implementation-dependent format/type pair, 
        /// format RGBA in conjunction with type UnsingedByte is always allowed by every implementation, 
        /// regardless of the currently bound render surface.
        ///
        /// <para><b>Note</b>: this is a global value that affects all framebuffers.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_IMPLEMENTATION_COLOR_READ_TYPE)</para>
        /// </remarks>
        /// <seealso cref="ReadPixels"/>
        /// <seealso cref="ColorReadFormat"/>
        public gl.PixelDataType ColorReadType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding(All.Framebuffer, Handle, this);
                GL.GetInteger(GetPName.ImplementationColorReadType, out int value);
                glUtils.Check(this);
                return (gl.PixelDataType)value;
            }
        }

        internal Framebuffer(int handle) => Handle = handle;

        /// <summary>
        /// Creates a framebuffer.
        /// </summary>
        /// <remarks>
        /// No framebuffer objects are associated with the returned framebuffer object names until they are first bound by calling <see cref="Bind"/>.
        ///
        /// <para><b>OpenGL API</b>: glGenFramebuffers</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        public Framebuffer()
        {
            GL.GenFramebuffers(1, out _handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Binds the framebuffer.
        /// </summary>
        /// <remarks>
        /// <c>Bind</c> lets you create or use a named framebuffer object.
        /// When a framebuffer object is bound, the previous binding is automatically broken.
        ///
        /// <para>Framebuffer object names are unsigned integers.
        /// The value zero is reserved to represent the 'system' framebuffer provided by the windowing system.
        /// Framebuffer object names and the corresponding framebuffer object contents are local to the shared object space of the current GL rendering context.</para>
        ///
        /// <para>You may use <see cref="Framebuffer()"/> to generate a new framebuffer object name.</para>
        ///
        /// <para>The state of a framebuffer object immediately after it is first bound is three attachment points 
        /// (ColorAttachment, DepthAttachment, and StencilAttachment) each with None as the object type.</para>
        ///
        /// <para>While a framebuffer object name is bound, all rendering to the framebuffer (with <see cref="gl.DrawArrays(gl.PrimitiveType, int)"/> 
        /// and <see cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>) and reading from the framebuffer (with <see cref="ReadPixels"/>, 
        /// <see cref="Texture.Copy"/>, or <see cref="Texture.SubCopy"/>) use the images attached to the application-created 
        /// framebuffer object rather than the default window-system-provided framebuffer.</para>
        ///
        /// <para>Application created framebuffer objects differ from the default window-system-provided framebuffer in a few important ways.
        /// First, they have modifiable attachment points for a color buffer, a depth buffer, 
        /// and a stencil buffer to which framebuffer attachable images may be attached and detached.
        /// Second, the size and format of the attached images are controlled entirely within the GL and are not affected by window-system events, 
        /// such as pixel format selection, window resizes, and display mode changes.
        /// Third, when rendering to or reading from an application created framebuffer object, 
        /// the pixel ownership test always succeeds (i.e. they own all their pixels).
        /// Fourth, there are no visible color buffer bitplanes, only a single 'off-screen' color image attachment, 
        /// so there is no sense of front and back buffers or swapping.
        /// Finally, there is no multisample buffer.</para>
        ///
        /// <para>A framebuffer object binding created with <c>Bind</c> remains active until a different framebuffer object name is bound, 
        /// or until the bound framebuffer object is deleted.</para>
        ///
        /// <para><b>Note</b>: you cannot unbind a framebuffer.
        /// If you want to revert back to the default framebuffer, then you should call <see cref="Framebuffer.Current"/> <b>before</b> you generate a new framebuffer.
        /// Then you can <c>bind</c> to the that framebuffer later to revert back to it.</para>
        ///
        /// <para><b>OpenGL API</b>: glBindFramebuffer</para>
        /// </remarks>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="IsBound"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Attach a renderbuffer to this framebuffer.
        /// </summary>
        /// <remarks>
        /// This method attaches the renderbuffer specified by <paramref name="renderbuffer"/> as one of the logical buffers of the currently bound framebuffer object.
        /// <paramref name="attachment"/> specifies whether the renderbuffer should be attached to the framebuffer object's color, depth, or stencil buffer.
        /// A renderbuffer may not be attached to the default framebuffer object name 0.
        ///
        /// <para><b>Note</b>: if a renderbuffer object is deleted while its image is attached to the currently bound framebuffer, 
        /// then it is as if <see cref="DetachRenderbuffer"/> had been called for the attachment point to which this image was attached in the currently bound framebuffer object.
        /// In other words, the renderbuffer image is detached from the currently bound framebuffer.
        /// Note that the renderbuffer image is specifically not detached from any non-bound framebuffers.
        /// Detaching the image from any non-bound framebuffers is the responsibility of the application.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glFramebufferRenderbuffer</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to which <paramref name="renderbuffer"/> should be attached.</param>
        /// <param name="renderbuffer">the renderbuffer to attach.</param>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <seealso cref="DetachRenderbuffer"/>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Renderbuffer.Bind"/>
        /// <seealso cref="CompletenessStatus"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="Renderbuffer.Storage"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AttachRenderbuffer(Attachment attachment, Renderbuffer renderbuffer)
        {
            Debug.Assert(renderbuffer != null);
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, (TKAttachment)attachment, RenderbufferTarget.Renderbuffer, renderbuffer.Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Detach a renderbuffer from this framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glFramebufferRenderbuffer</para>
        /// </remarks>
        /// <param name="attachment">the attachment point from which the renderbuffer should be detached.</param>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <seealso cref="AttachRenderbuffer"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DetachRenderbuffer(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, (TKAttachment)attachment, RenderbufferTarget.Renderbuffer, 0);
            glUtils.Check(this);
        }

        /// <summary>
        /// Attach a texture to this framebuffer.
        /// </summary>
        /// <remarks>
        /// This method attaches the texture image specified by <paramref name="texture"/> as one of the logical buffers of the currently bound framebuffer object.
        /// <paramref name="attachment"/> specifies whether the texture image should be attached to the framebuffer object's color, depth, or stencil buffer.
        /// A texture image may not be attached to the default framebuffer object name 0.
        ///
        /// <para><b>Note</b>: special precautions need to be taken to avoid attaching a texture image to the currently bound framebuffer 
        /// while the texture object is currently bound and potentially sampled by the current vertex or fragment shader.
        /// Doing so could lead to the creation of a &quot;feedback loop&quot; between the writing of pixels by rendering operations 
        /// and the simultaneous reading of those same pixels when used as texels in the currently bound texture.
        /// In this scenario, the framebuffer will be considered framebuffer complete, but the values of fragments rendered while in this state will be undefined.
        /// The values of texture samples may be undefined as well.</para>
        ///
        /// <para><b>Note</b>: if a texture object is deleted while its image is attached to the currently bound framebuffer, 
        /// then it is as if <see cref="DetachTexture"/> had been called for the attachment point to which this image was attached in the currently bound framebuffer object.
        /// In other words, the texture image is detached from the currently bound framebuffer.
        /// Note that the texture image is specifically not detached from any non-bound framebuffers.
        /// Detaching the image from any non-bound framebuffers is the responsibility of the application.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glFramebufferTexture2D</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to which <paramref name="texture"/> should be attached.</param>
        /// <param name="texture">the texture to attach.</param>
        /// <param name="cubeTarget">(optional) if this <paramref name="texture"/> is a cube texture, 
        /// then this parameter specifies which of the 6 cube faces to attach. This parameter is ignored for 2D textures.</param>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <seealso cref="DetachTexture"/>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Texture.Bind"/>
        /// <seealso cref="CompletenessStatus"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="Texture.GenerateMipmap"/>
        /// <seealso cref="Texture.Upload{T}(gl.PixelFormat, int, int, T[], int, gl.PixelDataType, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AttachTexture(Attachment attachment, Texture texture, int cubeTarget = 0)
        {
            Debug.Assert(texture != null);
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
#if __ANDROID__ || __IOS__
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (TKAttachment)attachment, texture.GetTarget(cubeTarget), texture.Handle, 0);
#else
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (TKAttachment)attachment, (TextureTarget2d)texture.GetTarget(cubeTarget), texture.Handle, 0);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Detach a texture from this framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glFramebufferTexture2D</para>
        /// </remarks>
        /// <param name="attachment">the attachment point from which the texture should be detached.</param>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <seealso cref="AttachTexture"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DetachTexture(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
#if __ANDROID__ || __IOS__
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (TKAttachment)attachment, TextureTarget.Texture2D, 0, 0);
#else
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (TKAttachment)attachment, (TextureTarget2d)TextureTarget.Texture2D, 0, 0);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Returns the type of object attached to an attachment point of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetFramebufferAttachmentParameteriv(GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE)</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to check.</param>
        /// <returns>The type of attachment attached to this point.</returns>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="GetAttachedTexture"/>
        /// <seealso cref="GetAttachedRenderbuffer"/>
        /// <seealso cref="GetAttachedTextureLevel"/>
        /// <seealso cref="GetAttachedCubeMapFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AttachmentType GetAttachedObjectType(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
            GL.GetFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, (TKAttachment)attachment, FramebufferParameterName.FramebufferAttachmentObjectType, out int value);
            glUtils.Check(this);
            return (AttachmentType)value;
        }

        /// <summary>
        /// Returns the texture attached to an attachment point of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetFramebufferAttachmentParameteriv(GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME)</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to check.</param>
        /// <returns>The texture attached to this point.</returns>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <exception cref="GLException">InvalidOperation if no texture is attached to the given attachment point.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="GetAttachedObjectType"/>
        /// <seealso cref="GetAttachedRenderbuffer"/>
        /// <seealso cref="GetAttachedTextureLevel"/>
        /// <seealso cref="GetAttachedCubeMapFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Texture GetAttachedTexture(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);

#if DEBUG
            if (GetAttachedObjectType(attachment) != AttachmentType.Texture)
                throw new GLException(gl.Error.InvalidOperation, "Framebuffer.GetAttachedTexture");
#endif
            GL.GetFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, (TKAttachment)attachment, FramebufferParameterName.FramebufferAttachmentObjectName, out int value);
            glUtils.Check(this);

            if (value == 0)
                return null;
            return new Texture(value, Texture.Type.TwoD);
        }

        /// <summary>
        /// Returns the mipmap level of the texture attached to an attachment point of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetFramebufferAttachmentParameteriv(GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL)</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to check.</param>
        /// <returns>The mipmap level of texture attached to this point.</returns>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <exception cref="GLException">InvalidOperation if no texture is attached to the given attachment point.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="GetAttachedObjectType"/>
        /// <seealso cref="GetAttachedRenderbuffer"/>
        /// <seealso cref="GetAttachedTexture"/>
        /// <seealso cref="GetAttachedCubeMapFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAttachedTextureLevel(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);

#if DEBUG
            if (GetAttachedObjectType(attachment) != AttachmentType.Texture)
                throw new GLException(gl.Error.InvalidOperation, "Framebuffer.GetAttachedTextureLevel");
#endif

            GL.GetFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, (TKAttachment)attachment, FramebufferParameterName.FramebufferAttachmentTextureLevel, out int value);
            glUtils.Check(this);

            return value;
        }

        /// <summary>
        /// Returns the cube map face of the cube-map texture attached to an attachment point of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetFramebufferAttachmentParameteriv(GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE)</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to check.</param>
        /// <returns>The cube map face of texture attached to this point.</returns>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <exception cref="GLException">InvalidOperation if no texture is attached to the given attachment point.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="GetAttachedObjectType"/>
        /// <seealso cref="GetAttachedRenderbuffer"/>
        /// <seealso cref="GetAttachedTexture"/>
        /// <seealso cref="GetAttachedTextureLevel"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAttachedCubeMapFace(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);

#if DEBUG
            if (GetAttachedObjectType(attachment) != AttachmentType.Texture)
                throw new GLException(gl.Error.InvalidOperation, "Framebuffer.GetAttachedTextureCubeMapFace");
#endif

            GL.GetFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, (TKAttachment)attachment, FramebufferParameterName.FramebufferAttachmentTextureCubeMapFace, out int value);
            glUtils.Check(this);

            return value;
        }

        /// <summary>
        /// Returns the renderbuffer attached to an attachment point of the framebuffer.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetFramebufferAttachmentParameteriv(GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME)</para>
        /// </remarks>
        /// <param name="attachment">the attachment point to check.</param>
        /// <returns>The renderbuffer attached to this point.</returns>
        /// <exception cref="GLException">InvalidOperation if no framebuffer is bound.</exception>
        /// <exception cref="GLException">InvalidOperation if no renderbuffer is attached to the given attachment point.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="AttachRenderbuffer"/>
        /// <seealso cref="AttachTexture"/>
        /// <seealso cref="GetAttachedObjectType"/>
        /// <seealso cref="GetAttachedTexture"/>
        /// <seealso cref="GetAttachedTextureLevel"/>
        /// <seealso cref="GetAttachedCubeMapFace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Renderbuffer GetAttachedRenderbuffer(Attachment attachment)
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);

#if DEBUG
            if (GetAttachedObjectType(attachment) != AttachmentType.Renderbuffer)
                throw new GLException(gl.Error.InvalidOperation, "Framebuffer.GetAttachedRenderbuffer");
#endif

            GL.GetFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, (TKAttachment)attachment, FramebufferParameterName.FramebufferAttachmentObjectName, out int value);
            glUtils.Check(this);

            if (value == 0)
                return null;
            return new Renderbuffer(value);
        }

        /// <summary>
        /// Read a block of pixels from the frame buffer.
        /// </summary>
        /// <remarks>
        /// This method returns pixel data from the frame buffer, starting with the pixel whose lower left corner is at 
        /// location (<paramref name="left"/>, <paramref name="bottom"/>), into client memory starting at location <paramref name="data"/>.
        /// The <see cref="gl.PixelStoreMode.PackAlignment"/>, set with the <see cref="gl.PixelStore"/> command, 
        /// affects the processing of the pixel data before it is placed into client memory.
        ///
        /// <para>Pixels are returned in row order from the lowest to the highest row, left to right in each row.</para>
        ///
        /// <para><paramref name="format"/> specifies the format for the returned pixel values.
        /// RGBA color components are read from the color buffer.
        /// Each color component is converted to floating point such that zero intensity maps to 0.0 and full intensity maps to 1.0.</para>
        ///
        /// <para>Unneeded data is then discarded.
        /// For example, <see cref="gl.PixelFormat.Alpha"/> discards the red, green, and blue components, 
        /// while <see cref="gl.PixelFormat.Rgb"/> discards only the alpha component.
        /// The final values are clamped to the range 0..1.</para>
        ///
        /// <para>Finally, the components are converted to the proper format, as specified by <paramref name="dataType"/>.
        /// When type is <see cref="gl.PixelDataType.UnsignedByte"/> (the default), each component is multiplied by 255.
        /// When <paramref name="dataType"/> is <see cref="gl.PixelDataType.UnsignedShort565"/>, <see cref="gl.PixelDataType.UnsignedShort4444"/>
        /// or <see cref="gl.PixelDataType.UnsignedShort5551"/>, each component is multiplied accordingly.</para>
        ///
        /// <para><b>Note</b>: if the currently bound framebuffer is not the default framebuffer object, 
        /// color components are read from the color image attached to the <see cref="Attachment.Color"/> attachment point.</para>
        ///
        /// <para><b>Note</b>: only two <paramref name="format"/>/<paramref name="dataType"/> parameter pairs are accepted.
        /// <see cref="gl.PixelFormat.Rgba"/>/<see cref="gl.PixelDataType.UnsignedByte"/> is always accepted, 
        /// and the other acceptable pair can be discovered by calling <see cref="ColorReadFormat"/> and <see cref="ColorReadType"/>.</para>
        ///
        /// <para><b>Note</b>: values for pixels that lie outside the window connected to the current GL context are undefined.</para>
        ///
        /// <para><b>Note</b>: if an error is generated, no change is made to the contents of data.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this framebuffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glReadPixels</para>
        /// </remarks>
        /// <typeparam name="T">the type of the elements in the <paramref name="data"/> array.</typeparam>
        /// <param name="left">left window coordinate of the first pixel that is read from the framebuffer.</param>
        /// <param name="bottom">bottom window coordinate of the first pixel that is read from the framebuffer.</param>
        /// <param name="width">width of the rectangle in pixels.</param>
        /// <param name="height">height of the rectangle in pixels.</param>
        /// <param name="data">the pixel data to be filled.</param>
        /// <param name="format">(optional) format of the pixels to return. Only Alpha, RGB and RGBA are supported. Defaults to RGBA.</param>
        /// <param name="dataType">(optional) data type of the pixels to return. Defaults to UnsignedByte.</param>
        /// <exception cref="GLException">InvalidEnum if <paramref name="format"/> or <paramref name="dataType"/> is not an accepted value.</exception>
        /// <exception cref="GLException">InvalidValue if either <paramref name="width"/> or <paramref name="height"/> is negative.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="dataType"/> is UnsignedShort565 and <paramref name="format"/> is not RGB.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="dataType"/> is UnsignedShort4444 or UnsignedShort5551 and <paramref name="format"/> is not RGBA.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="format"/> and <paramref name="dataType"/> are neither RGBA UnsignedByte, 
        /// respectively, nor the <paramref name="format"/>/<paramref name="dataType"/> pair returned by calling <see cref="ColorReadFormat"/> and <see cref="ColorReadType"/>.</exception>
        /// <exception cref="GLException">InvalidFramebufferOperation if the framebuffer is not framebuffer complete.</exception>
        /// <seealso cref="ColorReadFormat"/>
        /// <seealso cref="ColorReadType"/>
        /// <seealso cref="gl.PixelStore"/>
        /// <seealso cref="gl.GetPixelStore"/>
        /// <seealso cref="CompletenessStatus"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadPixels<T>(int left, int bottom, int width, int height, T[] data, gl.PixelFormat format = gl.PixelFormat.Rgba, gl.PixelDataType dataType = gl.PixelDataType.UnsignedByte) where T : struct
        {
            glUtils.CheckBinding(All.Framebuffer, Handle, this);
            GL.ReadPixels(left, bottom, width, height, (PixelFormat)format, (PixelType)dataType, data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Deletes the framebuffer.
        /// </summary>
        /// <remarks>
        /// After a framebuffer object is deleted, it has no attachments, and its name is free for reuse.
        /// If a framebuffer object that is currently bound is deleted, the binding reverts to 0 (the window-system-provided framebuffer).
        ///
        /// <para><b>OpenGL API</b>: glDeleteFramebuffers</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        protected override void DisposeHandle()
        {
            GL.DeleteFramebuffers(1, ref _handle);
            glUtils.Check(this);
        }
    }
}
