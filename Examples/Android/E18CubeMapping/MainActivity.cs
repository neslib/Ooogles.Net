using Android.App;
using Examples.Shared.Android;

namespace E18CubeMapping
{
    [Activity(Label = "E18CubeMapping", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

