using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using System.Diagnostics;
using OpenTK.Input;

namespace Examples.Shared.Windows
{
    public class GLWindow : GameWindow, IPlatform
    {
        private readonly Application _application;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        static GLWindow()
        {
            string path = Assembly.GetEntryAssembly().Location;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, (IntPtr.Size == 8) ? "x64" : "x86");
            SetDllDirectory(path);
        }

        public GLWindow(Application application, int width, int height, string title, bool shutDownOnEscape = true) : base(
            width, height,
            //GraphicsMode.Default,
            new GraphicsMode(32, 16, application.NeedStencilBuffer() ? 8 : 0, 0, 0, 2, false),
            title,
            GameWindowFlags.FixedWindow,
            DisplayDevice.Default,
            2, 0,
            GraphicsContextFlags.Embedded)
        {
            Debug.Assert(application != null);
            try
            {
                _application = application;
                application.Init(this, width, height, shutDownOnEscape);
                Run(60);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _application.Load();
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

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            _application.KeyDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _application.Resize(Width, Height);
        }

        #region IPlatform
        public float ScreenScale => 1.0f;

        public void ShutDown()
        {
            Exit();
        }
        #endregion
    }
}
