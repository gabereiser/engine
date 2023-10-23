
using System.Drawing;

namespace Reactor.Graphics
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