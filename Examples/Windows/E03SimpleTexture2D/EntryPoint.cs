using Examples.Shared.Windows;
using System;

namespace E03SimpleTexture2D
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Simple Texture 2D");
        }
    }
}
