﻿using Foundation;
using Examples.Shared.iOS;

namespace E08StencilOperations
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


