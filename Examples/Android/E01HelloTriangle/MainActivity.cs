using Android.App;
using Android.Widget;
using Android.OS;
using Examples.Shared.Android;

namespace E01HelloTriangle
{
    [Activity(Label = "E01HelloTriangle", MainLauncher = true)]
    public class MainActivity : GLActivity
    {
        public MainActivity() : base(new App())
        {

        }
    }
}

