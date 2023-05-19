using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct LevelsUniform : IBuffer
    {
        public LevelsUniform(float inBlack, float inWhite, float inGamma, float outWhite, float outBlack)
        {
            InBlack = inBlack;
            InWhite = inWhite;
            InGamma = inGamma;
            OutWhite = outWhite;
            OutBlack = outBlack;
        }

        public float InBlack { get; }
        public float InWhite { get; }
        public float InGamma { get; }
        public float OutWhite { get; }
        public float OutBlack { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)sizeof(LevelsUniform);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<LevelsUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
