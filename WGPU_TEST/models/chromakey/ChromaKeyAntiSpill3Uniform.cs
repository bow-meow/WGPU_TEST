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
    public struct ChromaKeyAntiSpill3Uniform : IBuffer
    {
        public ChromaKeyAntiSpill3Uniform(Vector3D<float> keyHSV, float edgeDistance, float edgeFeather, float decrease)
        {
            KeyHSV = keyHSV;
            EdgeDistance = edgeDistance;
            EdgeFeather = edgeFeather;
            Decrease = decrease;
        }

        public Vector3D<float> KeyHSV { get; }
        public float EdgeDistance { get; }
        public float EdgeFeather { get; }
        public float Decrease { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyAntiSpill3Uniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyAntiSpill3Uniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
