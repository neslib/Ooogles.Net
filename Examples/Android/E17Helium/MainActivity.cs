using Android.App;
using Examples.Shared.Android;

namespace E17Helium
{
    [Activity(Label = "E17Helium", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

