using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using Silk.NET.WebGPU;
using Silk.NET.Maths;
using Silk.NET.WebGPU.Extensions.WGPU;
using WgpuWrappersSilk.Net;
using SixLabors.ImageSharp.PixelFormats;
using SpinMe.Imaging.OpenGL.Processor;
using WGPU_TEST.models.chromakey;
using WGPU_TEST.models.image;
using WGPU_TEST.models.core.filters;
using System.Reflection;

namespace WGPU_TEST.Processor
{
    public class WgpuImageProcessor
    {
        public readonly struct Vertex
        {
            public Vertex(Vector3D<float> pos, Vector2D<float> texCoord)
            {
                position = pos;
                tex_coords = texCoord;
            }

            public Vector3D<float> position { get; }
            public Vector2D<float> tex_coords { get; }
        }

        Vertex[] quad_vert =
        {
                    // Positions  // Texture Coords
                    new Vertex(new Vector3D<float>(1.0f, 1.0f, 0.0f), new Vector2D<float>(1.0f, 1.0f - 1.0f)),
                    new Vertex(new Vector3D<float>(-1.0f, 1.0f, 0.0f), new Vector2D<float>(0.0f, 1.0f - 1.0f)),
                    new Vertex(new Vector3D<float>(-1.0f, -1.0f, 0.0f), new Vector2D<float>(0.0f, 1.0f - 0.0f)),
                    new Vertex(new Vector3D<float>(1.0f, -1.0f, 0.0f), new Vector2D<float>(1.0f, 1.0f -0.0f)),
                };

        uint[] quad_index_map =
        {
                    0, 1, 2,
                    3, 0, 2,
                };

        private Dictionary<ShaderType, ShaderModulePtr> _shaders = new Dictionary<ShaderType, ShaderModulePtr>();
        WebGPU wgpu;

        InstancePtr instance;
        AdapterPtr adapter;
        DevicePtr device;

        TexturePtr render_tex;
        TextureViewPtr render_tex_view;

        TexturePtr bind_tex;
        TextureViewPtr bind_tex_view;
        BindGroupLayoutPtr bind_tex_group_layout;
        BindGroupPtr bind_tex_group;

        BindGroupLayoutPtr uniform_bindgroup_layout;
        BindGroupPtr uniform_bindgroup;

        RenderPipelinePtr render_pipeline;

        BufferPtr _vertex_buffer;
        BufferPtr _index_buffer;
        BufferPtr _uniform_buffer;

        ulong _vertex_buffer_size;
        ulong _index_buffer_size;
        ulong _uniform_buffer_size;

        public AdapterProperties AdapterProperties { get; }

        public WgpuImageProcessor()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                wgpu = new WebGPU(WebGPU.CreateDefaultContext("libwgpu_native.dylib"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                wgpu = new WebGPU(WebGPU.CreateDefaultContext("wgpu_native.dll"));
            }
            else
                throw new Exception();

            instance = InstancePtr.Create(wgpu, default);

            adapter = instance.RequestAdapter(new RequestAdapterOptions
            {
                 PowerPreference = PowerPreference.HighPerformance
            }).Result;

            AdapterProperties = adapter.GetProperties();

            device = adapter.RequestDevice(default).Result;

            device.SetUncapturedErrorCallback(new WgpuWrappersSilk.Net.ErrorCallback((err, str) =>
            {
                var a = 1;
            }));

            unsafe
            {
                _vertex_buffer_size = (ulong)(sizeof(Vertex) * quad_vert.Length); // unsafe
                _index_buffer_size = (ulong)(sizeof(int) * quad_index_map.Length); // safe
            }


            _vertex_buffer = device.CreateBuffer(BufferUsage.Vertex | BufferUsage.CopyDst, _vertex_buffer_size, false);
            _index_buffer = device.CreateBuffer(BufferUsage.Index | BufferUsage.CopyDst, _index_buffer_size, false);

            var queue = device.GetQueue();

            queue.WriteBuffer<Vertex>(_vertex_buffer, 0, quad_vert.AsSpan());
            queue.WriteBuffer<uint>(_index_buffer, 0, quad_index_map.AsSpan());

            //{
            //    Span<Vertex> mapped = _vertex_buffer.GetMappedRange<Vertex>(0, (nuint)_vertex_buffer_size);

            //    quad_vert.CopyTo(mapped);

            //    _vertex_buffer.Unmap();
            //}



            //{
            //    Span<uint> mapped = _index_buffer.GetMappedRange<uint>(0, (nuint)_index_buffer_size);

            //    quad_index_map.CopyTo(mapped);

            //    _index_buffer.Unmap();
            //}


        }

