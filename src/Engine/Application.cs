using System;
using System.Diagnostics;

using Reactor.Common;
using Reactor.Platform;
using Reactor.Systems;
using Reactor.Types;
using Window = Reactor.Platform.Window;

namespace Reactor
{
    public abstract class Application : IDisposable
    {
        private static Threading _threading = Threading.Instance;
        
        internal static Application Instance;
        public Window Window { get; internal set; }
        public Viewport Viewport { get; set; }
        public Camera Camera { get; set; }
        
        public Action<double> Render;
        public Action<double> Update;

        internal Stopwatch renderTimer;
        internal Stopwatch updateTimer;
        
        public Application(string title)
        {
            Instance = this;
            Window.GlfwInit();
            var monitor = Window.PrimaryMonitor();
            Window = new Window(title, monitor.WorkArea.Size);
            
            renderTimer = new Stopwatch();
            updateTimer = new Stopwatch();
        }

        void initTimers()
        {
            renderTimer.Start();
            updateTimer.Start();
        }

        void resetTimers()
        {
            renderTimer.Reset();
            updateTimer.Reset();
        }

        void stopTimers()
        {
            renderTimer.Stop();
            updateTimer.Stop();
        }
        public void Run()
        {

            while (!Window.IsClosing)
            {
                GPU.ProcessEvents();
                
                resetTimers();
                initTimers();
                
                Threading.RunOnMain(() =>
                {
                    var delta = renderTimer.Elapsed.TotalSeconds;
                    Render?.Invoke(delta);
                });
                Threading.RunOnPool(() =>
                {
                    var delta = updateTimer.Elapsed.TotalSeconds;
                    Update?.Invoke(delta);
                });
                

                Threading.Flush();
                stopTimers();
            }
        }

        public void Dispose()
        {
            Window.Dispose();
        }
    }
}