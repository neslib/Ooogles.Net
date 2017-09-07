using System;
using System.Diagnostics;
using OpenTK.Platform.iPhoneOS;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using OpenGLES;
using UIKit;
using CoreAnimation;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using System.Reflection;
using System.Drawing;

namespace Examples.Shared.iOS
{
    public class GLView : iPhoneOSGameView
    {
        private Application _application;
        private int _colorRenderbuffer;
        public bool _sizeChanged;

        [Export("layerClass")]
        public static Class GetLayerClass()
        {
            return iPhoneOSGameView.GetLayerClass();
        }

        [Export("initWithFrame:")]
        public GLView(CGRect frame) : base(frame)
        {
            LayerColorFormat = EAGLColorFormat.RGBA8;
            UserInteractionEnabled = true;
            MultipleTouchEnabled = true;
            ContentScaleFactor = UIScreen.MainScreen.Scale;
        }

        public void SetApplication(Application application)
        {
            Debug.Assert(application != null);
            _application = application;
        }

        protected override void CreateFrameBuffer()
        {
            Debug.Assert(_application != null);
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES2;

            CreateFrameBufferEx();

            _application.Load();
        }

        private void CreateFrameBufferEx()
        {         
            /* iPhoneOSGameView does NOT support depth and stencil buffers!
             * To get theses to work, we need to write our own implementation of CreateFrameBuffer().
             * However, the base CreateFrameBuffer() method access some private fields and internal classes.
             * So we need some ugly reflection to update those fields.
             * Most of the implementation is copied from iPhoneOSGameView.CreateFrameBuffer(). */

            CAEAGLLayer eaglLayer = (CAEAGLLayer)Layer;
            eaglLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys(
                new NSObject[] { NSNumber.FromBoolean(LayerRetainsBacking), LayerColorFormat },
                new NSObject[] { EAGLDrawableProperty.RetainedBacking, EAGLDrawableProperty.ColorFormat });
            ConfigureLayer(eaglLayer);

            GraphicsContext = Utilities.CreateGraphicsContext(ContextRenderingApi);

            // Use reflection to mimic this code:
            //   this.gl = GLCalls.GetGLCalls(ContextRenderingApi);
            var type = typeof(iPhoneOSGameView).Assembly.GetType("OpenTK.Platform.iPhoneOS.GLCalls");
            Debug.Assert(type != null);
            var method = type.GetMethod("GetGLCalls", BindingFlags.Public | BindingFlags.Static);
            Debug.Assert(method != null);
            var gl = method.Invoke(null, new object[] { ContextRenderingApi });
            var field = typeof(iPhoneOSGameView).GetField("gl", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null);
            field.SetValue(this, gl);

            // We use the regular "renderbuffer" for depth and stencil operations.
            // We need an additional _colorRenderbuffer just for the pixel colors.
            GL.GenRenderbuffers(1, out _colorRenderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            // We need to link the EAGL layer to the color buffer (NOT to the regular renderbuffer)
            if (!EAGLContext.RenderBufferStorage((uint)All.Renderbuffer, eaglLayer))
            {
                GL.DeleteRenderbuffers(1, ref _colorRenderbuffer);
                _colorRenderbuffer = 0;
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                throw new InvalidOperationException("Error with EAGLContext.RenderBufferStorage!");
            }

            // Get dimensions of _colorRenderbuffer
            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out int width);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);
            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out int height);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            GL.GenFramebuffers(1, out int framebuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            // Use reflection to mimic this code:
            //   this.framebuffer = framebuffer
            field = typeof(iPhoneOSGameView).GetField("framebuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null);
            field.SetValue(this, framebuffer);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            // Create regular RenderBuffer for depth and stencil
            GL.GenRenderbuffers(1, out int renderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            // Use reflection to mimic this code:
            //   this.renderbuffer = renderbuffer
            field = typeof(iPhoneOSGameView).GetField("renderbuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null);
            field.SetValue(this, renderbuffer);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            if (_application.NeedStencilBuffer())
            {
                // Use the regular renderbuffer for combined storage of depth and stencil values
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferInternalFormat)All.Depth24Stencil8Oes, width, height);
                Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

                // Attach stencil buffer
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.StencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);
                Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);
            }
            else
            {
                // Use the regular renderbuffer for combined storage of depth values only
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, width, height);
                Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);
            }

            // Attach color and depth buffers
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, _colorRenderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderbuffer);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            // Check if framebuffer is complete
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            Debug.Assert(status == FramebufferErrorCode.FramebufferComplete);

            Size newSize = new Size((int)Math.Round(eaglLayer.Bounds.Size.Width), (int)eaglLayer.Bounds.Size.Height);
            this.Size = newSize;

            GL.Viewport(0, 0, newSize.Width, newSize.Height);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);
            GL.Scissor(0, 0, newSize.Width, newSize.Height);
            Debug.Assert(GL.GetErrorCode() == ErrorCode.NoError);

            var frameBufferWindow = new WeakReference(Window);

            // Use reflection to mimic this code:
            //   this.frameBufferWindow = frameBufferWindopw
            field = typeof(iPhoneOSGameView).GetField("frameBufferWindow", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null);
            field.SetValue(this, frameBufferWindow);

            var frameBufferLayer = new WeakReference(Layer);

            // Use reflection to mimic this code:
            //   this.frameBufferLayer = frameBufferLayer
            field = typeof(iPhoneOSGameView).GetField("frameBufferLayer", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(field != null);
            field.SetValue(this, frameBufferLayer);
        }

        protected override void DestroyFrameBuffer()
        {
            _application.Unload();
            if (_colorRenderbuffer != 0)
            {
                GL.DeleteRenderbuffers(1, ref _colorRenderbuffer);
                _colorRenderbuffer = 0;
            }
            base.DestroyFrameBuffer();
        }

        protected override void ConfigureLayer(CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Debug.Assert(_application != null);

            if (_sizeChanged)
            {
                // Dimensions of layer have changed.
                // We need to update dimensions of color/depth/stencil buffers accordingly.
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderbuffer);
                EAGLContext.RenderBufferStorage((uint)All.Renderbuffer, (CAEAGLLayer)Layer);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Renderbuffer);
                if (_application.NeedStencilBuffer())
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferInternalFormat)All.Depth24Stencil8Oes, _application.Width, _application.Height);
                else
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, _application.Width, _application.Height);

                _sizeChanged = false;
            }

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderbuffer);
            _application.Render();

            // Do NOT call regular SwapBuffers(), since this binds the wrong render buffer
            GraphicsContext.SwapBuffers();
        }

        public override void LayoutSubviews()
        {
            // This method is also called when device rotates
            int newWidth = (int)(Layer.Bounds.Width * ContentScaleFactor);
            int newHeight = (int)(Layer.Bounds.Height * ContentScaleFactor);
            _sizeChanged = ((newWidth != _application.Width) || (newHeight != _application.Height));
            base.LayoutSubviews();
            _application.Resize(newWidth, newHeight);
        }
    }
}
