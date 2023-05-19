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
    public struct DilateUniform : IBuffer
    {
        public DilateUniform(Vector2D<float> pixelSize, float strength)
        {
            PixelSize = pixelSize;
            Strength = strength;
        }

        public Vector2D<float> PixelSize { get; }
        public float Strength { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(DilateUniform) + sizeof(float));

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<DilateUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
