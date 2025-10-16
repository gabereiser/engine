using Red.Platform;

namespace Red.Systems
{
    public static class GraphicsSystem
    {
        private static GPU gpu = new GPU();
        public static void Register(Window window)
        {
            GPU.Register(window);
            window._context = gpu;
        }

        public static void Run()
        {
            // Start a thread to continuously cull from the camera frustum.
            while (!Application.Instance.Window.IsClosing)
            {
                GPU.Instance.ProcessEvents();
            }
        }

    }
}