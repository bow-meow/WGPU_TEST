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
    public struct SharpenUniform : IBuffer
    {
        public SharpenUniform(Vector4D<float> pixelSize, float factor)
        {
            PixelSize = pixelSize;
            Factor = factor;
        }

        public Vector4D<float> PixelSize { get; }
        public float Factor { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(SharpenUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<SharpenUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
