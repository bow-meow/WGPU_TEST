using System;

namespace SpinMe.Imaging.OpenGL.Processor
{
    public class FastCopy
    {
        private unsafe struct Copystruct2 { private fixed ulong l[2]; }
        private unsafe struct Copystruct4 { private fixed ulong l[4]; }
        private unsafe struct Copystruct16 { private fixed ulong l[16]; }
        private unsafe struct Copystruct64 { private fixed ulong l[64]; }
        private unsafe struct Copystruct256 { private fixed ulong l[256]; }

        public static unsafe void Copy(IntPtr dest, IntPtr src, int bytes)
        {
            Copy((byte*)dest, (byte*)src, bytes);
        }

        public static unsafe void Copy(byte* bdest, byte* bsrc, int bytes)
        {
            var remaining = bytes;
            while (remaining > 0)
            {
                if (remaining > sizeof(Copystruct256))
                {
                    *(Copystruct256*)bdest = *(Copystruct256*)bsrc;
                    bsrc += sizeof(Copystruct256);
                    bdest += sizeof(Copystruct256);
                    remaining -= sizeof(Copystruct256);
                }
                else
                {
                    if (remaining > sizeof(Copystruct64))
                    {
                        *(Copystruct64*)bdest = *(Copystruct64*)bsrc;
                        bsrc += sizeof(Copystruct64);
                        bdest += sizeof(Copystruct64);
                        remaining -= sizeof(Copystruct64);
                    }
                    else

                    {
                        if (remaining > sizeof(Copystruct16))
                        {
                            *(Copystruct16*)bdest = *(Copystruct16*)bsrc;
                            bsrc += sizeof(Copystruct16);
                            bdest += sizeof(Copystruct16);
                            remaining -= sizeof(Copystruct16);
                        }
                        else
                        {
                            if (remaining > sizeof(Copystruct4))
                            {
                                *(Copystruct4*)bdest = *(Copystruct4*)bsrc;
                                bsrc += sizeof(Copystruct4);
                                bdest += sizeof(Copystruct4);
                                remaining -= sizeof(Copystruct4);
                            }
                            else
                            {
                                if (remaining > sizeof(Copystruct2))
                                {
                                    *(Copystruct2*)bdest = *(Copystruct2*)bsrc;
                                    bsrc += sizeof(Copystruct2);
                                    bdest += sizeof(Copystruct2);
                                    remaining -= sizeof(Copystruct2);
                                }
                                else
                                {
                                    while (remaining-- > 0)
                                    {
                                        *bdest++ = *bsrc++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}