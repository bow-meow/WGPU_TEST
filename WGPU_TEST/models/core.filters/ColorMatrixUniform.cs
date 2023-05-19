using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct ColorMatrixUniform : IBuffer
    {
        public ColorMatrixUniform(Matrix4X4<float> colorMatrix)
        {
            ColorMatrix = colorMatrix;
        }

        public Matrix4X4<float> ColorMatrix { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)sizeof(ColorMatrixUniform);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ColorMatrixUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
