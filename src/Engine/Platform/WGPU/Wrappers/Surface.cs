using System;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class Surface : IDisposable
    {
        internal SurfaceImpl Impl;

        internal Surface(SurfaceImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Surface));

            Impl = impl;
        }

        public TextureFormat GetPreferredFormat(Adapter adapter)
        {
            return SurfaceGetPreferredFormat(Impl, adapter.Impl);
        }

        public void Dispose()
        {
            SurfaceRelease(Impl);
            Impl = default;
        }

        public void Configure()
        {

        }
    }
}
