using System;
using Red.Common;
using Red.Platform;
using Red.Platform.WGPU;
using Red.Platform.WGPU.Wrappers;

namespace Red.Graphics
{
    public class RenderPass
    {
        private RenderPassEncoder renderPassEncoder;
        private CommandEncoder encoder;


        private Wgpu.TextureDescriptor depthBufferDescriptor;
        private readonly RenderPassColorAttachment renderPassColorAttachment;
        private readonly RenderPassDepthStencilAttachment renderPassDepthAttachment;

        internal RenderPass()
        {


            renderPassColorAttachment = new RenderPassColorAttachment()
            {
                clearValue = new Wgpu.Color() { r = 0.01, g = 0.01, b = 0.01, a = 0.1 },
                loadOp = Wgpu.LoadOp.Clear,
                storeOp = Wgpu.StoreOp.Store,
                view = GPU.BackBuffer,
                resolveTarget = null
            };

            renderPassDepthAttachment = new RenderPassDepthStencilAttachment()
            {
                DepthClearValue = 1.0f,
                DepthLoadOp = Wgpu.LoadOp.Clear,
                DepthStoreOp = Wgpu.StoreOp.Store,
                DepthReadOnly = false,
                StencilClearValue = 0,
                StencilLoadOp = Wgpu.LoadOp.Clear,
                StencilStoreOp = Wgpu.StoreOp.Store,
                StencilReadOnly = false,
                View = GPU.DepthBufferView,
            };
        }

        public unsafe void Begin()
        {



            Log.Debug("FRAME BEGIN");
            encoder = GPU.Device.CreateCommandEncoder(null);



            renderPassEncoder = encoder.BeginRenderPass(null, new[] { renderPassColorAttachment }, renderPassDepthAttachment);

        }

        public void End()
        {
            renderPassEncoder.End();

            encoder.InsertDebugMarker("FRAME");

            var commandBuffer = encoder.Finish("FRAME");
            GPU.Queue.Submit(new[] { commandBuffer });

            Log.Debug("FRAME END");
        }

        public void Dispose()
        {
            //BackBuffer.Dispose(); // this is disposed on End

            if (encoder != null)
                encoder.Dispose();

        }
    }
}