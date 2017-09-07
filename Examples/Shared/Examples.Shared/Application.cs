using System.Diagnostics;
#if !__IOS__ && !__ANDROID__
using OpenTK.Input;
#endif
using Ooogles;

namespace Examples.Shared
{
    public abstract class Application
    {
        private double _secondsPerTick;
        private long _startTicks;
        private long _lastUpdateTicks;

        protected IPlatform Platform { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool ShutDownOnEscape { get; private set; }

        public void Init(IPlatform platform, int width, int height, bool shutDownOnEscape = true)
        {
            Debug.Assert(platform != null);
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);

            Platform = platform;
            Width = width;
            Height = height;
            ShutDownOnEscape = shutDownOnEscape;

            _secondsPerTick = 1.0 / Stopwatch.Frequency;
            _startTicks = Stopwatch.GetTimestamp();
            _lastUpdateTicks = _startTicks;
        }

        public abstract void Load();
        public abstract void Unload();
        public abstract void Render(float deltaTimeSec, double totalTimeSec);

        public virtual void Resize(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;
        }

        internal void Render()
        {
            long ticks = Stopwatch.GetTimestamp();
            double deltaSec = (ticks - _lastUpdateTicks) * _secondsPerTick;
            double totalSec = (ticks - _startTicks) * _secondsPerTick;
            _lastUpdateTicks = ticks;

            gl.Viewport(Width, Height);

            Render((float)deltaSec, totalSec);
        }

        public virtual bool NeedStencilBuffer()
        {
            return false;
        }

#if !__IOS__ && !__ANDROID__
        public virtual void KeyDown(KeyboardKeyEventArgs e)
        {
            if ((e.Key == Key.Escape) && ShutDownOnEscape)
                Platform.ShutDown();
        }
#endif
    }
}
