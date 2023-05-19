using Silk.NET.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public struct CameraBarrelFixUniform : IBuffer
    {
        public CameraBarrelFixUniform(float focalLength, float principalX, float principalY)
        {
            focal_length = focalLength;
            principal_x = principalX;
            principal_y = principalY;
        }

        public float focal_length { get; }
        public float principal_x { get; }
        public float principal_y { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)sizeof(CameraBarrelFixUniform);

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<CameraBarrelFixUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
