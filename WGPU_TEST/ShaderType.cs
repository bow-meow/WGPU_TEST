using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGPU_TEST
{
    public enum ShaderType
    {
        // CORE
        Quad,
        Border,
        Brightness_Contrast,
        CameraBarrelFix,
        ChannelMixer,
        ChromaBlurH,
        ChromaBlurV,
        ColorMatrix,
        DeinterlaceBlend,
        Dilate,
        DilateAlpha,
        Erode,
        ErodeAlpha,
        ExposureHightlight,
        GaussianBlurH,
        GaussianBlurV,
        Levels,
        LevelsAlpha,
        Quantise,
        RgbBalance,
        RgbBalanceKeepLuma,
        Sharpen,
        WhiteBalance,

        // ChromaKey
        ChromaKey,
        ChromaKeyAntiSpill,
        ChromaKeyAntiSpill3,
        ChromaKeyFast,
        ChromaKeyNew, // unsure if we need
        ChromaKeyNew2, // unsure if we need

        //IMAGE
        AlphaBlendAlpha,

        CopyCheckerboard,

        HdycToRgb,
        HdycToRgbLerp,
        TiledBackground,
        UyvyToRgb,
        UyvyToRgbLerp,
        Yuy2ToRgb,
        Yuy2ToRgbLerp,
        Yuy2hdToRgb,
        Yuy2hdToRgbLerp,
        YvyuToRgb,
        YvyuToRgbLerp,


    }
}
