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
    public struct ChromaBlurHUniform : IBuffer
    {
        public ChromaBlurHUniform(Vector4D<float> pixelSize, int radiusH)
        {
            this.pixelSize = pixelSize;
            this.radiusH = radiusH;
        }

        public Vector4D<float> pixelSize { get; }
        public int radiusH { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaBlurHUniform) + sizeof(float) * 4);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaBlurHUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
