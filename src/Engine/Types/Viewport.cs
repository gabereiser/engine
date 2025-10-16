using System.Runtime.InteropServices;

namespace Red.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Viewport
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}