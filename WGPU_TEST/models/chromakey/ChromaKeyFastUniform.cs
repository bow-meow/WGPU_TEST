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
    public struct ChromaKeyFastUniform : IBuffer
    {
        public ChromaKeyFastUniform(float keyHue, float spillEdgeDistance, float spillEdgeFeather, Vector3D<float> startDists, Vector3D<float> endDists) 
        {
            KeyHue = keyHue;
            SpillEdgeDistance = spillEdgeDistance;
            SpillEdgeFeather = spillEdgeFeather;
            StartDists = startDists;
            EndDists = endDists;
        }

        public float KeyHue { get; }
        public float SpillEdgeDistance { get; }
        public float SpillEdgeFeather { get; }
        public Vector3D<float> StartDists { get; }
        public Vector3D<float> EndDists { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyFastUniform) + sizeof(float) * 3);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyFastUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
