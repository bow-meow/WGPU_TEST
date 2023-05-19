using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGPU_TEST.models.core.filters;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.image
{
    public struct TiledBackgroundUniform : IBuffer
    {
        public TiledBackgroundUniform(Vector2D<float> textureScale) 
        {
            TextureScale = textureScale;
        }

        public Vector2D<float> TextureScale { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(TiledBackgroundUniform));

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<TiledBackgroundUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
