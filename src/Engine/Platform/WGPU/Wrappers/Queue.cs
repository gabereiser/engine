using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Red.Platform.WGPU.Helpers;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class Queue : IDisposable
    {
        private QueueImpl _impl;

        internal Queue(QueueImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Queue));

            _impl = impl;
        }

        public void OnSubmittedWorkDone(QueueWorkDoneCallback callback)
        {
            var context = new Callback<QueueWorkDoneStatus>
            {
                Delegate = (s, m, userData) => callback(s),
                UserData = IntPtr.Zero,
            };
            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    QueueOnSubmittedWorkDone(_impl, &QueueWorkDoneCallback, (void*)GCHandle.ToIntPtr(handle));
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void QueueWorkDoneCallback(QueueWorkDoneStatus status, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (Callback<QueueWorkDoneStatus>)handle.Target;
                context.Delegate?.Invoke(status, "Done", context.UserData);
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }

        public void Submit(CommandBuffer[] commands)
        {
            Span<CommandBufferImpl> commandBufferImpls = stackalloc CommandBufferImpl[commands.Length];

            for (int i = 0; i < commands.Length; i++)
                commandBufferImpls[i] = commands[i].Impl;

            QueueSubmit(_impl, (uint)commands.Length, ref commandBufferImpls.GetPinnableReference());
        }

        public void WriteBuffer<T>(Buffer buffer, ulong bufferOffset, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            unsafe
            {
                ulong structSize = (ulong)sizeof(T);

                fixed (void* dataPtr = data)
                {
                    QueueWriteBuffer(_impl, buffer.Impl, bufferOffset,
                        dataPtr,
                        (ulong)data.Length * structSize);
                }
            }
        }

        public void WriteTexture<T>(ImageCopyTexture destination, ReadOnlySpan<T> data,
            in TextureDataLayout dataLayout, in Extent3D writeSize)
            where T : unmanaged
        {
            unsafe
            {
                ulong structSize = (ulong)sizeof(T);

                fixed (void* dataPtr = data)
                {
                    QueueWriteTexture(_impl, destination,
                        dataPtr,
                        (ulong)data.Length * structSize,
                        dataLayout, in writeSize);
                }
            }
        }

        /// <summary>
        /// This function will be called automatically when this Queue's associated Device is disposed.
        /// </summary>
        public void Dispose()
        {
            QueueRelease(_impl);
        }
    }
}
