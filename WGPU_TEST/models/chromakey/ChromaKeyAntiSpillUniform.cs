using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGPU_TEST.models.core.filters;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.chromakey
{
    public struct ChromaKeyAntiSpillUniform : IBuffer
    {
        public ChromaKeyAntiSpillUniform(Vector3D<float> keyHSV, float edgeDistance, float edgeFeather)
        {
            KeyHSV = keyHSV;
            EdgeDistance = edgeDistance;
            EdgeFeather = edgeFeather;
        }

        public Vector3D<float> KeyHSV { get; }
        public float EdgeDistance { get; }
        public float EdgeFeather { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyAntiSpillUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyAntiSpillUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