        private unsafe void CreateUniform(ShaderType shaderType)
        {
            switch(shaderType)
            {
                //CORE
                case ShaderType.Border:
                    {
                        var border = new BorderUniform(new Vector4D<float>(0.0f, 0.0f, 1.0f, 1.0f), new Vector4D<float>(1.0f, 0.0f, 0.0f, 1.0f), 0, 0, 0, 0);

                        (_uniform_buffer, _uniform_buffer_size) = border.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Brightness_Contrast:
                    {
                        var brightness_contrast = new BrightnessContrastUniform(0.4f, 0.5f);

                        (_uniform_buffer, _uniform_buffer_size) = brightness_contrast.CreateBuffer(device);
                    }
                    break;
                case ShaderType.CameraBarrelFix:
                    {
                        var camera_barrel_fix = new CameraBarrelFixUniform(1.0f, 0.4f, 0.3f);

                        (_uniform_buffer, _uniform_buffer_size) = camera_barrel_fix.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChannelMixer:
                    {
                        var channel_mixer = new ChannelMixerUniform(new Vector3D<float>(0.5f, 0.2f, 0.1f), new Vector3D<float>(0.1f, 0.5f, 0.1f), new Vector3D<float>(0.1f, 0.1f, 0.9f));

                        (_uniform_buffer, _uniform_buffer_size) = channel_mixer.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaBlurH:
                    {
                        var chromablur_h = new ChromaBlurHUniform(new Vector4D<float>(5.5f, 5.5f, 5.5f, 1.0f), 1);

                        (_uniform_buffer, _uniform_buffer_size) = chromablur_h.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaBlurV:
                    {
                        var chromablur_h = new ChromaBlurVUniform(new Vector4D<float>(5.5f, 5.5f, 5.5f, 1.0f), 1);

                        (_uniform_buffer, _uniform_buffer_size) = chromablur_h.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ColorMatrix:
                    {
                        var color_matrix = new ColorMatrixUniform(new Matrix4X4<float>(
                            1.0f, 0.0f, 1.0f, 0.0f,
                            0.0f, 1.0f, 0.0f, 1.0f,
                            0.0f, 1.0f, 0.0f, 0.0f,
                            1.0f, 1.0f, 1.0f, 1.0f));

                        (_uniform_buffer, _uniform_buffer_size) = color_matrix.CreateBuffer(device);
                    }
                    break;
                case ShaderType.DeinterlaceBlend:
                    {
                        var deinterlace_blend = new DeinterlaceBlendUniform(new Vector4D<float>(1.0f, 0.5f, 0.0f, 1.0f));

                        (_uniform_buffer, _uniform_buffer_size) = deinterlace_blend.CreateBuffer(device);
                    }
                    break;
                case ShaderType.DilateAlpha:
                case ShaderType.Dilate:
                case ShaderType.Erode:
                case ShaderType.ErodeAlpha:
                    {
                        var dilate = new DilateUniform(new Vector2D<float>(0.1f, 0.1f), 0.8f);

                        (_uniform_buffer, _uniform_buffer_size) = dilate.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ExposureHightlight:
                    {
                        var exposure_highlight = new ExposureHighlightUniform(0.5f, new Vector4D<float>(1.0f, 0.5f, 0.0f, 1.0f), new Vector4D<float>(0.4f, 0.1f, 0.8f, 1.0f));

                        (_uniform_buffer, _uniform_buffer_size) = exposure_highlight.CreateBuffer(device);
                    }
                    break;
                case ShaderType.GaussianBlurV:
                case ShaderType.GaussianBlurH:
                    {
                        var gaussian_blur = new GaussianBlurVHUniform(new Vector4D<float>(1.0f, 0.5f, 0.0f, 1.0f), 0.5f);

                        (_uniform_buffer, _uniform_buffer_size) = gaussian_blur.CreateBuffer(device);
                    }
                    break;
                case ShaderType.LevelsAlpha:
                case ShaderType.Levels:
                    {
                        var levels = new LevelsUniform(0.5f, 0.3f, 0.4f, 0.1f, 0.3f);

                        (_uniform_buffer, _uniform_buffer_size) = levels.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Quantise:
                    {
                        var quantise = new QuantiseUniform(1.0f);

                        (_uniform_buffer, _uniform_buffer_size) = quantise.CreateBuffer(device);
                    }
                    break;
                case ShaderType.RgbBalanceKeepLuma:
                case ShaderType.RgbBalance:
                    {
                        var rgb_balance = new RgbBalanceUniform(red, green, blue);

                        (_uniform_buffer, _uniform_buffer_size) = rgb_balance.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Sharpen:
                    {
                        var sharpen = new SharpenUniform(new Vector4D<float>(1.0f, 0.4f, 0.8f, 1.0f), 0.1f);

                        (_uniform_buffer, _uniform_buffer_size) = sharpen.CreateBuffer(device);
                    }
                    break;
                case ShaderType.WhiteBalance:
                    {
                        var white_balance = new WhiteBalanceUniform(1.0f, 0.2f, 0.2f, 0.2f, 0.8f, new Vector2D<float>(1.0f, 1.0f));

                        (_uniform_buffer, _uniform_buffer_size) = white_balance.CreateBuffer(device);
                    }
                    break;
                    //ChromaKey
                case ShaderType.ChromaKey:
                    {
                        var chromakey = new ChromaKeyUnifrom(new Vector3D<float>(1.0f, 0.2f, 0.5f),1.0f, 0.2f, 0.2f, 0.8f, 0.5f, 0.7f);

                        (_uniform_buffer, _uniform_buffer_size) = chromakey.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaKeyAntiSpill:
                    {
                        var chromakey_antispill = new ChromaKeyAntiSpillUniform(new Vector3D<float>(1.0f, 0.2f, 0.5f), 10.0f, 20.5f);

                        (_uniform_buffer, _uniform_buffer_size) = chromakey_antispill.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaKeyAntiSpill3:
                    {
                        var chromakey_antispill3 = new ChromaKeyAntiSpill3Uniform(new Vector3D<float>(1.0f, 0.2f, 0.5f), 10.0f, 20.5f, 1.0f);

                        (_uniform_buffer, _uniform_buffer_size) = chromakey_antispill3.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaKeyFast:
                    {
                        var chromakey_fast = new ChromaKeyFastUniform( 10.0f, 20.5f, 1.0f, new Vector3D<float>(0.0f, 0.0f, 0.5f), new Vector3D<float>(1.0f, 1.0f, 0.5f));

                        (_uniform_buffer, _uniform_buffer_size) = chromakey_fast.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaKeyNew:
                    {
                        var chromakey_new = new ChromaKeyNewUniform(new Vector3D<float>(21.0f, 20.5f, 20.2f), 20.5f, 21.0f, 20.6f, 20.5f, 21.0f, 20.5f, 21.0f, true);

                        (_uniform_buffer, _uniform_buffer_size) = chromakey_new.CreateBuffer(device);
                    }
                    break;
                case ShaderType.ChromaKeyNew2:
                    {
                        var chromakey_new2 = new ChromaKeyNew2Uniform(new Vector3D<float>(21.0f, 20.5f, 20.2f), 20.5f, 21.0f, 20.6f, 20.5f, 21.0f, 20.5f, 21.0f);

                        (_uniform_buffer, _uniform_buffer_size) = chromakey_new2.CreateBuffer(device);
                    }
                    break;
                // IMAGE
                case ShaderType.CopyCheckerboard:
                    {
                        var copy_checkerboard = new CopyCheckerboardUniform(new Vector2D<float>(600.0f, 600.0f));

                        (_uniform_buffer, _uniform_buffer_size) = copy_checkerboard.CreateBuffer(device);
                    }
                    break;
                case ShaderType.HdycToRgbLerp:
                case ShaderType.HdycToRgb:
                    {
                        var hdyc_to_rgb = new HdycToRgbUniform(new Vector4D<float>(500.0f));

                        (_uniform_buffer, _uniform_buffer_size) = hdyc_to_rgb.CreateBuffer(device);
                    }
                    break;
                case ShaderType.TiledBackground:
                    {
                        var tiled_background = new TiledBackgroundUniform(new Vector2D<float>(0.8f));

                        (_uniform_buffer, _uniform_buffer_size) = tiled_background.CreateBuffer(device);
                    }
                    break;
                case ShaderType.UyvyToRgb:
                    {
                        var uyvy_to_rgb = new UyvyToRgbUniform(new Vector4D<float>(300.0f));

                        (_uniform_buffer, _uniform_buffer_size) = uyvy_to_rgb.CreateBuffer(device);
                    }
                    break;
                case ShaderType.UyvyToRgbLerp:
                    {
                        var uyvy_to_rgb_lerp = new UyvyToRgbLerpUniform(new Vector4D<float>(300.0f), 20f);

                        (_uniform_buffer, _uniform_buffer_size) = uyvy_to_rgb_lerp.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Yuy2ToRgb:
                    {
                        var yuy2_to_rgb = new Yuy2ToRgbUniform(20f, new Vector2D<float>(0.5f), new Vector4D<float>(300.0f));

                        (_uniform_buffer, _uniform_buffer_size) = yuy2_to_rgb.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Yuy2ToRgbLerp:
                    {
                        var yuy2_to_rgb_lerp = new Yuy2ToRgbLerpUniform(20f, new Vector2D<float>(0.5f), new Vector4D<float>(300.0f), 20f);

                        (_uniform_buffer, _uniform_buffer_size) = yuy2_to_rgb_lerp.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Yuy2hdToRgb:
                    {
                        var yuy2hd_to_rgb = new Yuy2hdToRgbUniform(new Vector4D<float>(300.0f));

                        (_uniform_buffer, _uniform_buffer_size) = yuy2hd_to_rgb.CreateBuffer(device);
                    }
                    break;
                case ShaderType.Yuy2hdToRgbLerp:
                    {
                        var yuy2hd_to_rgb_lerp = new Yuy2hdToRgbLerpUniform(new Vector4D<float>(300.0f), 20f);

                        (_uniform_buffer, _uniform_buffer_size) = yuy2hd_to_rgb_lerp.CreateBuffer(device);
                    }
                    break;
                case ShaderType.YvyuToRgb:
                    {
                        var yvyu_to_rgb = new YvyuToRgbUniform(new Vector4D<float>(300.0f));

                        (_uniform_buffer, _uniform_buffer_size) = yvyu_to_rgb.CreateBuffer(device);
                    }
                    break;
                case ShaderType.YvyuToRgbLerp:
                    {
                        var yvyu_to_rgb_lerp = new YvyuToRgbLerpUniform(new Vector4D<float>(300.0f), 20f);

                        (_uniform_buffer, _uniform_buffer_size) = yvyu_to_rgb_lerp.CreateBuffer(device);
                    }
                    break;
            }
        }

        public void Init_Shader(string embedPath, ShaderType shaderType)
        {
            string wgsl = string.Empty;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embedPath))
            using (var sr = new StreamReader(stream))
                wgsl = sr.ReadToEnd();

            _shaders[shaderType] = device.CreateShaderModuleWGSL(wgsl, new WgpuWrappersSilk.Net.ShaderModuleCompilationHint[]
            {
                new WgpuWrappersSilk.Net.ShaderModuleCompilationHint { EntryPoint = "vs_main"},
                new WgpuWrappersSilk.Net.ShaderModuleCompilationHint { EntryPoint = "fs_main"}
            },
            $"{shaderType}");
        }

        private void CreateRenderSurface(WriteableBitmap image)
        {
            render_tex = device.CreateTexture(usage: TextureUsage.RenderAttachment | TextureUsage.CopySrc,
                dimension: TextureDimension.TextureDimension2D,
                size: new Extent3D((uint)image.Size.Width, (uint)image.Size.Height, 1),
                format: TextureFormat.Rgba8Unorm,
                mipLevelCount: 1,
                sampleCount: 1,
                viewFormats: new ReadOnlySpan<TextureFormat>(new[] { TextureFormat.Rgba8Unorm }),
                "tex");

            render_tex_view = render_tex.CreateView(format: TextureFormat.Rgba8Unorm,
                dimension: TextureViewDimension.TextureViewDimension2D,
                aspect: TextureAspect.All,
                baseMipLevel: 0,
                mipLevelCount: 1,
                baseArrayLayer: 0,
                arrayLayerCount: 1,
                "tex view");
        }

        private void CreateBindGroup0(WriteableBitmap image, ShaderType type)
        {
            bind_tex = device.CreateTexture(
           usage: TextureUsage.CopyDst | TextureUsage.TextureBinding,
           dimension: TextureDimension.TextureDimension2D,
           size: new Extent3D
           {
               Width = (uint)image.Size.Width,
               Height = (uint)image.Size.Height,
               DepthOrArrayLayers = 1,
           },
           format: TextureFormat.Rgba8Unorm,
           mipLevelCount: 1,
           sampleCount: 1,
           viewFormats: new ReadOnlySpan<TextureFormat>(new[] { TextureFormat.Rgba8Unorm }));

            bind_tex_view = bind_tex.CreateView(format: TextureFormat.Rgba8Unorm,
                dimension: TextureViewDimension.TextureViewDimension2D,
                aspect: TextureAspect.All,
                baseMipLevel: 0,
                mipLevelCount: 1,
                baseArrayLayer: 0,
                arrayLayerCount: 1,
                "bind tex view");

            using (var rep = image.Lock())
            {
                unsafe
                {
                    var pixels = new Span<byte>((void*)rep.Address, (int)rep.RowBytes * (int)image.Size.Height);

                    var queue = device.GetQueue();

                    queue.WriteTexture<byte>(new WgpuWrappersSilk.Net.ImageCopyTexture
                    {
                        Texture = bind_tex,
                        Aspect = TextureAspect.All,
                        MipLevel = 0,
                        Origin = new Origin3D { X = 0, Y = 0, Z = 0 },
                    },
                    data: pixels,
                    new TextureDataLayout
                    {
                        BytesPerRow = (uint)(sizeof(Rgba32) * image.Size.Width),
                        RowsPerImage = (uint)image.Size.Height,
                        Offset = 0,
                    },
                    new Extent3D
                    {
                        Width = (uint)image.Size.Width,
                        Height = (uint)image.Size.Height,
                        DepthOrArrayLayers = 1
                    });
                }
            }


            var sampler = device.CreateSampler(addressModeU: AddressMode.ClampToEdge,
                addressModeV: AddressMode.ClampToEdge,
                addressModeW: AddressMode.ClampToEdge,
                magFilter: FilterMode.Linear,
                minFilter: FilterMode.Nearest,
                mipmapFilter: MipmapFilterMode.Nearest,
                lodMinClamp: 0,
                lodMaxClamp: 1,
                compare: CompareFunction.Undefined,
                maxAnisotropy: 0);

            bind_tex_group_layout = device.CreateBindGroupLayout(new ReadOnlySpan<BindGroupLayoutEntry>(new BindGroupLayoutEntry[]
{
            new BindGroupLayoutEntry
            {
                Binding = 0,
                Visibility = ShaderStage.Fragment,
                Texture =  new TextureBindingLayout
                {
                    ViewDimension = TextureViewDimension.TextureViewDimension2D,
                    Multisampled = false,
                    SampleType = TextureSampleType.Float
                },
            },
            new BindGroupLayoutEntry{
                Binding = 1,
                Sampler = new SamplerBindingLayout
                {
                    Type = SamplerBindingType.Filtering,
                },
                Visibility = ShaderStage.Fragment,
            } }));

            bind_tex_group = device.CreateBindGroup(bind_tex_group_layout, new ReadOnlySpan<BindGroupEntry>(new BindGroupEntry[]
            {
                new BindGroupEntry
                {
                    Binding = 0,
                    TextureView = bind_tex_view,
                },
                new BindGroupEntry
                {
                    Binding = 1,
                    Sampler = sampler
                }
            }));
        }

        private void CreateRenderPipeline(ShaderType shaderType)
        {
            var pip = device.CreatePipelineLayout(new ReadOnlySpan<BindGroupLayoutPtr>(new BindGroupLayoutPtr[]
            {
                bind_tex_group_layout,
                uniform_bindgroup_layout
            }));

            unsafe
            {
                render_pipeline = device.CreateRenderPipeline(layout: pip,
                vertex: new WgpuWrappersSilk.Net.VertexState
                {
                    ShaderModule = _shaders[ShaderType.Quad],
                    EntryPoint = "vs_main",
                    Constants = new (string key, double value)[] { },
                    Buffers = new WgpuWrappersSilk.Net.VertexBufferLayout[]
                    {
                        new WgpuWrappersSilk.Net.VertexBufferLayout((ulong)sizeof(Vertex), VertexStepMode.Vertex,
                        new VertexAttribute[]
                        {
                            new VertexAttribute(VertexFormat.Float32x3, 0, 0),
                            new VertexAttribute(VertexFormat.Float32x2, (uint)sizeof(Vector3D<float>), 1)
                        })
                    }
                },
                primitive: new PrimitiveState
                {
                    Topology = PrimitiveTopology.TriangleList,
                    StripIndexFormat = IndexFormat.Undefined,
                    FrontFace = FrontFace.Ccw,
                    CullMode = CullMode.Back,
                },
                null,
                multisample: new MultisampleState
                {
                    Count = 1,
                    Mask = ~0u,
                    AlphaToCoverageEnabled = false,
                },
                new WgpuWrappersSilk.Net.FragmentState(
                    shaderModule: _shaders[shaderType],
                    entryPoint: "fs_main",
                    new (string key, double value)[] { },
                    colorTargets: new WgpuWrappersSilk.Net.ColorTargetState[]
                    {
                        new(
                        TextureFormat.Rgba8Unorm,
                        (
                            color: new(BlendOperation.Add, BlendFactor.One, BlendFactor.Zero),
                            alpha: new(BlendOperation.Add, BlendFactor.One, BlendFactor.Zero)
                        ),
                        ColorWriteMask.All
                        )
                    }));
            }
        }

        private void CreateBindGroup1(ShaderType type)
        {
            CreateUniform(type);

            uniform_bindgroup_layout = device.CreateBindGroupLayout(new ReadOnlySpan<BindGroupLayoutEntry>(new BindGroupLayoutEntry[]
            {
                new BindGroupLayoutEntry{
                    Binding = 0,
                    Buffer = new BufferBindingLayout
                    {
                        Type = BufferBindingType.Uniform,
                        HasDynamicOffset = false,
                        MinBindingSize = _uniform_buffer_size
                    },
                    Visibility = ShaderStage.Fragment
                }
            }));

            uniform_bindgroup = device.CreateBindGroup(uniform_bindgroup_layout, new ReadOnlySpan<BindGroupEntry>(new BindGroupEntry[]
            {
                new BindGroupEntry
                {
                    Binding = 0,
                    Buffer = _uniform_buffer,
                    Offset = 0,
                    Size = _uniform_buffer_size,
                }
            }));
        }

        float red;
        float green;
        float blue;

        public void DemoSetRGBBalance(float red, float green, float blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
        private bool isSetup = false;
        public void Render(WriteableBitmap image, ShaderType type)
        {
            if(!isSetup)
            {
                // Create Texture
                CreateRenderSurface(image);

                // Bind Tex
                CreateBindGroup0(image, type);
            }

            CreateBindGroup1(type);

            // Create pipeline
            CreateRenderPipeline(type);

            isSetup = true;

            Render(image);
        }

        private void Render(WriteableBitmap image)
        {
            var commandEncoder = device.CreateCommandEncoder("encoder");

            var (bytes_per_row, padded_bytes_per_row) = GetBufferDimensions(image);

            var output_buffer = device.CreateBuffer(usage: BufferUsage.CopyDst | BufferUsage.MapRead,
                size: (ulong)(padded_bytes_per_row * image.Size.Height),
                mappedAtCreation: false,
                "output buff");

            var render_pass = commandEncoder.BeginRenderPass(colorAttachments: new ReadOnlySpan<WgpuWrappersSilk.Net.RenderPassColorAttachment>(new WgpuWrappersSilk.Net.RenderPassColorAttachment[]
            {
                new WgpuWrappersSilk.Net.RenderPassColorAttachment
                {
                    View = render_tex_view,
                    ResolveTarget = default,
                    LoadOp = LoadOp.Clear,
                    StoreOp = StoreOp.Store,
                    ClearValue = new Color
                    {
                        R = 0.1,
                        G = 0.2,
                        B = 0.3,
                        A = 1
                    },
                }
            }));

            render_pass.SetPipeline(render_pipeline);
            render_pass.SetBindGroup(0, bind_tex_group, ReadOnlySpan<uint>.Empty);
            render_pass.SetBindGroup(1, uniform_bindgroup, ReadOnlySpan<uint>.Empty);
            render_pass.SetVertexBuffer(0, _vertex_buffer, 0, _vertex_buffer_size);
            render_pass.SetIndexBuffer(_index_buffer, IndexFormat.Uint32, 0, _index_buffer_size);

            render_pass.DrawIndexed((uint)quad_index_map.Length, 1, 0, 0, 0);
            render_pass.End();

            commandEncoder.CopyTextureToBuffer(new WgpuWrappersSilk.Net.ImageCopyTexture
            {
                Aspect = TextureAspect.All,
                MipLevel = 0,
                Origin = new Origin3D { X = 0, Y = 0, Z = 0 },
                Texture = render_tex,
            },
            new WgpuWrappersSilk.Net.ImageCopyBuffer
            {
                Buffer = output_buffer,
                Layout = new TextureDataLayout
                {
                    Offset = 0,
                    BytesPerRow = padded_bytes_per_row, // must be a valid multiple of 256
                    RowsPerImage = (uint)image.Size.Height
                }
            },
            new Extent3D
            {
                Height = (uint)image.Size.Height,
                Width = (uint)image.Size.Width,
                DepthOrArrayLayers = 1,
            });

            var queue = device.GetQueue();

            var commandBuffer = commandEncoder.Finish(null);
            queue.Submit(new ReadOnlySpan<CommandBufferPtr>(new[] { commandBuffer }));

            unsafe
            {
                wgpu.BufferMapAsync((Silk.NET.WebGPU.Buffer*)output_buffer, MapMode.Read, (nuint)0, (nuint)(padded_bytes_per_row * image.Size.Height), new PfnBufferMapCallback(
                                (arg0, data) =>
                                {
                                    if (arg0 != BufferMapAsyncStatus.Success)
                                    {
                                        throw new Exception($"Unable to map buffer! status: {arg0}");
                                    }

                                    var p = (byte*)wgpu.BufferGetMappedRange(output_buffer, (nuint)0, (nuint)(padded_bytes_per_row * image.Size.Height));

                                    unsafe
                                    {
                                        using (var rep = image.Lock())
                                        {
                                            var size = (int)padded_bytes_per_row * rep.Size.Height;
                                            byte[] paddedData = new byte[size];
                                            Marshal.Copy((IntPtr)p, paddedData, 0, size);

                                            var unpadded_chunks = paddedData.Chunk((int)padded_bytes_per_row).Select(m => m[..(int)bytes_per_row]).SelectMany(m => m).ToArray();

                                            fixed (void* ptr = unpadded_chunks)
                                            {
                                                FastCopy.Copy(rep.Address, new IntPtr(ptr), rep.RowBytes * rep.Size.Height);
                                            }
                                        }
                                    }
                                }), null);

                wgpu.TryGetDeviceExtension(null, out Wgpu wgpuSpecific);
                wgpuSpecific.DevicePoll(device, true, null);
                
            }

            //bind_tex.Destroy();
            //output_buffer.Unmap();
            //_uniform_buffer.Destroy();

            //wgpuSpecific.TextureDrop(tex);
            //wgpuSpecific.TextureViewDrop(tex_view);
            //wgpuSpecific.RenderPipelineDrop(pipeline);
            //wgpuSpecific.BindGroupDrop(tex_sampler_bindgroup);
            //wgpuSpecific.BindGroupDrop(frag_uniform_bindgroup);
            //wgpu.BufferUnmap(output_buffer);
        }

        private unsafe (uint bytes_per_row, uint padded_bytes_per_row) GetBufferDimensions(WriteableBitmap image)
        {
            uint bytes_per_row = (uint)(sizeof(Rgba32) * image.Size.Width);
            uint padded_bytes_per_row = 0;

            var padding = (256 - bytes_per_row % 256) % 256;

            padded_bytes_per_row = bytes_per_row + padding;

            return (bytes_per_row, padded_bytes_per_row);
        }
    }
}
