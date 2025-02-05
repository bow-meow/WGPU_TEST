﻿using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGPU_TEST
{
    public static class PixelFormatExtensions
    {
        public static int GetBytesPerPixel(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Rgb565:
                    return 2;
                case PixelFormat.Rgba8888:
                case PixelFormat.Bgra8888:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null);
            }
        }
    }


    public static class LockedFramebufferExtensions
    {
        public static Span<byte> GetPixels(this ILockedFramebuffer? framebuffer)
        {
            unsafe
            {
                return new Span<byte>((byte*)framebuffer.Address, framebuffer.RowBytes * framebuffer.Size.Height);
            }
        }

        public static Span<byte> GetPixel(this ILockedFramebuffer? framebuffer, int x, int y)
        {
            unsafe
            {
                var bytesPerPixel = framebuffer.Format.GetBytesPerPixel();
                var zero = (byte*)framebuffer.Address;
                var offset = framebuffer.RowBytes * y + bytesPerPixel * x;
                return new Span<byte>(zero + offset, bytesPerPixel);
            }
        }

        public static void SetPixel(this ILockedFramebuffer? framebuffer, int x, int y, Color color)
        {
            var pixel = framebuffer.GetPixel(x, y);

            var alpha = color.A / 255.0;

            switch (framebuffer.Format)
            {
                case PixelFormat.Rgb565:
                    var value = (((color.R & 0b11111000) << 8) + ((color.G & 0b11111100) << 3) + (color.B >> 3));
                    pixel[0] = (byte)value;
                    pixel[1] = (byte)(value >> 8);
                    break;

                case PixelFormat.Rgba8888:
                    pixel[0] = (byte)(color.R * alpha);
                    pixel[1] = (byte)(color.G * alpha);
                    pixel[2] = (byte)(color.B * alpha);
                    pixel[3] = color.A;
                    break;

                case PixelFormat.Bgra8888:
                    pixel[0] = (byte)(color.B * alpha);
                    pixel[1] = (byte)(color.G * alpha);
                    pixel[2] = (byte)(color.R * alpha);
                    pixel[3] = color.A;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
