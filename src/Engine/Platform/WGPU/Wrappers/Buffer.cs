using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Red.Platform.WGPU.Helpers;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class Buffer : IDisposable
    {
        private BufferImpl _impl;

        internal BufferImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Buffer));

                return _impl;
            }

            private set => _impl = value;
        }

        public ulong SizeInBytes { get; private set; }

        internal Buffer(BufferImpl impl, in BufferDescriptor descriptor)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Buffer));

            Impl = impl;

            SizeInBytes = descriptor.size;
        }

        public unsafe Span<T> GetConstMappedRange<T>(ulong offset, int size)
            where T : unmanaged
        {
            var structSize = (ulong)Marshal.SizeOf<T>();

            void* ptr = BufferGetConstMappedRange(Impl,
                offset * structSize, (ulong)size * structSize);

            return new Span<T>(ptr, size);
        }

        public unsafe Span<T> GetMappedRange<T>(ulong offset, int size)
            where T : unmanaged
        {
            var structSize = (ulong)Marshal.SizeOf<T>();

            void* ptr = BufferGetMappedRange(Impl,
                offset * structSize, (ulong)size * structSize);

            return new Span<T>(ptr, size);
        }

        public void MapAsync(MapMode mode, ulong offset, ulong size, BufferMapCallback callback)
        {
            GCHandle handle = default;
            var context = new Callback<BufferMapAsyncStatus>
            {
                Delegate = (s, m, _) =>
                {
                    if (handle.IsAllocated) handle.Free();
                    callback.Invoke(s);
                }
            };
            handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    BufferMapAsync(Impl, (uint)mode, offset, size, &BufferMapCallback, (void*)GCHandle.ToIntPtr(handle));
                }
            }
            catch
            {
                if (handle.IsAllocated) handle.Free();
                throw;
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void BufferMapCallback(BufferMapAsyncStatus status, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (Callback<BufferMapAsyncStatus>)handle.Target;
                context.Delegate?.Invoke(status, "", context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        public void Unmap() => BufferUnmap(Impl);

        public void Dispose()
        {
            BufferDestroy(Impl);
            BufferRelease(Impl);
            Impl = default;
        }
    }

    public delegate void BufferMapCallback(BufferMapAsyncStatus status);

    public static partial class BufferExtensions
    {
        public static void SetData<T>(this Buffer buffer, ulong offset, ReadOnlySpan<T> span)
            where T : unmanaged
        {
            var dest = buffer.GetMappedRange<T>(offset, span.Length);

            span.CopyTo(dest);
        }
    }
}
