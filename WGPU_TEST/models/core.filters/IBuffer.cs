using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WgpuWrappersSilk.Net;

namespace WGPU_TEST.models.core.filters
{
    public interface IBuffer
    {
        (BufferPtr, ulong buffer_size) CreateBuffer(DevicePtr device);
    }
}
