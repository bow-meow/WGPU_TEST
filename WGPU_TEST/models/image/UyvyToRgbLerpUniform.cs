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
    public struct UyvyToRgbLerpUniform : IBuffer
    {
        public UyvyToRgbLerpUniform(Vector4D<float> quadScreenSize, float quadTexOffset)
        {
            QuadScreenSize = quadScreenSize;
            QuadTexOffset = quadTexOffset;
        }

        public Vector4D<float> QuadScreenSize { get; }
        public float QuadTexOffset { get; }


        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(UyvyToRgbLerpUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<UyvyToRgbLerpUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
