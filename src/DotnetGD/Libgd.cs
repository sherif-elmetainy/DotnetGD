using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

#pragma warning disable 169

namespace DotnetGD
{
    internal static unsafe class Libgd
    {
        private const string LibgdName = "libgd";
        private const CallingConvention DefaultCallingConvention  = CallingConvention.StdCall;
        private const CallingConvention CallbackConvention = CallingConvention.Cdecl;

        public static class NativeMethods
        {
            // Creation and destruction functions
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageDestroy(GdImage* im);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreate(int width, int height);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateTrueColor(int width, int height);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdFree(IntPtr m);

            // Utility functions
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern ImageCompareResult gdImageCompare(GdImage* im1, GdImage* im2);

            // Color functions
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern int gdImageColorResolveAlpha(GdImage* im, int r, int g, int b, int a);

            // Drawing functions
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageRectangle(GdImage *im, int x1, int y1, int x2, int y2, int color);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageFilledRectangle(GdImage* im, int x1, int y1, int x2, int y2, int color);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageEllipse(GdImage* im, int x1, int y1, int w, int h, int color);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageFilledEllipse(GdImage* im, int x1, int y1, int w, int h, int color);


            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageSetClip(GdImage* im, int x1, int y1, int x2, int y2);


            // PNG Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageJpegCtx(GdImage* im, GdIoCtx* output, int quality);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImageJpegPtr(GdImage* im, out int size, int quality);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromPngCtx(GdIoCtx* input);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromPngPtr(int size, IntPtr data);


            // Jpeg Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImagePngCtxEx(GdImage* im, GdIoCtx* output, int level);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImagePngPtrEx(GdImage* im, out int size, int level);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromJpegCtxEx(GdIoCtx* input, int ignoreWarning);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromJpegPtrEx(int size, IntPtr data, int ignoreWarning);

            // Gif Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageGifCtx(GdImage* im, GdIoCtx* output);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImageGifPtr(GdImage* im, out int size);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromGifCtx(GdIoCtx* input);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromGifPtr(int size, IntPtr data);

            // Bmp Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageBmpCtx(GdImage* im, GdIoCtx* output, int compression);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImageBmpPtr(GdImage* im, out int size, int compression);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromBmpCtx(GdIoCtx* input);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromBmpPtr(int size, IntPtr data);

            // wbmp Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageWBMPCtx(GdImage* im, int fg, GdIoCtx* output);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImageWBMPPtr(GdImage* im, out int size, int fg);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromWBMPCtx(GdIoCtx* input);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromWBMPPtr(int size, IntPtr data);

#if WEBP
            // Webp Encoding/Decoding
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern void gdImageWebpCtx(GdImage* im, GdIoCtx* output, int quantization);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern IntPtr gdImageWebpPtrEx(GdImage* im, out int size, int quantization);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromWebpCtx(GdIoCtx* input);
            [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
            public static extern GdImage* gdImageCreateFromWebpPtr(int size, IntPtr data);
#endif
        }



        internal struct GdIoCtx : IDisposable
        {
            public GdIoCtx(Stream stream)
            {
                _getC = Marshal.GetFunctionPointerForDelegate<GetCDelelegate>(GetC);
                _getBuff = Marshal.GetFunctionPointerForDelegate<GetBufDelegate>(GetBuf);
                _putC = Marshal.GetFunctionPointerForDelegate<PutCDelegate>(PutC);
                _putBuf = Marshal.GetFunctionPointerForDelegate<PutBufDelegate>(PutBuf);
                _seek = Marshal.GetFunctionPointerForDelegate<SeekDelegate>(Seek);
                _tell = Marshal.GetFunctionPointerForDelegate<TellDelegate>(Tell);
                _gdFree = Marshal.GetFunctionPointerForDelegate<GdFreeDelegate>(GdFree);
                _data = IntPtr.Zero;
                _key = Interlocked.Increment(ref _currentKey);
                lock (Streams)
                {
                    Streams.Add(_key, stream);
                }
            }

            // ReSharper disable NotAccessedField.Local
            private readonly IntPtr _getC;
            private readonly IntPtr _getBuff;
            private readonly IntPtr _putC;
            private readonly IntPtr _putBuf;
            private readonly IntPtr _seek;
            private readonly IntPtr _tell;
            private readonly IntPtr _gdFree;
            private readonly IntPtr _data;
            private readonly long _key;
            private static long _currentKey;
            private static readonly Dictionary<long, Stream> Streams = new Dictionary<long, Stream>();

