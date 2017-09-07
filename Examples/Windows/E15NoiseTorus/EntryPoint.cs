using System;
using Examples.Shared.Windows;

namespace E15NoiseTorus
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Noise Torus");
        }
    }
}
