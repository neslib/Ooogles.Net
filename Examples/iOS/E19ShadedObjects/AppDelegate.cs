﻿using Examples.Shared.iOS;
using Foundation;

namespace E19ShadedObjects
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