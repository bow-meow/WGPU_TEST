using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct ExposureHighlightUniform : IBuffer
    {
        public ExposureHighlightUniform(float tolerance, Vector4D<float> newWhite, Vector4D<float> newBlack)
        {
            Tolerance = tolerance;
            NewWhite = newWhite;
            NewBlack = newBlack;
        }

        public float Tolerance { get; }
        public Vector4D<float> NewWhite { get; }
        public Vector4D<float> NewBlack { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ExposureHighlightUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ExposureHighlightUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
