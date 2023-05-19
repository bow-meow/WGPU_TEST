using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct ChromaBlurVUniform : IBuffer
    {
        public ChromaBlurVUniform(Vector4D<float> pixelSize, int radiusV)
        {
            PixelSize = pixelSize;
            RadiusV = radiusV;
        }

        public Vector4D<float> PixelSize { get; }
        public int RadiusV { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaBlurVUniform) + sizeof(float) * 4);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaBlurVUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
