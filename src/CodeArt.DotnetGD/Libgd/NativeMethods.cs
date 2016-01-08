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
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdFontCacheSetup();

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
        public static extern void gdImageSetBrush(GdImage* im, GdImage* brush);
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
        public static extern void gdImagePolygon(GdImage* im, Point* pointPtr, int n, int color);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageOpenPolygon(GdImage* im, Point* pointPtr, int n, int color);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFilledPolygon(GdImage* im, Point* pointPtr, int n, int color);


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


        // Copy methods
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopy(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int w, int h);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopyMerge(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int w, int h, int pct);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopyMergeGray(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int w, int h, int pct);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopyResized(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int dstW, int dstH, int srcW, int srcH);


        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopyResampled(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int dstW, int dstH, int srcW, int srcH);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageCopyRotated(GdImage* dst, GdImage* src, double dstX, double dstY, int srcX, int srcY, int srcW, int srcH, int angle);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageClone(GdImage* src);

        // Palette <-> True color methods
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageTrueColorToPalette(GdImage* im, int ditherFlag, int colorsWanted);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImagePaletteToTrueColor(GdImage* src);

        //[DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        //public static extern int gdImageColorMatch(GdImage* im1, GdImage* im2);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageTrueColorToPaletteSetMethod(GdImage* im, int method, int speed);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageTrueColorToPaletteSetQuality(GdImage* im, int minQuality, int maxQuality);

        // color replace
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageColorReplace(GdImage* im, int src, int dst);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageColorReplaceThreshold(GdImage* im, int src, int dst, float threshold);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageColorReplaceArray(GdImage* im, int len, int* src, int* dst);



        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImagePixelate(GdImage* im, int blockSize, uint mode);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageScatter(GdImage* im, int sub, int plus);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageScatterColor(GdImage* im, int sub, int plus, int* colors, uint numColors);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageScatterEx(GdImage* im, GdScatter* s);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageSmooth(GdImage* im, float weight);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageMeanRemoval(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageEmboss(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageGaussianBlur(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageEdgeDetectQuick(GdImage* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageSelectiveBlur(GdImage* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageConvolution(GdImage* src, float* filter, float filterDiv, float offset);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageContrast(GdImage* src, double contrast);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageBrightness(GdImage* src, int brightness);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageGrayScale(GdImage* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCopyGaussianBlurred(GdImage* src, int radius, double sigma);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageNegate(GdImage* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFlipHorizontal(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFlipVertical(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern void gdImageFlipBoth(GdImage* im);


        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCrop(GdImage* src, Rectangle* crop);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCropAuto(GdImage* im, CropMode mode);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageCropThreshold(GdImage* im, int color, float threshold);


        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdImageSetInterpolationMethod(GdImage* im, InterpolationMethod id);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern InterpolationMethod gdImageGetInterpolationMethod(GdImage* im);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageScale(GdImage* src, uint newWidth, uint newHeight);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern GdImage* gdImageRotateInterpolated(GdImage* src, float angle, int bgcolor);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineApplyToPointF(PointF* dst, PointF* src, double* affine);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineInvert(double* dst, double* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineFlip(double* dst, double* src, int flipH, int flipV);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineConcat(double* dst, double* m1, double* m2);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineIdentity(double* dst);
        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineScale(double* dst, double scaleX, double scaleY);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineRotate(double* dst, double angle);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineShearHorizontal(double* dst, double angle);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineShearVertical(double* dst, double angle);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineTranslate(double* dst, double offsetX, double offsetY);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern double gdAffineExpansion(double* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineRectilinear(double* src);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdAffineEqual(double* matrix1, double* matrix2);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdTransformAffineGetImage(out GdImage* dst, GdImage* src, Rectangle* srcArea, double* affine);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdTransformAffineCopy(GdImage* dst, int dstX, int dstY, GdImage* src, Rectangle* srcRegion, double* affine);

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdMajorVersion();

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdMinorVersion();

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        public static extern int gdReleaseVersion();

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        [return:MarshalAs(UnmanagedType.LPStr)]
        public static extern string gdExtraVersion();

        [DllImport(LibgdName, CallingConvention = DefaultCallingConvention)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string gdVersionString();
    }


}
