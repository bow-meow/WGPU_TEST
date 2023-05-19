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
    public struct Yuy2ToRgbUniform : IBuffer
    {
        public Yuy2ToRgbUniform(float textureAspect, Vector2D<float> textureScale, Vector4D<float> quadScreenSize)
        {
            TextureAspect = textureAspect;
            TextureScale = textureScale;
            QuadScreenSize = quadScreenSize;
        }

        public float TextureAspect { get; }
        public Vector2D<float> TextureScale { get; }
        public Vector4D<float> QuadScreenSize { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(Yuy2ToRgbUniform) + sizeof(float));

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<Yuy2ToRgbUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