            private Stream Stream
            {
                get
                {
                    lock (Streams)
                    {
                        return Streams[_key];
                    }
                }
            }
            //private Stream _data;
            // ReSharper restore NotAccessedField.Local



            private static int GetC(GdIoCtx* ioCtx)
            {
                try
                {
                    var stream = (*ioCtx).Stream;
                    return stream.ReadByte();
                }
                catch
                {
                    return -1;
                }
            }

            private static int GetBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
            {
                if (size <= 0)
                    return 0;
                try
                {
                    var stream = (*ioCtx).Stream;
                    var managedBuff = new byte[size];
                    var res = stream.Read(managedBuff, 0, size);
                    Marshal.Copy(managedBuff, 0, buff, res);
                    return res;
                }
                catch
                {
                    return 0;   
                }
                
            }

            private static void PutC(GdIoCtx* ioCtx, int ch)
            {
                try
                {
                    var stream = (*ioCtx).Stream;
                    stream.WriteByte( unchecked((byte)ch));
                }
                catch
                {
                    // ignored
                }
            }

            private static int PutBuf(GdIoCtx* ioCtx, IntPtr buff, int size)
            {
                if (size <= 0)
                    return 0;
                try
                {
                    var stream = (*ioCtx).Stream;
                    var managedBuff = new byte[size];
                    Marshal.Copy(buff, managedBuff, 0, size);
                    stream.Write(managedBuff, 0, size);
                    return size;
                }
                catch
                {
                    return 0;
                }
            }

            private static int Seek(GdIoCtx* ioCtx, int offset)
            {
                var stream = (*ioCtx).Stream;
                try
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    return 1;
                }
                catch
                {
                    return 0;
                }
            }

            private static long Tell(GdIoCtx* ioCtx)
            {
                try
                {
                    var stream = (*ioCtx).Stream;
                    return stream.Position;
                }
                catch
                {
                    return -1;
                }
                
            }

            private static void GdFree(GdIoCtx* ioCtx)
            {
                // There is nothing to free
                // Do Nothing
            }

            public void Dispose()
            {
                lock (Streams)
                {
                    Streams.Remove(_key);
                }   
            }
        }

        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int GetCDelelegate(GdIoCtx* ioCtx);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int GetBufDelegate(GdIoCtx* ioCtx, IntPtr buff, int size);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate void PutCDelegate(GdIoCtx* ioCtx, int ch);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int PutBufDelegate(GdIoCtx* ioCtx, IntPtr buff, int size);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate int SeekDelegate(GdIoCtx* ioCtx, int offset);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate long TellDelegate(GdIoCtx* ioCtx);
        [UnmanagedFunctionPointer(CallbackConvention)]
        private delegate void GdFreeDelegate(GdIoCtx* ioCtx);

