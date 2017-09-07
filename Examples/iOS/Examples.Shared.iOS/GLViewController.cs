using System;
using System.Diagnostics;
using UIKit;

namespace Examples.Shared.iOS
{
    public class GLViewController : UIViewController
    {
        private readonly Application _application;

        public GLViewController(Application application) : base()
        {
            Debug.Assert(application != null);
            _application = application;
        }

        public override void LoadView()
        {
            View = new GLView(UIScreen.MainScreen.Bounds);
            ((GLView)View).SetApplication(_application);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ((GLView)View).Run(60.0f);
        }
    }
}
