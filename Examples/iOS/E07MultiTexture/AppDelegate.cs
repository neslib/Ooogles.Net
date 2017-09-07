using Examples.Shared.iOS;
using Foundation;
using UIKit;

namespace E07MultiTexture
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


