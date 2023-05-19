﻿using Silk.NET.Maths;
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
    public struct UyvyToRgbUniform : IBuffer
    {
        public UyvyToRgbUniform(Vector4D<float> quadScreenSize)
        {
            QuadScreenSize = quadScreenSize;
        }

        public Vector4D<float> QuadScreenSize { get; }

        public unsafe (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device)
        {
            var buffer_size = (ulong)(sizeof(UyvyToRgbUniform));

            var buffer = device.CreateBuffer(BufferUsage.Uniform | BufferUsage.CopyDst, buffer_size);

            var queue = device.GetQueue();

            queue.WriteBuffer(buffer, 0, new ReadOnlySpan<UyvyToRgbUniform>(new[] { this }));

            return (buffer, buffer_size);
        }
    }
}
