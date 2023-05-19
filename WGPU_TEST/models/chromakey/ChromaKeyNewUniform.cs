using Silk.NET.Maths;
using Silk.NET.WebGPU;
using System;
using WGPU_TEST.models.core.filters;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.chromakey
{
    public struct ChromaKeyNewUniform : IBuffer
    {
        public ChromaKeyNewUniform(Vector3D<float> keyHSV, float hueDistance, float satDistance, float valDistance, float hueFeather, float hueFeather2, float satFeather, float valFeather, bool isAlphaPremultiplied) 
        {
            KeyHSV = keyHSV;
            HueDistance = hueDistance;
            SatDistance = satDistance;
            ValDistance = valDistance;
            HueFeather = hueFeather;
            HueFeather2 = hueFeather2;
            SatFeather = satFeather;
            ValFeather = valFeather;
            IsAlphaPremultiplied = isAlphaPremultiplied ? 1 : 0;

        }

        public Vector3D<float> KeyHSV { get; }
        public float HueDistance { get; }
        public float SatDistance { get; }
        public float ValDistance { get; }
        public float HueFeather { get; }
        public float SatFeather { get; }
        public float ValFeather { get; }
        public float HueFeather2 { get; }
        public int IsAlphaPremultiplied { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(ChromaKeyNewUniform) + sizeof(float) * 5);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<ChromaKeyNewUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
