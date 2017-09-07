using System;
using System.Diagnostics;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Examples.Shared.iOS
{
    public abstract class GLAppDelegate : UIApplicationDelegate, IPlatform
    {
        private Application _application;
        private float _screenScale;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            CGRect bounds = UIScreen.MainScreen.Bounds;
            _screenScale = (float)UIScreen.MainScreen.Scale;

            Window = new UIWindow(bounds);

            Debug.Assert(_application == null);
            _application = CreateApplication();
            Debug.Assert(_application != null);

            _application.Init(this, (int)(bounds.Width * _screenScale), (int)(bounds.Height * _screenScale));
            GLViewController viewController = new GLViewController(_application);
            Window.RootViewController = viewController;

            Window.MakeKeyAndVisible();

            return true;
        }

        protected abstract Application CreateApplication();

        #region IPlatform
        public float ScreenScale => _screenScale;

        public void ShutDown()
        {
            // Not used
        }
        #endregion
    }
}