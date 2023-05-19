using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;
using WGPU_TEST.Processor;

namespace WGPU_TEST.Views
{
    public partial class MainWindow : Window
    {
        WgpuImageProcessor state;

        private ShaderType _currentShader = ShaderType.RgbBalance;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WGPU_TEST.Assets.Kaioken_high_quality.PNG"))
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream))
            {
                var writableBitmap = new WriteableBitmap(
                    new PixelSize(image.Width, image.Height),
                    new Vector(96, 96),
                    PixelFormat.Rgba8888
                );

                CopyPixelsToWriteableBitmap(image, writableBitmap);

                var imageControl = this.FindControl<Avalonia.Controls.Image>("MyImage");
                imageControl.Source = writableBitmap;
                
            }
            

            state = new WgpuImageProcessor();

            RenderEngine.Content = $"{state.AdapterProperties.BackendType}";
            CurrentShader.Content = $"{_currentShader}";

            //QUAD
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.CommonVertexShaders.quadih.wgsl", ShaderType.Quad);
            //CORE
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.border.wgsl", ShaderType.Border);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.brightness_contrast.wgsl", ShaderType.Brightness_Contrast);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.camerabarrelfix.wgsl", ShaderType.CameraBarrelFix);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.channel_mixer.wgsl", ShaderType.ChannelMixer);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.chromablurh.wgsl", ShaderType.ChromaBlurH);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.chromablurv.wgsl", ShaderType.ChromaBlurV);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.colormatrix.wgsl", ShaderType.ColorMatrix);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.deinterlace_blend.wgsl", ShaderType.DeinterlaceBlend);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.dilate.wgsl", ShaderType.Dilate);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.dilate_alpha.wgsl", ShaderType.DilateAlpha);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.erode.wgsl", ShaderType.Erode);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.erode_alpha.wgsl", ShaderType.ErodeAlpha);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.exposure_highlight.wgsl", ShaderType.ExposureHightlight);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.gaussian_blurH.wgsl", ShaderType.GaussianBlurH);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.gaussian_blurV.wgsl", ShaderType.GaussianBlurV);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.levels.wgsl", ShaderType.Levels);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.levels_alpha.wgsl", ShaderType.LevelsAlpha);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.quantise.wgsl", ShaderType.Quantise);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.rgb_balance.wgsl", ShaderType.RgbBalance);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.rgb_balance_keepluma.wgsl", ShaderType.RgbBalanceKeepLuma);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.sharpen.wgsl", ShaderType.Sharpen);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.CoreFilters.white_balance.wgsl", ShaderType.WhiteBalance);
            //ChromaKey
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey.wgsl", ShaderType.ChromaKey);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey_antispill.wgsl", ShaderType.ChromaKeyAntiSpill);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey_antispill3.wgsl", ShaderType.ChromaKeyAntiSpill3);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey_fast.wgsl", ShaderType.ChromaKeyFast);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey_new.wgsl", ShaderType.ChromaKeyNew);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.ChromaKey.chromakey_new2.wgsl", ShaderType.ChromaKeyNew2);
            //IMAGE
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.copy_checkerboard.wgsl", ShaderType.CopyCheckerboard);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.hdyc_to_rgb.wgsl", ShaderType.HdycToRgb);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.hdyc_to_rgb_lerp.wgsl", ShaderType.HdycToRgbLerp);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.tiled_background.wgsl", ShaderType.TiledBackground);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.uyvy_to_rgb.wgsl", ShaderType.UyvyToRgb);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.uyvy_to_rgb_lerp.wgsl", ShaderType.UyvyToRgbLerp);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yuy2_to_rgb.wgsl", ShaderType.Yuy2ToRgb);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yuy2_to_rgb_lerp.wgsl", ShaderType.Yuy2ToRgbLerp);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yuy2hd_to_rgb.wgsl", ShaderType.Yuy2hdToRgb);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yuy2hd_to_rgb_lerp.wgsl", ShaderType.Yuy2hdToRgbLerp);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yvyu_to_rgb.wgsl", ShaderType.YvyuToRgb);
            state.Init_Shader("WGPU_TEST.GLSLShaders.v330.GPUImage.yvyu_to_rgb_lerp.wgsl", ShaderType.YvyuToRgbLerp);

            RED.GetPropertyChangedObservable(Slider.ValueProperty).AddClassHandler<Slider>((t, args) => {
                this.InvalidateVisual();
            });
            GREEN.GetPropertyChangedObservable(Slider.ValueProperty).AddClassHandler<Slider>((t, args) => {
                this.InvalidateVisual();
            });
            BLUE.GetPropertyChangedObservable(Slider.ValueProperty).AddClassHandler<Slider>((t, args) => {
                this.InvalidateVisual();
            });
        }

        private void CopyPixelsToWriteableBitmap(Image<Rgba32> image, WriteableBitmap writableBitmap)
        {
            using (var fb = writableBitmap.Lock())
            {
                image.ProcessPixelRows(p =>
                {
                    for (var y = 0; y < p.Height; y++)
                    {
                        var imageRow = p.GetRowSpan(y);

                        for(int x = 0; x < p.Width; x++)
                        {
                            var pixel = imageRow[x];

                            fb.SetPixel(x, y, new Avalonia.Media.Color(pixel.A, pixel.R, pixel.G, pixel.B));
                        }
                    }
                });
            }
        }

        public override void Render(DrawingContext context)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var imageControl = this.FindControl<Avalonia.Controls.Image>("MyImage");
                state.DemoSetRGBBalance((float)RED.Value, (float)GREEN.Value, (float)BLUE.Value);
                state.Render((WriteableBitmap)imageControl.Source, _currentShader);
                imageControl.InvalidateVisual();
            }, DispatcherPriority.Render);

        }
    }
}