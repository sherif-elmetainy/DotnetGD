using System;
using System.Runtime.InteropServices;

namespace CodeArt.DotnetGD.Libgd
{
    /// <summary>
    /// Libgd native methods
    /// </summary>
    internal static unsafe class NativeMethods
    {
        private const string LibgdName = "libgd";
        private const CallingConvention DefaultCallingConvention = CallingConvention.StdCall;

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdSetErrorMethod(IntPtr functionPointer);

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

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageGetTrueColorPixel(GdImage* im, int x, int y);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageSetPixel(GdImage* im, int x, int y, int color);



        // Drawing functions
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageSetStyle(GdImage* im, IntPtr style, int noOfPixels);
        // Drawing functions
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageSetBrush(GdImage* im, GdImage *brush);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageSetTile(GdImage* im, GdImage* tile);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageLine(GdImage* im, int x1, int y1, int x2, int y2, int color);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageRectangle(GdImage* im, int x1, int y1, int x2, int y2, int color);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFilledRectangle(GdImage* im, int x1, int y1, int x2, int y2, int color);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageEllipse(GdImage* im, int x1, int y1, int w, int h, int color);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFilledEllipse(GdImage* im, int x1, int y1, int w, int h, int color);


        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageSetClip(GdImage* im, int x1, int y1, int x2, int y2);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string gdImageStringFT(GdImage* im, int* brect, int fg, 
                [MarshalAs(UnmanagedType.LPStr)]
                string fontList, double ptSize, double angle, int x, int y,
                byte[] utf8String
            );

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

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageGifAnimBeginCtx(GdImage* im, GdIoCtx* output, int globalCM, int loops);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageGifAnimAddCtx(GdImage* im, GdIoCtx* output, int localCM, int leftOfs,
            int topOfs, int delay, int disposal, GdImage* previousImage);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageGifAnimEndCtx(GdIoCtx* output);

        // Tiff Encoding/Decoding
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageTiffCtx(GdImage* im, GdIoCtx* output);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern IntPtr gdImageTiffPtr(GdImage* im, out int size);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCreateFromTiffCtx(GdIoCtx* input);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCreateFromTiffPtr(int size, IntPtr data);

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

        // Webp Encoding/Decoding
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageWebpCtx(GdImage* im, GdIoCtx* output, int quantization);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern IntPtr gdImageWebpPtrEx(GdImage* im, out int size, int quantization);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCreateFromWebpCtx(GdIoCtx* input);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCreateFromWebpPtr(int size, IntPtr data);
    }


}
