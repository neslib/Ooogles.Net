using System;
using System.Diagnostics;
using Android.Content;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform.Android;

namespace Examples.Shared.Android
{
    public class GLView : AndroidGameView
    {
        private readonly Application _application;

        public GLView(Context context, Application application) : base(context)
        {
            Debug.Assert(application != null);
            _application = application;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _application.Load();
            Run(60);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            _application.Unload();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _application.Render();
            SwapBuffers();
        }

        protected override void CreateFrameBuffer()
        {
            ContextRenderingApi = GLVersion.ES2;
            GraphicsMode = new GraphicsMode(32, 16, _application.NeedStencilBuffer() ? 8 : 0, 0, 0, 2, false);
            base.CreateFrameBuffer();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _application.Resize(Width, Height);
        }
    }
}
