﻿using Examples.Shared.Windows;
using System;

namespace E14MetallicTorus
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Metallic Torus");
        }
    }
}
