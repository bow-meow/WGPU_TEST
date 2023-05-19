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
    public struct ChromaKeyUnifrom : IBuffer
    {
        public ChromaKeyUnifrom(Vector3D<float> keyHSV, float edgeFeather, float edgeDistance, float spillEdgeFeather, float spillEdgeDistance, float spillLuma, float spillAmount)
        {
            KeyHSV = keyHSV;
            EdgeFeather = edgeFeather;
            EdgeDistance = edgeDistance;
            SpillEdgeFeather = spillEdgeFeather;
            SpillEdgeDistance = spillEdgeDistance;
            SpillLuma = spillLuma;
            SpillAmount = spillAmount;
        }

        public Vector3D<float> KeyHSV { get; }
        public float EdgeFeather { get; }
        public float EdgeDistance { get; }
        public float SpillEdgeFeather { get; }
        public float SpillEdgeDistance { get; }
        public float SpillLuma { get; }
        public float SpillAmount { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyUnifrom) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyUnifrom>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
