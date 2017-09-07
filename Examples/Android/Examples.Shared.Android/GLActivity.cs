using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Util;

namespace Examples.Shared.Android
{
    public abstract class GLActivity : Activity, IPlatform
    {
        private GLView _view;
        private readonly Application _application;
        private float _screenScale;

        protected GLActivity(Application application) : base()
        {
            _application = application;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _view = new GLView(this, _application);

            DisplayMetrics metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);
            _screenScale = metrics.Density;

            Point point = new Point();
            WindowManager.DefaultDisplay.GetRealSize(point);
            _application.Init(this, point.X, point.Y);
            SetContentView(_view);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _view.Resume();
            Window.DecorView.SystemUiVisibility |= (StatusBarVisibility)(Int32)
                (SystemUiFlags.HideNavigation
                | SystemUiFlags.Fullscreen
                | SystemUiFlags.LayoutFullscreen
                | SystemUiFlags.ImmersiveSticky
                | SystemUiFlags.LayoutStable);
        }

        #region IPlatform
        public float ScreenScale => _screenScale;

        public void ShutDown()
        {
            // Not used
        }
        #endregion
    }
}