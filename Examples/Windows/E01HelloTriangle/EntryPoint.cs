using System;
using Examples.Shared.Windows;

namespace E01HelloTriangle
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Hello Triangle");
        }
    }
}
