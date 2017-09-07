using Android.App;
using Examples.Shared.Android;

namespace E12StripedCubes
{
    [Activity(Label = "E12StripedCubes", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

