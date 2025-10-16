
using System.Drawing;

namespace Red.Graphics
{
    public interface IGraphicsContext
    {
        void CheckError();
        void Begin();
        void Clear();
        void Dispose();

        void End();
        void Present();

        void Resize();
    }
}