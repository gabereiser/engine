using System;
using System.Drawing;
using Reactor.Graphics;
using GLFW;
using Reactor.Math3D;
using Reactor.Platform.WGPU.Wrappers;
using Reactor.Platform.Windows;
using Reactor.Systems;
using Reactor.Platform.WGPU;
using Reactor.Common;

namespace Reactor.Platform
{
    public class Window : NativeWindow, IDisposable
    {
        
        internal IGraphicsContext _context;
        
        public Action Load;
        public Action Resize;

        public Window(string title) : this(title, new Size(1280, 720))
        {
        }

        public Window(string title, Size size) : base(size.Width, size.Height, title)
        {
            Refreshed += (o, s) => { Log.Debug("Window Refreshed"); };
            FramebufferSizeChanged += (o, size) =>
            {
                if (size.Size.IsEmpty)
                    return;
                Resize?.Invoke();
            };
        }
        public void Init()
        {
            register();
            Load?.Invoke();
        }
        internal static void GlfwInit()
        {
            if (!Glfw.Init())
                throw new Exception("Unable to initialize Glfw");
            
            Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
        }

        internal static Monitor PrimaryMonitor()
        {
            return Glfw.PrimaryMonitor;
        }
        void register()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Win32.WindowDressing(this.Hwnd);
            }

            Glfw.PollEvents();
            GraphicsSystem.Register(this);
        }
        
        /*public IInputContext GetInputContext()
        {
            return _window.CreateInput();
        }*/

        public IGraphicsContext GetGraphicsContext()
        {
            return _context;
        }
        
        internal Surface CreateWebGPUSurface(ref Instance instance)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return instance.CreateSurfaceFromWindowsHWND(Handle, Hwnd, "Window:0");
            }
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return instance.CreateSurfaceFromMetalLayer(handle, "Window:0");
            }
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return instance.CreateSurfaceFromWaylandSurface(handle, "Window:0");
            }

            return new Surface(new Wgpu.SurfaceImpl(handle));
        }
        
        public void Dispose()
        {
            _context?.Dispose();
        }
        
        
    }
}