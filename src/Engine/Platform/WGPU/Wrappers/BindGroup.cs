using System;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class BindGroup : IDisposable
    {
        private BindGroupImpl _impl;

        internal BindGroupImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(BindGroup));

                return _impl;
            }

            private set => _impl = value;
        }

        internal BindGroup(BindGroupImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(BindGroup));

            Impl = impl;
        }

        public void Dispose()
        {
            BindGroupRelease(Impl);
            Impl = default;
        }
    }
}
