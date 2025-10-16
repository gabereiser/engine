using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Red.Platform.WGPU.Helpers;

using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public struct BindGroupEntry
    {
        public uint Binding;
        public Buffer Buffer;
        public ulong Offset;
        public ulong Size;
        public Sampler Sampler;
        public TextureView TextureView;
    }

    public struct ProgrammableStageDescriptor
    {
        public ShaderModule Module;
        public string EntryPoint;
    }

    public struct VertexState
    {
        public ShaderModule Module;
        public string EntryPoint;
        public VertexBufferLayout[] bufferLayouts;
    }

    public partial struct VertexBufferLayout
    {
        public ulong ArrayStride;

        public Wgpu.VertexStepMode StepMode;

        public VertexAttribute[] Attributes;
    }

    public struct FragmentState
    {
        public ShaderModule Module;
        public string EntryPoint;
        public ColorTargetState[] colorTargets;
    }

    public struct ColorTargetState
    {
        public Wgpu.TextureFormat Format;

        public BlendState? BlendState;

        public uint WriteMask;
    }

    public class Device : IDisposable
    {
        private DeviceImpl _impl;

        public DeviceImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Device));

                return _impl;
            }

            private set => _impl = value;
        }

        public Queue Queue { get; private set; }

        internal Device(DeviceImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Device));

            Impl = impl;
            Queue = new Queue(DeviceGetQueue(impl));
        }

        public BindGroup CreateBindGroup(string label, BindGroupLayout layout, BindGroupEntry[] entries)
        {
            Span<Wgpu.BindGroupEntry> entriesInner = stackalloc Wgpu.BindGroupEntry[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                BindGroupEntry entryOuter = entries[i];
                entriesInner[i] = new Wgpu.BindGroupEntry
                {
                    binding = entryOuter.Binding,
                    buffer = entryOuter.Buffer?.Impl ?? default,
                    offset = entryOuter.Offset,
                    size = entryOuter.Size,
                    sampler = entryOuter.Sampler?.Impl ?? default,
                    textureView = entryOuter.TextureView?.Impl ?? default
                };
            }

            unsafe
            {
                fixed (Wgpu.BindGroupEntry* pEntries = entriesInner)
                {
                    return new BindGroup(
                        DeviceCreateBindGroup(Impl, new BindGroupDescriptor
                        {
                            label = label,
                            layout = layout.Impl,
                            entries = pEntries,
                            entryCount = (uint)entries.Length
                        })
                    );
                }
            }
        }

        public BindGroupLayout CreateBindgroupLayout(string label, BindGroupLayoutEntry[] entries)
        {
            unsafe
            {
                fixed (BindGroupLayoutEntry* pEntries = entries)
                {
                    return BindGroupLayout.For(
                        DeviceCreateBindGroupLayout(Impl, new BindGroupLayoutDescriptor
                        {
                            label = label,
                            entries = pEntries,
                            entryCount = (uint)entries.Length
                        })
                    );
                }
            }
        }

        public Buffer CreateBuffer(string label, bool mappedAtCreation, ulong size, BufferUsage usage)
        {
            var desc = new BufferDescriptor
            {
                label = label,
                mappedAtCreation = mappedAtCreation,
                size = size,
                usage = (uint)usage
            };

            return new Buffer(DeviceCreateBuffer(Impl, desc), desc);
        }

        public CommandEncoder CreateCommandEncoder(string label)
        {
            return new CommandEncoder(
                DeviceCreateCommandEncoder(Impl, new CommandEncoderDescriptor
                {
                    label = label
                })
            );
        }

        public ComputePipeline CreateComputePipeline(string label, ProgrammableStageDescriptor compute)
        {
            return new ComputePipeline(
                DeviceCreateComputePipeline(Impl, new ComputePipelineDescriptor
                {
                    label = label,
                    compute = new Wgpu.ProgrammableStageDescriptor
                    {
                        module = compute.Module.Impl,
                        entryPoint = compute.EntryPoint
                    }
                })
            );
        }

        public void CreateComputePipelineAsync(string label, CreateComputePipelineAsyncCallback callback, ProgrammableStageDescriptor compute)
        {
            var context = new CallbackContext<Wgpu.CreatePipelineAsyncStatus, Wgpu.ComputePipelineImpl>
            {
                Delegate = (s, p, m, userData) => callback?.Invoke(s, new ComputePipeline(p), m),
                UserData = IntPtr.Zero,
            };

            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    DeviceCreateComputePipelineAsync(Impl, new ComputePipelineDescriptor
                    {
                        label = label,
                        compute = new Wgpu.ProgrammableStageDescriptor
                        {
                            module = compute.Module.Impl,
                            entryPoint = compute.EntryPoint
                        }
                    }, &CreateComputePipelineAsyncCallback, (void*)GCHandle.ToIntPtr(handle)
                    );
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void CreateComputePipelineAsyncCallback(Wgpu.CreatePipelineAsyncStatus status, Wgpu.ComputePipelineImpl impl, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (CallbackContext<CreatePipelineAsyncStatus, ComputePipelineImpl>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(status, impl, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }


        public PipelineLayout CreatePipelineLayout(string label, BindGroupLayout[] bindGroupLayouts)
        {
            Span<BindGroupLayoutImpl> bindGroupLayoutsInner = stackalloc BindGroupLayoutImpl[bindGroupLayouts.Length];

            for (int i = 0; i < bindGroupLayouts.Length; i++)
                bindGroupLayoutsInner[i] = bindGroupLayouts[i].Impl;

            unsafe
            {
                fixed (BindGroupLayoutImpl* pBindGroup = bindGroupLayoutsInner)
                {
                    return new PipelineLayout(
                        DeviceCreatePipelineLayout(Impl, new PipelineLayoutDescriptor
                        {
                            label = label,
                            bindGroupLayouts = pBindGroup,
                            bindGroupLayoutCount = (uint)bindGroupLayouts.Length
                        })
                    );
                }
            }
        }

        public QuerySet CreateQuerySet(string label, QueryType queryType, uint count, PipelineStatisticName[] pipelineStatistics)
        {
            unsafe
            {
                fixed (PipelineStatisticName* pipelineStatisticsPtr = pipelineStatistics)
                {
                    return new QuerySet(
                        DeviceCreateQuerySet(Impl, new QuerySetDescriptor
                        {
                            label = label,
                            type = queryType,
                            count = count,
                            pipelineStatistics = pipelineStatisticsPtr,
                            pipelineStatisticsCount = (uint)pipelineStatistics.Length
                        })
                    );
                }
            }
        }

        public RenderBundleEncoder CreateRenderBundleEncoder(string label, TextureFormat[] colorFormats, TextureFormat depthStencilFormat,
            uint sampleCount, bool depthReadOnly, bool stencilReadOnly)
        {
            unsafe
            {
                fixed (TextureFormat* colorFormatsPtr = colorFormats)
                {
                    return new RenderBundleEncoder(
                        DeviceCreateRenderBundleEncoder(Impl, new RenderBundleEncoderDescriptor
                        {
                            label = label,
                            colorFormats = colorFormatsPtr,
                            colorFormatsCount = (uint)colorFormats.Length,
                            depthStencilFormat = depthStencilFormat,
                            sampleCount = sampleCount,
                            depthReadOnly = depthReadOnly,
                            stencilReadOnly = stencilReadOnly
                        })
                    );
                }
            }
        }

        public RenderPipeline CreateRenderPipeline(string label, PipelineLayout layout,
            VertexState vertexState, PrimitiveState primitiveState, MultisampleState multisampleState,
            DepthStencilState? depthStencilState = null, FragmentState? fragmentState = null)
        {
            RenderPipelineDescriptor desc = CreateRenderPipelineDescriptor(label, layout, vertexState, primitiveState, multisampleState, depthStencilState, fragmentState);
            RenderPipelineImpl pipelineImpl = DeviceCreateRenderPipeline(Impl, desc);

            FreeRenderPipelineDescriptor(desc);
            return new RenderPipeline(pipelineImpl);
        }

        public void CreateRenderPipelineAsync(string label, CreateRenderPipelineAsyncCallback callback, PipelineLayout layout,
            VertexState vertexState, PrimitiveState primitiveState, MultisampleState multisampleState,
            DepthStencilState? depthStencilState = null, FragmentState? fragmentState = null)
        {
            RenderPipelineDescriptor desc = CreateRenderPipelineDescriptor(label, layout, vertexState, primitiveState, multisampleState, depthStencilState, fragmentState);
            var context = new CallbackContext<Wgpu.CreatePipelineAsyncStatus, RenderPipelineImpl>
            {
                Delegate = (s, p, m, userData) =>
                {
                    FreeRenderPipelineDescriptor(desc);
                    callback?.Invoke(s, new RenderPipeline(p), m);
                },
                UserData = IntPtr.Zero,
            };
            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    DeviceCreateRenderPipelineAsync(Impl, desc, &CreateRenderPipelineAsyncCallback, (void*)GCHandle.ToIntPtr(handle));
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void CreateRenderPipelineAsyncCallback(CreatePipelineAsyncStatus status, RenderPipelineImpl impl, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (CallbackContext<CreatePipelineAsyncStatus, RenderPipelineImpl>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(status, impl, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        private static RenderPipelineDescriptor CreateRenderPipelineDescriptor(string label, PipelineLayout layout, VertexState vertexState,
            PrimitiveState primitiveState, MultisampleState multisampleState, DepthStencilState? depthStencilState,
            FragmentState? fragmentState)
        {
            unsafe
            {
                return new RenderPipelineDescriptor
                {
                    label = label,
                    layout = layout.Impl,
                    vertex = new Wgpu.VertexState
                    {
                        module = vertexState.Module.Impl,
                        entryPoint = vertexState.EntryPoint,
                        buffers = (Wgpu.VertexBufferLayout*)Util.AllocHArray(vertexState.bufferLayouts.Length,
                            vertexState.bufferLayouts.Select(x => new Wgpu.VertexBufferLayout
                            {
                                arrayStride = x.ArrayStride,
                                stepMode = x.StepMode,
                                attributes = (VertexAttribute*)Util.AllocHArray(x.Attributes),
                                attributeCount = (uint)x.Attributes.Length
                            })
                        ),
                        bufferCount = (uint)vertexState.bufferLayouts.Length
                    },
                    primitive = primitiveState,
                    depthStencil = (DepthStencilState*)Util.Optional(depthStencilState),
                    multisample = multisampleState,
                    fragment = fragmentState == null ? null : (Wgpu.FragmentState*)Util.AllocHStruct(new Wgpu.FragmentState
                    {
                        module = fragmentState.Value.Module.Impl,
                        entryPoint = fragmentState.Value.EntryPoint,
                        targets = (Wgpu.ColorTargetState*)Util.AllocHArray(fragmentState.Value.colorTargets.Length,
                            fragmentState.Value.colorTargets.Select(x => new Wgpu.ColorTargetState
                            {
                                format = x.Format,
                                blend = (BlendState*)Util.Optional(x.BlendState),
                                writeMask = x.WriteMask
                            })
                        ),
                        targetCount = (uint)fragmentState.Value.colorTargets.Length
                    })
                };
            }
        }

        private static void FreeRenderPipelineDescriptor(RenderPipelineDescriptor descriptor)
        {
            unsafe
            {
                Wgpu.VertexBufferLayout* buffers = (Wgpu.VertexBufferLayout*)descriptor.vertex.buffers;

                for (ulong i = 0; i < descriptor.vertex.bufferCount; i++)
                    Util.FreePtr((nint)buffers[i].attributes);

                Util.FreePtr((nint)descriptor.vertex.buffers);
                Util.FreePtr((nint)descriptor.depthStencil);

                if (descriptor.fragment == null)
                    return;

                Wgpu.FragmentState* fragment = (Wgpu.FragmentState*)descriptor.fragment;
                Wgpu.ColorTargetState* targets = (Wgpu.ColorTargetState*)fragment->targets;

                for (ulong i = 0; i < fragment->targetCount; i++)
                    Util.FreePtr((nint)targets[i].blend);

                Util.FreePtr((nint)fragment->targets);
                Util.FreePtr((nint)descriptor.fragment);
            }
        }

        public Sampler CreateSampler(string label, AddressMode addressModeU, AddressMode addressModeV, AddressMode addressModeW,
            FilterMode magFilter, FilterMode minFilter, MipmapFilterMode mipmapFilter,
            float lodMinClamp, float lodMaxClamp, CompareFunction compare, ushort maxAnisotropy)
        {
            return new Sampler(
                DeviceCreateSampler(Impl, new SamplerDescriptor
                {
                    label = label,
                    addressModeU = addressModeU,
                    addressModeV = addressModeV,
                    addressModeW = addressModeW,
                    magFilter = magFilter,
                    minFilter = minFilter,
                    mipmapFilter = mipmapFilter,
                    lodMinClamp = lodMinClamp,
                    lodMaxClamp = lodMaxClamp,
                    compare = compare,
                    maxAnisotropy = maxAnisotropy
                })
            );
        }

        public ShaderModule CreateSprivShaderModule(string label, byte[] spirvCode)
        {
            unsafe
            {
                return new ShaderModule(
                    DeviceCreateShaderModule(Impl, new ShaderModuleDescriptor
                    {
                        label = label,
                        nextInChain = (Wgpu.ChainedStruct*)new WgpuStructChain()
                    .AddShaderModuleSPIRVDescriptor(spirvCode)
                    .GetPointer()
                    })
                );
            }
        }

        public ShaderModule CreateWgslShaderModule(string label, string wgslCode)
        {
            unsafe
            {
                return new ShaderModule(
                    DeviceCreateShaderModule(Impl, new ShaderModuleDescriptor
                    {
                        label = label,
                        nextInChain = (Wgpu.ChainedStruct*)new WgpuStructChain()
                        .AddShaderModuleWGSLDescriptor(wgslCode)
                        .GetPointer()
                    })
                );
            }
        }

        public ShaderModule CreateGlslShaderModule(string label, string vertCode, string fragCode, Wgpu.ShaderDefine[] defines)
        {
            unsafe
            {
                return new ShaderModule(DeviceCreateShaderModule(Impl, new ShaderModuleDescriptor
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                        .AddShaderModuleGLSLDescriptor(vertCode, fragCode, defines)
                        .GetPointer()
                })
                );
            }
        }
        public SwapChain CreateSwapChain(string label, Surface surface, TextureUsage usage,
            TextureFormat format, uint width, uint height, PresentMode presentMode)
        {
            return new SwapChain(
                DeviceCreateSwapChain(Impl, surface.Impl,
                new SwapChainDescriptor
                {
                    label = label,
                    usage = (uint)usage,
                    format = format,
                    width = width,
                    height = height,
                    presentMode = presentMode
                })
            );
        }

        public SwapChain CreateSwapChain(Surface surface, in SwapChainDescriptor descriptor)
        {
            return new SwapChain(
                DeviceCreateSwapChain(Impl, surface.Impl, descriptor)
            );
        }

        public Texture CreateTexture(string label, TextureUsage usage,
            TextureDimension dimension, Extent3D size, TextureFormat format,
            uint mipLevelCount, uint sampleCount)
        {
            var desc = new TextureDescriptor
            {
                label = label,
                usage = (uint)usage,
                dimension = dimension,
                size = size,
                format = format,
                mipLevelCount = mipLevelCount,
                sampleCount = sampleCount,
            };

            return CreateTexture(in desc);
        }

        public Texture CreateTexture(in TextureDescriptor descriptor)
        {
            return new Texture(DeviceCreateTexture(Impl, descriptor), descriptor);
        }

        public unsafe FeatureName[] EnumerateFeatures()
        {
            FeatureName features = default;

            ulong size = DeviceEnumerateFeatures(Impl, ref features);

            var featuresSpan = new Span<FeatureName>(Unsafe.AsPointer(ref features), (int)size);

            FeatureName[] result = new FeatureName[size];

            featuresSpan.CopyTo(result);

            return result;
        }

        public bool GetLimits(out SupportedLimits limits)
        {
            limits = new SupportedLimits();

            return DeviceGetLimits(Impl, ref limits);
        }

        public bool HasFeature(FeatureName feature) => DeviceHasFeature(Impl, feature);

        public void PushErrorScope(ErrorFilter filter) => DevicePushErrorScope(Impl, filter);
        public void PopErrorScope(ErrorCallback callback)
        {
            var context = new Callback<ErrorType>
            {
                Delegate = (t, m, _) => callback?.Invoke(t, m),
                UserData = IntPtr.Zero,
            };
            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    DevicePopErrorScope(Impl,
                        &DevicePopErrorScopeCallback,
                        (void*)GCHandle.ToIntPtr(handle));
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void DevicePopErrorScopeCallback(Wgpu.ErrorType errorType, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (Callback<ErrorType>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(errorType, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        private static readonly List<Wgpu.ErrorCallback> s_errorCallbacks =
            new List<Wgpu.ErrorCallback>();

        public void SetUncapturedErrorCallback(ErrorCallback callback)
        {
            Wgpu.ErrorCallback errorCallback = (t, m, _) => callback(t, m);

            s_errorCallbacks.Add(errorCallback);

            var context = new Callback<ErrorType>
            {
                Delegate = (t, m, _) => callback?.Invoke(t, m),
                UserData = IntPtr.Zero,
            };
            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    DeviceSetUncapturedErrorCallback(Impl,
                        &DeviceSetUncapturedCallback, (void*)GCHandle.ToIntPtr(handle));
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void DeviceSetUncapturedCallback(Wgpu.ErrorType errorType, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (Callback<ErrorType>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(errorType, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        public void Dispose()
        {
            Queue.Dispose();
            Queue = null;

            DeviceDestroy(Impl);
            DeviceRelease(Impl);
            Impl = default;
        }
    }

    public delegate void ErrorCallback(ErrorType type, string message);
    public delegate void DeviceLostCallback(DeviceLostReason reason, string message);

    public delegate void CreateComputePipelineAsyncCallback(CreatePipelineAsyncStatus status, ComputePipeline pipeline, string message);
    public delegate void CreateRenderPipelineAsyncCallback(CreatePipelineAsyncStatus status, RenderPipeline pipeline, string message);
}
