using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct GaussianBlurVHUniform : IBuffer
    {
        public GaussianBlurVHUniform(Vector4D<float> pixelSize, float radius)
        {
            PixelSize = pixelSize;
            Radius = radius;
        }

        public Vector4D<float> PixelSize { get; }
        public float Radius { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(GaussianBlurVHUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<GaussianBlurVHUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
