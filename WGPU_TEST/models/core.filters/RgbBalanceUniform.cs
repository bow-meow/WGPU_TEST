using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct RgbBalanceUniform : IBuffer
    {
        public RgbBalanceUniform(float red, float green, float blue)
        {
            Red = red;
            Blue = blue;
            Green = green;
        }

        public float Red { get; }
        public float Green { get; }
        public float Blue { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)sizeof(RgbBalanceUniform);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<RgbBalanceUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
