using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Red.Platform.WGPU.Helpers;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class ComputePipeline : IDisposable
    {
        private ComputePipelineImpl _impl;

        internal ComputePipelineImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(ComputePipeline));

                return _impl;
            }

            private set => _impl = value;
        }

        internal ComputePipeline(ComputePipelineImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(ComputePipeline));

            Impl = impl;
        }

        public BindGroupLayout GetBindGroupLayout(uint groupIndex)
            => BindGroupLayout.For(ComputePipelineGetBindGroupLayout(Impl, groupIndex));

        public void SetLabel(string label) => ComputePipelineSetLabel(Impl, label);

        public void Dispose()
        {
            ComputePipelineRelease(Impl);
            Impl = default;
        }
    }

    public class PipelineLayout : IDisposable
    {
        private PipelineLayoutImpl _impl;

        internal PipelineLayoutImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(PipelineLayout));

                return _impl;
            }

            private set => _impl = value;
        }

        internal PipelineLayout(PipelineLayoutImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(PipelineLayout));

            Impl = impl;
        }

        public void Dispose()
        {
            PipelineLayoutRelease(Impl);
            Impl = default;
        }
    }

    public class QuerySet : IDisposable
    {
        private QuerySetImpl _impl;

        internal QuerySet(QuerySetImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(QuerySet));

            Impl = impl;
        }

        internal QuerySetImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(QuerySet));

                return _impl;
            }

            private set => _impl = value;
        }

        public void Dispose()
        {
            QuerySetDestroy(Impl);
            QuerySetRelease(Impl);
            Impl = default;
        }
    }

    public class RenderPipeline : IDisposable
    {
        private RenderPipelineImpl _impl;

        internal RenderPipelineImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(RenderPipeline));

                return _impl;
            }

            private set => _impl = value;
        }

        internal RenderPipeline(RenderPipelineImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderPipeline));

            Impl = impl;
        }

        public BindGroupLayout GetBindGroupLayout(uint groupIndex)
            => BindGroupLayout.For(RenderPipelineGetBindGroupLayout(Impl, groupIndex));

        public void SetLabel(string label) => RenderPipelineSetLabel(Impl, label);

        public void Dispose()
        {
            RenderPipelineRelease(Impl);
            Impl = default;
        }
    }

    public class Sampler : IDisposable
    {
        private SamplerImpl _impl;

        internal SamplerImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Sampler));

                return _impl;
            }

            private set => _impl = value;
        }

        internal Sampler(SamplerImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Sampler));

            Impl = impl;
        }

        public void Dispose()
        {
            SamplerRelease(Impl);
            Impl = default;
        }
    }

    public class ShaderModule : IDisposable
    {
        private ShaderModuleImpl _impl;

        internal ShaderModuleImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(ShaderModule));

                return _impl;
            }

            private set => _impl = value;
        }

        internal ShaderModule(ShaderModuleImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(ShaderModule));

            Impl = impl;
        }

        public void GetCompilationInfo(CompilationInfoCallback callback)
        {
            unsafe
            {
                var context = new CallbackContext<CompilationInfoRequestStatus, CompilationInfo>
                {
                    Delegate = (s, p, m, userData) => callback?.Invoke(s, new ReadOnlySpan<CompilationMessage>(p.messages, (int)p.messages)),
                    UserData = IntPtr.Zero,
                };
                var handle = GCHandle.Alloc(context);
                try
                {
                    ShaderModuleGetCompilationInfo(Impl, &GetCompilationInfoCallback, (void*)GCHandle.ToIntPtr(handle));
                }
                finally
                {
                    if (handle.IsAllocated) handle.Free();
                }
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void GetCompilationInfoCallback(CompilationInfoRequestStatus s, CompilationInfo* info, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (CallbackContext<CompilationInfoRequestStatus, CompilationInfo>)handle.Target;
                context.Delegate.Invoke(s, *info, "", context.UserData);
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        public void SetLabel(string label) => ShaderModuleSetLabel(Impl, label);

        public void Dispose()
        {
            ShaderModuleRelease(Impl);
            Impl = default;
        }
    }

    public delegate void CompilationInfoCallback(CompilationInfoRequestStatus status,
        ReadOnlySpan<CompilationMessage> messages);

    public class SwapChain : IDisposable
    {
        private SwapChainImpl _impl;

        internal SwapChain(SwapChainImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(SwapChain));

            _impl = impl;
        }

        public TextureView GetCurrentTextureView() => TextureView.CreateUntracked(SwapChainGetCurrentTextureView(_impl));

        public void Present()
            => SwapChainPresent(_impl);

        public void Dispose()
        {
            TextureView.Forget(SwapChainGetCurrentTextureView(_impl));
            SwapChainRelease(_impl);
            _impl = default;
        }
    }

    public delegate void QueueWorkDoneCallback(QueueWorkDoneStatus status);

    public class CommandBuffer : IDisposable
    {
        private CommandBufferImpl _impl;

        internal CommandBufferImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(CommandBuffer));

                return _impl;
            }

            private set => _impl = value;
        }

        internal CommandBuffer(CommandBufferImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(CommandBuffer));

            Impl = impl;
        }

        public void Dispose()
        {
            CommandBufferRelease(Impl);
            Impl = default;
        }
    }

    public class RenderBundle : IDisposable
    {
        private RenderBundleImpl _impl;

        internal RenderBundleImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(RenderBundle));

                return _impl;
            }

            private set => _impl = value;
        }

        internal RenderBundle(RenderBundleImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderBundle));
            Impl = impl;
        }

        public void Dispose()
        {
            RenderBundleRelease(Impl);
            Impl = default;
        }
    }
}
