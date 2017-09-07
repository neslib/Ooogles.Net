using Examples.Shared.iOS;
using Foundation;
using UIKit;

namespace E04MipMap2D
{
    [Register("AppDelegate")]
    public class AppDelegate : GLAppDelegate
    {
        protected override Examples.Shared.Application CreateApplication()
        {
            return new App();
        }
    }
}


