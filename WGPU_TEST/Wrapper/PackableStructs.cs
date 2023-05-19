using Microsoft.VisualBasic;
using Silk.NET.WebGPU;
using System;
using System.Buffers;
using System.Text;
using WGPU = Silk.NET.WebGPU;

namespace WgpuWrappersSilk.Net
{
    public unsafe struct ProgrammableStage
    {
        public ShaderModulePtr ShaderModule;
        public string EntryPoint;
        public (string key, double value)[] Constants;

        public ProgrammableStage(ShaderModulePtr shaderModule, string entryPoint,
            (string key, double value)[] constants)
        {
            ShaderModule = shaderModule;
            EntryPoint = entryPoint;
            Constants = constants;
        }
        
        internal int CalculatePayloadSize()
        {
            int size = Encoding.UTF8.GetByteCount(EntryPoint)+1;
            size += sizeof(ConstantEntry) * Constants.Length;
            for (int i = 0; i < Constants.Length; i++)
            {
                size += Encoding.UTF8.GetByteCount(Constants[i].key)+1;
            }
            return size;
        }

        internal int PackInto(ref ProgrammableStageDescriptor baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                var ptr = startPtr;

                var constants = (ConstantEntry*)ptr;
                ptr += sizeof(ConstantEntry) * Constants.Length;

                baseStruct.Module = ShaderModule;
                baseStruct.ConstantCount = (uint)Constants.Length;
                baseStruct.Constants = constants;

                baseStruct.EntryPoint = ptr;
                ptr += Encoding.UTF8.GetBytes(EntryPoint, payloadBuffer[(int)(ptr - startPtr)..])+1;

                for (int i = 0; i < Constants.Length; i++)
                {
                    constants[i].Key = ptr;
                    ptr += Encoding.UTF8.GetBytes(Constants[i].key, payloadBuffer[(int)(ptr - startPtr)..])+1;

                    constants[i].Value = Constants[i].value;
                }

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct VertexState
    {
        public ShaderModulePtr ShaderModule;
        public string EntryPoint;
        public (string key, double value)[] Constants;
        public VertexBufferLayout[] Buffers;

        public VertexState(ShaderModulePtr shaderModule, string entryPoint,
            (string key, double value)[] constants, VertexBufferLayout[] buffers)
        {
            ShaderModule = shaderModule;
            EntryPoint = entryPoint;
            Constants = constants;
            Buffers = buffers;
        }

        internal int CalculatePayloadSize()
        {
            int size = Encoding.UTF8.GetByteCount(EntryPoint)+1;
            size += sizeof(ConstantEntry) * Constants.Length;
            for (int i = 0; i < Constants.Length; i++)
                size += Encoding.UTF8.GetByteCount(Constants[i].key)+1;

            size += sizeof(Silk.NET.WebGPU.VertexBufferLayout) * Buffers.Length;
            for (int i = 0; i < Buffers.Length; i++)
                size += Buffers[i].CalculatePayloadSize();

            return size;
        }

        internal int PackInto(ref Silk.NET.WebGPU.VertexState baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                var ptr = startPtr;

                var constants = (ConstantEntry*)ptr;
                if(Constants.Length > 0)
                    ptr += sizeof(ConstantEntry) * Constants.Length;

                var buffers = (Silk.NET.WebGPU.VertexBufferLayout*)ptr;
                if (Buffers.Length > 0)
                    ptr += sizeof(Silk.NET.WebGPU.VertexBufferLayout) * Buffers.Length;



                baseStruct.Module = ShaderModule;
                baseStruct.ConstantCount = (uint)Constants.Length;
                baseStruct.Constants = Constants.Length == 0 ? null : constants;
                baseStruct.BufferCount = (uint)Buffers.Length;
                baseStruct.Buffers = Buffers.Length == 0 ? null :  buffers;

                baseStruct.EntryPoint = ptr;
                ptr += Encoding.UTF8.GetBytes(EntryPoint, payloadBuffer[(int)(ptr - startPtr)..])+1;

                for (int i = 0; i < Constants.Length; i++)
                {
                    constants[i].Key = ptr;
                    ptr += Encoding.UTF8.GetBytes(Constants[i].key, payloadBuffer[(int)(ptr - startPtr)..])+1;

                    constants[i].Value = Constants[i].value;
                }

                for (int i = 0; i < Buffers.Length; i++)
                {
                    var subBuffer = payloadBuffer.Slice((int)(ptr - startPtr));
                    ptr += Buffers[i].PackInto(ref buffers[i], subBuffer);
                }

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct VertexBufferLayout
    {
        public ulong ArrayStride;
        public VertexStepMode StepMode;
        public VertexAttribute[] Attributes;

        public VertexBufferLayout(ulong arrayStride, VertexStepMode stepMode, VertexAttribute[] attributes)
        {
            this.ArrayStride = arrayStride;
            this.StepMode = stepMode;
            this.Attributes = attributes;
        }

        internal int CalculatePayloadSize()
        {
            return sizeof(VertexAttribute) * Attributes.Length;
        }

        internal int PackInto(ref Silk.NET.WebGPU.VertexBufferLayout baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                var ptr = startPtr;

                var attributes = (VertexAttribute*)ptr;
                ptr += sizeof(VertexAttribute) * Attributes.Length;

                baseStruct.ArrayStride = ArrayStride;
                baseStruct.StepMode = StepMode;
                baseStruct.AttributeCount = (uint)Attributes.Length;
                baseStruct.Attributes = attributes;

                var attribute = attributes;
                for (int i = 0; i < Attributes.Length; i++)
                    attributes[i] = Attributes[i];

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct FragmentState
    {
        public ShaderModulePtr ShaderModule;
        public string EntryPoint;
        public (string key, double value)[] Constants;
        public ColorTargetState[] ColorTargets;

        public FragmentState(ShaderModulePtr shaderModule, string entryPoint,
            (string key, double value)[] constants, ColorTargetState[] colorTargets)
        {
            ShaderModule = shaderModule;
            EntryPoint = entryPoint;
            Constants = constants;
            ColorTargets = colorTargets;
        }

        internal int CalculatePayloadSize()
        {
            int size = Encoding.UTF8.GetByteCount(EntryPoint)+1;
            size += sizeof(ConstantEntry) * Constants.Length;
            for (int i = 0; i < Constants.Length; i++)
                size += Encoding.UTF8.GetByteCount(Constants[i].key)+1;

            size += sizeof(Silk.NET.WebGPU.ColorTargetState) * ColorTargets.Length;
            for (int i = 0; i < ColorTargets.Length; i++)
                size += ColorTargets[i].CalculatePayloadSize();

            return size;
        }

        internal int PackInto(ref Silk.NET.WebGPU.FragmentState baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                var ptr = startPtr;

                var constants = (ConstantEntry*)ptr;
                if(Constants.Length > 0)
                    ptr += sizeof(ConstantEntry) * Constants.Length;

                var targets = (Silk.NET.WebGPU.ColorTargetState*)ptr;
                ptr += sizeof(Silk.NET.WebGPU.ColorTargetState) * ColorTargets.Length;

                baseStruct.Module = ShaderModule;
                baseStruct.ConstantCount = (uint)Constants.Length;
                baseStruct.Constants = Constants.Length == 0 ? null : constants;
                baseStruct.TargetCount = (uint)ColorTargets.Length;
                baseStruct.Targets = targets;

                baseStruct.EntryPoint = ptr;
                ptr += Encoding.UTF8.GetBytes(EntryPoint, payloadBuffer[(int)(ptr - startPtr)..])+1;

                for (int i = 0; i < Constants.Length; i++)
                {
                    constants[i].Key = ptr;
                    ptr += Encoding.UTF8.GetBytes(Constants[i].key, payloadBuffer[(int)(ptr - startPtr)..])+1;

                    constants[i].Value = Constants[i].value;
                }

                for (int i = 0; i < ColorTargets.Length; i++)
                {
                    var subBuffer = payloadBuffer.Slice((int)(ptr - startPtr));
                    ptr += ColorTargets[i].PackInto(ref targets[i], subBuffer);
                }

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct ColorTargetState
    {
        public TextureFormat Format;
        public (BlendComponent color, BlendComponent alpha)? BlendState;
        public ColorWriteMask WriteMask;


        public ColorTargetState(TextureFormat format, (BlendComponent color, BlendComponent alpha)? blendState, ColorWriteMask writeMask)
        {
            Format = format;
            BlendState = blendState;
            WriteMask = writeMask;
        }

        internal int CalculatePayloadSize()
        {
            return BlendState.HasValue ? sizeof(BlendState): 0;
        }

        internal int PackInto(ref Silk.NET.WebGPU.ColorTargetState baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                var ptr = startPtr;

                baseStruct.Format = Format;
                baseStruct.WriteMask = WriteMask;

                if (BlendState.HasValue)
                {
                    var blendState = (BlendState*)ptr;
                    ptr += sizeof(BlendState);
                    blendState[0].Color = BlendState.Value.color;
                    blendState[0].Alpha = BlendState.Value.alpha;
                    baseStruct.Blend = blendState;
                }

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct ShaderModuleCompilationHint
    {
        public string EntryPoint;
        public PipelineLayoutPtr PipelineLayout;

        public ShaderModuleCompilationHint(string entryPoint, PipelineLayoutPtr pipelineLayout)
        {
            EntryPoint = entryPoint;
            PipelineLayout = pipelineLayout;
        }

        internal int CalculatePayloadSize()
        {
            return Encoding.UTF8.GetByteCount(EntryPoint)+1;
        }

        internal int PackInto(ref Silk.NET.WebGPU.ShaderModuleCompilationHint baseStruct, Span<byte> payloadBuffer)
        {
            int payloadSize;

            fixed(byte* startPtr = &payloadBuffer[0])
            {
                baseStruct.Layout = PipelineLayout;

                var ptr = startPtr;

                baseStruct.EntryPoint = ptr;
                ptr += Encoding.UTF8.GetBytes(EntryPoint, payloadBuffer[(int)(ptr - startPtr)..])+1;

                payloadSize = (int)(ptr - startPtr);
            }
            return payloadSize;
        }
    }

    public unsafe struct ImageCopyBuffer
    {
        public BufferPtr Buffer;
        
        public TextureDataLayout Layout;

        public ImageCopyBuffer(BufferPtr buffer, TextureDataLayout layout)
        {
            Buffer = buffer;
            Layout = layout;
        }

        internal Silk.NET.WebGPU.ImageCopyBuffer Pack()
        {
            return new Silk.NET.WebGPU.ImageCopyBuffer 
            {
                Buffer = Buffer,
                Layout = Layout,
            };
        }
    }

    public unsafe struct ImageCopyTexture
    {
        public TexturePtr Texture;

        public uint MipLevel;

        public Origin3D Origin;

        public TextureAspect Aspect;

        public ImageCopyTexture(TexturePtr texture, uint mipLevel, Origin3D origin, TextureAspect aspect)
        {
            Texture = texture;
            MipLevel = mipLevel;
            Origin = origin;
            Aspect = aspect;
        }

        internal Silk.NET.WebGPU.ImageCopyTexture Pack()
        {
            return new Silk.NET.WebGPU.ImageCopyTexture 
            {
                Texture = Texture,
                MipLevel = MipLevel,
                Origin = Origin,
                Aspect = Aspect,
            };
        }
    }

    public unsafe struct ComputePassTimestampWrite
    {
        public ComputePassTimestampLocation Location;
        public uint QueryIndex;
        public QuerySetPtr QuerySet;

        public ComputePassTimestampWrite(ComputePassTimestampLocation location, uint queryIndex, QuerySetPtr querySet)
        {
            Location = location;
            QueryIndex = queryIndex;
            QuerySet = querySet;
        }

        internal Silk.NET.WebGPU.ComputePassTimestampWrite Pack()
        {
            return new Silk.NET.WebGPU.ComputePassTimestampWrite
            {
                Location = Location,
                QueryIndex = QueryIndex,
                QuerySet = QuerySet
            };
        }
    }

    public unsafe struct RenderPassTimestampWrite
    {
        public RenderPassTimestampLocation Location;
        public uint QueryIndex;
        public QuerySetPtr QuerySet;

        public RenderPassTimestampWrite(RenderPassTimestampLocation location, uint queryIndex, QuerySetPtr querySet)
        {
            Location = location;
            QueryIndex = queryIndex;
            QuerySet = querySet;
        }

        internal Silk.NET.WebGPU.RenderPassTimestampWrite Pack()
        {
            return new Silk.NET.WebGPU.RenderPassTimestampWrite
            {
                Location = Location,
                QueryIndex = QueryIndex,
                QuerySet = QuerySet
            };
        }
    }

    public unsafe partial struct RenderPassColorAttachment
    {
        public TextureViewPtr View;
        public TextureViewPtr ResolveTarget;
        public LoadOp LoadOp;
        public StoreOp StoreOp;
        public Color ClearValue;

        public RenderPassColorAttachment(TextureViewPtr view, TextureViewPtr resolveTarget, LoadOp loadOp, StoreOp storeOp, Color clearValue)
        {
            View = view;
            ResolveTarget = resolveTarget;
            LoadOp = loadOp;
            StoreOp = storeOp;
            ClearValue = clearValue;
        }

        internal Silk.NET.WebGPU.RenderPassColorAttachment Pack()
        {
            return new Silk.NET.WebGPU.RenderPassColorAttachment
            {
                View = View,
                ResolveTarget = ResolveTarget,
                LoadOp = LoadOp,
                StoreOp = StoreOp,
                ClearValue = ClearValue
            };
        }
    }

    public unsafe partial struct RenderPassDepthStencilAttachment
    {       
        public TextureViewPtr View;
        public LoadOp DepthLoadOp;
        public StoreOp DepthStoreOp;
        public float DepthClearValue;
        public bool DepthReadOnly;
        public LoadOp StencilLoadOp;
        public StoreOp StencilStoreOp;
        public uint StencilClearValue;
        public bool StencilReadOnly;

        public RenderPassDepthStencilAttachment(TextureViewPtr view, 
            LoadOp depthLoadOp, StoreOp depthStoreOp, float depthClearValue, bool depthReadOnly, 
            LoadOp stencilLoadOp, StoreOp stencilStoreOp, uint stencilClearValue, bool stencilReadOnly)
        {
            View = view;
            DepthLoadOp = depthLoadOp;
            DepthStoreOp = depthStoreOp;
            DepthClearValue = depthClearValue;
            DepthReadOnly = depthReadOnly;
            StencilLoadOp = stencilLoadOp;
            StencilStoreOp = stencilStoreOp;
            StencilClearValue = stencilClearValue;
            StencilReadOnly = stencilReadOnly;
        }

        internal Silk.NET.WebGPU.RenderPassDepthStencilAttachment Pack()
        {
            return new Silk.NET.WebGPU.RenderPassDepthStencilAttachment
            {
                View = View,
                DepthLoadOp = DepthLoadOp,
                DepthStoreOp = DepthStoreOp,
                DepthClearValue = DepthClearValue,
                DepthReadOnly = DepthReadOnly,
                StencilLoadOp = StencilLoadOp,
                StencilStoreOp = StencilStoreOp,
                StencilClearValue = StencilClearValue,
                StencilReadOnly = StencilReadOnly
            };
        }
    }
}
