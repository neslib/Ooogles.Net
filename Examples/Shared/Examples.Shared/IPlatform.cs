using System;
using System.Collections.Generic;
using System.Text;

namespace Examples.Shared
{
    public interface IPlatform
    {
        float ScreenScale { get; }
        void ShutDown();
    }
}