        private struct Colors4
        {
            private int _a1;
            private int _a2;
            private int _a3;
            private int _a4;

            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 1:
                            return _a1;
                        case 2:
                            return _a2;
                        case 3:
                            return _a3;
                        case 4:
                            return _a4;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 1:
                            _a1 = value;
                            break;
                        case 2:
                            _a2 = value;
                            break;
                        case 3:
                            _a3 = value;
                            break;
                        case 4:
                            _a4 = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        private struct Colors16
        {
            private Colors4 _a1;
            private Colors4 _a2;
            private Colors4 _a3;
            private Colors4 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0x3;
                    var i2 = (index & 0xffffffffc) >> 2;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0x3;
                    var i2 = index & 0xfffffffc0;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        private struct Colors64
        {
            private Colors16 _a1;
            private Colors16 _a2;
            private Colors16 _a3;
            private Colors16 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0xf;
                    var i2 = (index & 0xfffffff30) >> 4;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0xf;
                    var i2 = (index & 0xfffffff30) >> 4;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        public struct Colors256
        {
            private Colors64 _a1;
            private Colors64 _a2;
            private Colors64 _a3;
            private Colors64 _a4;

            public int this[int index]
            {
                get
                {
                    var i1 = index & 0x3f;
                    var i2 = (index & 0xfffffffc0) >> 6;

                    switch (i2)
                    {
                        case 1:
                            return _a1[i1];
                        case 2:
                            return _a2[i1];
                        case 3:
                            return _a3[i1];
                        case 4:
                            return _a4[i1];
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    var i1 = index & 0x3f;
                    var i2 = (index & 0xfffffffc0) >> 6;

                    switch (i2)
                    {
                        case 1:
                            _a1[i1] = value;
                            break;
                        case 2:
                            _a2[i1] = value;
                            break;
                        case 3:
                            _a3[i1] = value;
                            break;
                        case 4:
                            _a4[i1] = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

#pragma warning disable 649
        public struct GdImage
        {
            public byte** Pixels;
            public int Width;
            public int Height;
            /* These are valid in palette images only. See also
               'alpha', which appears later in the structure to
               preserve binary backwards compatibility */
            public int ColorsTotal;
            public Colors256 Red;
            public Colors256 Green;
            public Colors256 Blue;
            public Colors256 Open;
            /* For backwards compatibility, this is set to the
               first palette entry with 100% transparency,
               and is also set and reset by the
               gdImageColorTransparent function. Newer
               applications can allocate palette entries
               with any desired level of transparency; however,
               bear in mind that many viewers, notably
               many web browsers, fail to implement
               full alpha channel for PNG and provide
               support for full opacity or transparency only. */
            public int Transparent;
            public int* PolyInts;
            public int PolyAllocated;
            public GdImage* Brush;
            public GdImage* Tile;
            public Colors256 BrushColorMap;
            public Colors256 TileColorMap;
            public int StyleLength;
            public int StylePos;
            public int* Style;
            public int Interlace;
            /* New in 2.0: thickness of line. Initialized to 1. */
            public int Thick;
            /* New in 2.0: alpha channel for palettes. Note that only
               Macintosh Internet Explorer and (possibly) Netscape 6
               really support multiple levels of transparency in
               palettes, to my knowledge, as of 2/15/01. Most
               common browsers will display 100% opaque and
               100% Transparent correctly, and do something
               unpredictable and/or undesirable for levels
               in between. TBB */
            public Colors256 Alpha;
            /* Truecolor flag and Pixels. New 2.0 fields appear here at the
               end to minimize breakage of existing object code. */
            public int TrueColor;
            public int** Tpixels;
            /* Should alpha channel be copied, or applied, each time a
               pixel is drawn? This applies to truecolor images only.
               No attempt is made to alpha-blend in palette images,
               even if semitransparent palette entries exist.
               To do that, build your image as a truecolor image,
               then quantize down to 8 bits. */
            public AlphaBlendingEffect AlphaBlendingFlag;
            /* Should the alpha channel of the image be saved? This affects
               PNG at the moment; other future formats may also
               have that capability. JPEG doesn't. */
            public int SaveAlphaFlag;

            /* There should NEVER BE ACCESSOR MACROS FOR ITEMS BELOW HERE, so this
               part of the structure can be safely changed in new releases. */

            /* 2.0.12: anti-aliased globals. 2.0.26: just a few vestiges after
              switching to the fast, memory-cheap implementation from PHP-gd. */
            public int AA;
            public int AAColor;
            public int AADontBlend;

            /* 2.0.12: simple clipping rectangle. These values
              must be checked for safety when set; please use
              gdImageSetClip */
            public int ClipX1;
            public int ClipY1;
            public int ClipX2;
            public int ClipY2;

            /* 2.1.0: allows to specify resolution in dpi */
            public uint ResolutionX;
            public uint ResolutionY;

            /* Selects quantization method, see gdImageTrueColorToPaletteSetMethod() and gdPaletteQuantizationMethod enum. */
            public PaletteQuantizationMethod PaletteQuantizationMethod;
            /* speed/quality trade-off. 1 = best quality, 10 = best speed. 0 = method-specific default.
               Applicable to GD_QUANT_LIQ and GD_QUANT_NEUQUANT. */
            public int PaletteQuantizationSpeed;
            /* Image will remain true-color if conversion to palette cannot achieve given quality.
               Value from 1 to 100, 1 = ugly, 100 = perfect. Applicable to GD_QUANT_LIQ.*/
            public int PaletteQuantizationMinQuality;
            /* Image will use minimum number of palette colors needed to achieve given quality. Must be higher than paletteQuantizationMinQuality
               Value from 1 to 100, 1 = ugly, 100 = perfect. Applicable to GD_QUANT_LIQ.*/
            public int PaletteQuantizationMaxQuality;

            public InterpolationMethod InterpolationId;
            private IntPtr _interpolation;
        }
#pragma warning restore 649
    }
}

