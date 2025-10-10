using System.ComponentModel;
using System.Drawing;
using Reactor;
using Reactor.Common;
using Reactor.Graphics;

namespace Red
{
    public class Application : Reactor.Application
    {
        public static void Main(string[] args)
        {
            Log.Enabled = true;
            Log.Level = LogLevel.INFO;
            var app = new Application();
            app.Run();
        }

        private IGraphicsContext graphics;
        Application() : base("Reditor")
        {
            
            Window.Load += OnLoad;
            Window.Resize += Resized;
            
            Window.Closing += OnClosing;
            
            Render += OnRender;
            Update += OnUpdate;
            Window.Init();
        }

        void OnLoad()
        {
            graphics = Window.GetGraphicsContext();
            
            //Set-up input context. Once we set the event handler, we don't need the context anymore. It's a good practice to save
            //it like we do the graphics context. Perhaps in the future when it's needed for UI.
            /*IInputContext input = Window.GetInputContext();
            for (var i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }*/
        }

        void OnClosing(object sender, CancelEventArgs args)
        {
            
        }

        void OnUpdate(double time)
        {
            
        }

        void OnRender(double time)
        {
            graphics?.Begin();
            graphics?.Clear();
            graphics?.End();
            graphics?.Present();
        }

        void Resized()
        {
            graphics?.Resize();
        }

        /*void KeyDown(IKeyboard keyboard, Key key, int keycode)
        {
            if (key == Key.Escape)
            {
                Window.Close();
            }
        }*/
    }
}