using Android.App;
using Examples.Shared.Android;

namespace E10Mandelbrot
{
    [Activity(Label = "E10Mandelbrot", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

