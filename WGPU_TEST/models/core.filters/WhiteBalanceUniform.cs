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
    public struct WhiteBalanceUniform : IBuffer
    {
        public WhiteBalanceUniform(float red, float green, float blue, float strength, float keepWhite, Vector2D<float> refD65)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Strength = strength;
            KeepWhite = keepWhite;
            RefD65 = refD65;
        }

        public float Red { get; }
        public float Green { get; }
        public float Blue { get; }
        public float Strength { get; }
        public float KeepWhite { get; }
        public Vector2D<float> RefD65 { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(WhiteBalanceUniform) + sizeof(float) * 5);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<WhiteBalanceUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
