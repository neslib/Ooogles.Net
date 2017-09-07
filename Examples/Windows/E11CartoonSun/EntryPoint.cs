﻿using Examples.Shared.Windows;
using System;

namespace E11CartoonSun
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new GLWindow(new App(), 800, 600, "Cartoon Sun");
        }
    }
}