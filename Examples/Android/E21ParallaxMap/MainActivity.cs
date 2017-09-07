using Android.App;
using Examples.Shared.Android;

namespace E21ParallaxMap
{
    [Activity(Label = "E21ParallaxMap", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

