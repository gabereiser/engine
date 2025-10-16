using System;
using System.Collections.Generic;
using Red.Platform.WGPU.Helpers;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class BindGroupLayout : IDisposable
    {
        private static Dictionary<BindGroupLayoutImpl, BindGroupLayout> instances =
            new Dictionary<BindGroupLayoutImpl, BindGroupLayout>();

        private BindGroupLayoutImpl _impl;

        internal BindGroupLayoutImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(BindGroupLayout));

                return _impl;
            }

            private set => _impl = value;
        }

        private BindGroupLayout(BindGroupLayoutImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(BindGroupLayout));

            Impl = impl;
        }

        internal static BindGroupLayout For(BindGroupLayoutImpl impl)
            => impl.Handle == IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new BindGroupLayout(impl));

        public void Dispose()
        {
            BindGroupLayoutRelease(Impl);
            Impl = default;
        }
    }
}
