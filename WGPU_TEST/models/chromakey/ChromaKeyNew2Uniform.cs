using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using WGPU_TEST.models.core.filters;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.chromakey
{
    public struct ChromaKeyNew2Uniform : IBuffer
    {
        public ChromaKeyNew2Uniform(Vector3D<float> keyHSV, float hueDistance, float satDistance, float valDistance, float hueFeather, float hueFeather2, float satFeather, float valFeather)
        {
            KeyHSV = keyHSV;
            HueDistance = hueDistance;
            SatDistance = satDistance;
            ValDistance = valDistance;
            HueFeather = hueFeather;
            HueFeather2 = hueFeather2;
            SatFeather = satFeather;
            ValFeather = valFeather;
        }

        public Vector3D<float> KeyHSV { get; }
        public float HueDistance { get; }
        public float SatDistance { get; }
        public float ValDistance { get; }
        public float HueFeather { get; }
        public float SatFeather { get; }
        public float ValFeather { get; }
        public float HueFeather2 { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyNew2Uniform) + sizeof(float) * 5);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyNew2Uniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
