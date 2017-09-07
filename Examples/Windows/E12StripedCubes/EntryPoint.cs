using System;
using Examples.Shared.Windows;

namespace E12StripedCubes
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Striped Cubes");
        }
    }
}
