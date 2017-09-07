using Examples.Shared.Windows;
using System;

namespace E08StencilOperations
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Stencil Operations");
        }
    }
}
