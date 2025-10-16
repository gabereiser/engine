﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class RenderPassEncoder : IDisposable
    {
        private RenderPassEncoderImpl _impl;

        internal RenderPassEncoder(RenderPassEncoderImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderPassEncoder));

            _impl = impl;
        }

        public void BeginOcclusionQuery(uint queryIndex)
            => RenderPassEncoderBeginOcclusionQuery(_impl, queryIndex);

        public void BeginPipelineStatisticsQuery(QuerySet querySet, uint queryIndex)
            => RenderPassEncoderBeginPipelineStatisticsQuery(_impl, querySet.Impl, queryIndex);

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
            => RenderPassEncoderDraw(_impl, vertexCount, instanceCount, firstVertex, firstInstance);

        public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance)
            => RenderPassEncoderDrawIndexed(_impl, indexCount, instanceCount, firstIndex, baseVertex, firstInstance);

        public void DrawIndexedIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderPassEncoderDrawIndexedIndirect(_impl, indirectBuffer.Impl, indirectOffset);

        public void DrawIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderPassEncoderDrawIndirect(_impl, indirectBuffer.Impl, indirectOffset);

        public void End() => RenderPassEncoderEnd(_impl);

        public void EndOcclusionQuery() => RenderPassEncoderEndOcclusionQuery(_impl);

        public void EndPipelineStatisticsQuery() => RenderPassEncoderEndPipelineStatisticsQuery(_impl);

        public unsafe void ExecuteBundles(RenderBundle[] bundles)
        {
            Span<RenderBundleImpl> innerBundles = stackalloc RenderBundleImpl[bundles.Length];

            for (int i = 0; i < bundles.Length; i++)
                innerBundles[i] = bundles[i].Impl;

            RenderPassEncoderExecuteBundles(_impl, (uint)bundles.Length, ref innerBundles.GetPinnableReference());
        }

        public void InsertDebugMarker(string markerLabel)
            => RenderPassEncoderInsertDebugMarker(_impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => RenderPassEncoderPushDebugGroup(_impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => RenderPassEncoderPopDebugGroup(_impl);

        public unsafe void SetBindGroup(uint groupIndex, BindGroup group, uint[] dynamicOffsets)
        {
            fixed (uint* dynamicOffsetsPtr = dynamicOffsets)
            {
                RenderPassEncoderSetBindGroup(_impl, groupIndex,
                    group.Impl,
                    (uint)dynamicOffsets.Length,
                    ref Unsafe.AsRef<uint>(dynamicOffsetsPtr)
                );
            }
        }

        public void SetBlendConstant(in Color color) => RenderPassEncoderSetBlendConstant(_impl, color);

        public void SetIndexBuffer(Buffer buffer, IndexFormat format, ulong offset, ulong size)
            => RenderPassEncoderSetIndexBuffer(_impl, buffer.Impl, format, offset, size);

        public void SetPipeline(RenderPipeline pipeline) => RenderPassEncoderSetPipeline(_impl, pipeline.Impl);

        public void SetPushConstants<T>(ShaderStage stages, uint offset, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            unsafe
            {
                fixed (void* dataPtr = data)
                {
                    RenderPassEncoderSetPushConstants(
                                   _impl, (uint)stages, offset, (uint)(data.Length * sizeof(T)),
                                   dataPtr
                               );
                }
            }
        }

        public void SetScissorRect(uint x, uint y, uint width, uint height)
            => RenderPassEncoderSetScissorRect(_impl, x, y, width, height);

        public void SetStencilReference(uint reference) => RenderPassEncoderSetStencilReference(_impl, reference);

        public void SetVertexBuffer(uint slot, Buffer buffer, ulong offset, ulong size)
            => RenderPassEncoderSetVertexBuffer(_impl, slot, buffer.Impl, offset, size);

        public void SetViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
            => RenderPassEncoderSetViewport(_impl, x, y, width, height, minDepth, maxDepth);

        public void Dispose()
        {
            RenderPassEncoderRelease(_impl);
            _impl = default;
        }
    }
}
