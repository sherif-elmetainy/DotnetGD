// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeArt.DotnetGD.Libgd
{
    /// <summary>
    /// Wrappers around Libgd native methods
    /// </summary>
    internal static unsafe class NativeWrappers
    {
        // Most libgd functions don't return error messages
        // A lot of the function have a void return type but can still fail
        // Others can return a null pointer without stating a reason in return.
        // While this library tries to do some validation before calling libgd functions
        // Some function can still fail
        // The default error reporting in libgd would call gd_error which writes to stderr
        // This can be overriden by calling gdSetErrorMethod function
        // Normally I would Marshal a pointer to a managed function and have that function throw an exception
        // However, a closer look at the source code of libgd showed that a this is called before freeing unmanaged memory used by libgd
        // So having the managed error handle throw an exception would cause unmanaged memory leaks that the GC can't collect.
        // I also cannot read the error from stderr because it's the same for all threads

        // As a REALLY UGLY workaround, I have the managed handler write the error message to a thread static variable.

        private const int Notice = 5;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LibgdErrorCallbackDelegate(
            int priority, [MarshalAs(UnmanagedType.LPStr)] string message);

        [ThreadStatic]
        private static string _currentError;

        /// <summary>
        /// Managed error handler. Libgd handler is a c-style variable args function (line printf family of functions). 
        /// The error message passed is the prinft format string, but I don't know how I can access the C variable arguments
        /// from managed code.
        /// So the exception message would have format specifiers like %s or %d rather than actual error paramters. 
        /// But this is better than having no error message at all.
        /// The fact that the signature of the method is different from that of the C-decl (the var args argument is missing) is not a problem
        /// since in C declaration convention the caller is responsible for removing the arguments from the stack 
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="message"></param>
        private static void LibgdErrorCallback(int priority, string message)
        {
            if (priority >= Notice)
                return;
            _currentError = message;
        }

        /// <summary>
        /// Initialize setErrorMethod
        /// </summary>
        public static void InitializeLibGd()
        {
            NativeMethods.gdSetErrorMethod(Marshal.GetFunctionPointerForDelegate<LibgdErrorCallbackDelegate>(LibgdErrorCallback));

            // Calling this during initialization because of the following (comment copied from gd.h)
            /* 2.0.16: for thread-safe use of gdImageStringFT and friends,
            call this before allowing any thread to call gdImageStringFT.
            Otherwise it is invoked by the first thread to invoke
            gdImageStringFT, with a very small but real risk of a race condition.
            Return 0 on success, nonzero on failure to initialize freetype. */
            //var result = NativeMethods.gdFontCacheSetup();
            //if (result != 0)
            //    ThrowLibgdException($"Failed to initialize font cache. Call result: {result}.", nameof(NativeMethods.gdFontCacheSetup));
        }

        /// <summary>
        /// Reset error (this is called before every native call to clear the error message)
        /// </summary>
        private static void ResetError()
        {
            _currentError = null;
        }

        /// <summary>
        /// Throw an exception
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="methodName"></param>
        private static void ThrowException(string errorMessage, [CallerMemberName] string methodName = null)
        {
            var error = $"LIBGD Error: Method {methodName} failed: {errorMessage}";
            throw new LibgdException(error);
        }

        private static void ThrowLibgdException(string errorMessage, string methodName)
        {
            var error = $"LIBGD Error: Method {methodName} failed: {errorMessage}";
            throw new LibgdException(error);
        }

        private static void CheckPointerResult(IntPtr result, int size, [CallerMemberName] string methodName = null)
        {
            if (result != IntPtr.Zero && size > 0 && string.IsNullOrEmpty(_currentError)) return;
            var errorMessage = string.IsNullOrWhiteSpace(_currentError)
                ? (result == IntPtr.Zero ? "A null pointer was returned." : "A non-positive buffer size was returned")
                : _currentError;
            if (result != IntPtr.Zero)
            {
                // Normally we should never get here
                NativeMethods.gdFree(result);
            }
            ThrowLibgdException(errorMessage, methodName);
        }

        private static void CheckImageResult(GdImage* result, [CallerMemberName] string methodName = null)
        {
            if (result != null && string.IsNullOrEmpty(_currentError)) return;
            var errorMessage = string.IsNullOrWhiteSpace(_currentError)
                ? "A null pointer was returned."
                : _currentError;
            if (result != null)
            {
                // Normally we should never get here
                NativeMethods.gdImageDestroy(result);
            }
            ThrowLibgdException(errorMessage, methodName);
        }

        private static void CheckForLibgdError([CallerMemberName] string methodName = null)
        {
            if (string.IsNullOrWhiteSpace(_currentError)) return;
            var error = $"LIBGD Error: Method {methodName} failed: {_currentError}";
            throw new LibgdException(error);
        }

        // Creation and destruction functions
        // ReSharper disable InconsistentNaming

        public static void gdImageDestroy(GdImage* im)
        {
            ResetError();
            NativeMethods.gdImageDestroy(im);
            CheckForLibgdError();
        }

        public static GdImage* gdImageCreate(int width, int height)
        {
            ResetError();
            var res = NativeMethods.gdImageCreate(width, height);
            CheckImageResult(res);
            return res;
        }


        public static GdImage* gdImageCreateTrueColor(int width, int height)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateTrueColor(width, height);
            CheckImageResult(res);
            return res;
        }

        public static void gdFree(IntPtr m)
        {
            ResetError();
            NativeMethods.gdFree(m);
            CheckForLibgdError();
        }

        // Utility functions
        public static ImageCompareResult gdImageCompare(GdImage* im1, GdImage* im2)
        {
            ResetError();
            var res = NativeMethods.gdImageCompare(im1, im2);
            CheckForLibgdError();
            return res;
        }

        // Color functions

        public static int gdImageColorResolveAlpha(GdImage* im, int r, int g, int b, int a)
        {
            ResetError();
            var res = NativeMethods.gdImageColorResolveAlpha(im, r, g, b, a);
            CheckForLibgdError();
            if (res < 0)
            {
                ThrowException($"Failed to allocate color ({a}, {r}, {g}, {b}). Method returned {res}.");
            }
            return res;
        }

        public static int gdImageGetTrueColorPixel(GdImage* im, int x, int y)
        {
            ResetError();
            var res = NativeMethods.gdImageGetTrueColorPixel(im, x, y);
            CheckForLibgdError();
            return res;
        }

        public static void gdImageSetPixel(GdImage* im, int x, int y, int color)
        {
            ResetError();
            NativeMethods.gdImageSetPixel(im, x, y, color);
            CheckForLibgdError();
        }

        // Drawing functions
        public static void gdImageSetStyle(GdImage* im, IntPtr style, int noOfPixels)
        {
            ResetError();
            NativeMethods.gdImageSetStyle(im, style, noOfPixels);
            CheckForLibgdError();
        }

        public static void gdImageSetBrush(GdImage* im, GdImage* brush)
        {
            ResetError();
            NativeMethods.gdImageSetBrush(im, brush);
            CheckForLibgdError();
        }

        public static void gdImageSetTile(GdImage* im, GdImage* tile)
        {
            ResetError();
            NativeMethods.gdImageSetTile(im, tile);
            CheckForLibgdError();
        }

        public static void gdImageLine(GdImage* im, int x1, int y1, int x2, int y2, int color)
        {
            ResetError();
            NativeMethods.gdImageLine(im, x1, y1, x2, y2, color);
            CheckForLibgdError();
        }

        public static void gdImageRectangle(GdImage* im, int x1, int y1, int x2, int y2, int color)
        {
            ResetError();
            NativeMethods.gdImageRectangle(im, x1, y1, x2, y2, color);
            CheckForLibgdError();
        }

        public static void gdImageFilledRectangle(GdImage* im, int x1, int y1, int x2, int y2, int color)
        {
            ResetError();
            NativeMethods.gdImageFilledRectangle(im, x1, y1, x2, y2, color);
            CheckForLibgdError();
        }

        public static void gdImageEllipse(GdImage* im, int x1, int y1, int w, int h, int color)
        {
            ResetError();
            NativeMethods.gdImageEllipse(im, x1, y1, w, h, color);
            CheckForLibgdError();
        }

        public static void gdImageFilledEllipse(GdImage* im, int x1, int y1, int w, int h, int color)
        {
            ResetError();
            NativeMethods.gdImageFilledEllipse(im, x1, y1, w, h, color);
            CheckForLibgdError();
        }

        public static void gdImagePolygon(GdImage* im, Point* pointPtr, int n, int color)
        {
            ResetError();
            NativeMethods.gdImagePolygon(im, pointPtr, n, color);
            CheckForLibgdError();
        }

        public static void gdImageOpenPolygon(GdImage* im, Point* pointPtr, int n, int color)
        {
            ResetError();
            NativeMethods.gdImageOpenPolygon(im, pointPtr, n, color);
            CheckForLibgdError();
        }

        public static void gdImageFilledPolygon(GdImage* im, Point* pointPtr, int n, int color)
        {
            ResetError();
            NativeMethods.gdImageFilledPolygon(im, pointPtr, n, color);
            CheckForLibgdError();
        }


        public static void gdImageSetClip(GdImage* im, int x1, int y1, int x2, int y2)
        {
            ResetError();
            NativeMethods.gdImageSetClip(im, x1, y1, x2, y2);
            CheckForLibgdError();
        }

        public static void gdImageStringFT(GdImage* im, int* brect, int fg,
            string fontList, double ptSize, double angle, int x, int y,
            byte[] utf8String
            )
        {
            ResetError();
            var res = NativeMethods.gdImageStringFT(im, brect, fg, fontList, ptSize, angle, x, y, utf8String);
            CheckForLibgdError();
            if (!string.IsNullOrWhiteSpace(res))
                ThrowException(res);
        }


        // Jpeg Encoding/Decoding
        public static void gdImageJpegCtx(GdImage* im, GdIoCtx* output, int quality)
        {
            ResetError();
            NativeMethods.gdImageJpegCtx(im, output, quality);
            CheckForLibgdError();
        }

        public static IntPtr gdImageJpegPtr(GdImage* im, out int size, int quality)
        {
            ResetError();
            var res = NativeMethods.gdImageJpegPtr(im, out size, quality);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromJpegCtxEx(GdIoCtx* input, int ignoreWarning)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromJpegCtxEx(input, ignoreWarning);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromJpegPtrEx(int size, IntPtr data, int ignoreWarning)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromJpegPtrEx(size, data, ignoreWarning);
            CheckImageResult(res);
            return res;
        }

        // PNG Encoding/Decoding
        public static void gdImagePngCtxEx(GdImage* im, GdIoCtx* output, int level)
        {
            ResetError();
            NativeMethods.gdImagePngCtxEx(im, output, level);
            CheckForLibgdError();
        }

        public static IntPtr gdImagePngPtrEx(GdImage* im, out int size, int level)
        {
            ResetError();
            var res = NativeMethods.gdImagePngPtrEx(im, out size, level);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromPngCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromPngCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromPngPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromPngPtr(size, data);
            CheckImageResult(res);
            return res;
        }


        // Gif Encoding/Decoding
        public static void gdImageGifCtx(GdImage* im, GdIoCtx* output)
        {
            ResetError();
            NativeMethods.gdImageGifCtx(im, output);
            CheckForLibgdError();
        }

        public static IntPtr gdImageGifPtr(GdImage* im, out int size)
        {
            ResetError();
            var res = NativeMethods.gdImageGifPtr(im, out size);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromGifCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromGifCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromGifPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromGifPtr(size, data);
            CheckImageResult(res);
            return res;
        }

        public static void gdImageGifAnimBeginCtx(GdImage* im, GdIoCtx* output, int globalCM, int loops)
        {
            ResetError();
            NativeMethods.gdImageGifAnimBeginCtx(im, output, globalCM, loops);
            CheckForLibgdError();
        }

        public static void gdImageGifAnimAddCtx(GdImage* im, GdIoCtx* output, int localCM, int leftOfs,
            int topOfs, int delay, int disposal, GdImage* previousImage)
        {
            ResetError();
            NativeMethods.gdImageGifAnimAddCtx(im, output, localCM, leftOfs, topOfs, delay, disposal, previousImage);
            CheckForLibgdError();
        }

        public static void gdImageGifAnimEndCtx(GdIoCtx* output)
        {
            ResetError();
            NativeMethods.gdImageGifAnimEndCtx(output);
            CheckForLibgdError();
        }

        // Tiff Encoding/Decoding
        public static void gdImageTiffCtx(GdImage* im, GdIoCtx* output)
        {
            ResetError();
            NativeMethods.gdImageTiffCtx(im, output);
            CheckForLibgdError();
        }

        public static IntPtr gdImageTiffPtr(GdImage* im, out int size)
        {
            ResetError();
            var res = NativeMethods.gdImageTiffPtr(im, out size);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromTiffCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromTiffCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromTiffPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromTiffPtr(size, data);
            CheckImageResult(res);
            return res;
        }

        // Bmp Encoding/Decoding
        public static void gdImageBmpCtx(GdImage* im, GdIoCtx* output, int compression)
        {
            ResetError();
            NativeMethods.gdImageBmpCtx(im, output, compression);
            CheckForLibgdError();
        }

        public static IntPtr gdImageBmpPtr(GdImage* im, out int size, int compression)
        {
            ResetError();
            var res = NativeMethods.gdImageBmpPtr(im, out size, compression);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromBmpCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromBmpCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromBmpPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromBmpPtr(size, data);
            CheckImageResult(res);
            return res;
        }

        // wbmp Encoding/Decoding
        public static void gdImageWBMPCtx(GdImage* im, int fg, GdIoCtx* output)
        {
            ResetError();
            NativeMethods.gdImageWBMPCtx(im, fg, output);
            CheckForLibgdError();
        }

        public static IntPtr gdImageWBMPPtr(GdImage* im, out int size, int fg)
        {
            ResetError();
            var res = NativeMethods.gdImageWBMPPtr(im, out size, fg);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromWBMPCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromWBMPCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromWBMPPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromWBMPPtr(size, data);
            CheckImageResult(res);
            return res;
        }

        // Webp Encoding/Decoding
        public static void gdImageWebpCtx(GdImage* im, GdIoCtx* output, int quantization)
        {
            ResetError();
            NativeMethods.gdImageWebpCtx(im, output, quantization);
            CheckForLibgdError();
        }

        public static IntPtr gdImageWebpPtrEx(GdImage* im, out int size, int quantization)
        {
            ResetError();
            var res = NativeMethods.gdImageWebpPtrEx(im, out size, quantization);
            CheckPointerResult(res, size);
            return res;
        }

        public static GdImage* gdImageCreateFromWebpCtx(GdIoCtx* input)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromWebpCtx(input);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCreateFromWebpPtr(int size, IntPtr data)
        {
            ResetError();
            var res = NativeMethods.gdImageCreateFromWebpPtr(size, data);
            CheckImageResult(res);
            return res;
        }

        // Copy methods
        public static void gdImageCopy(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int w, int h)
        {
            ResetError();
            NativeMethods.gdImageCopy(dst, src, dstX, dstY, srcX, srcY, w, h);
            CheckForLibgdError();
        }

        public static void gdImageCopyMerge(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int w, int h, int pct)
        {
            ResetError();
            NativeMethods.gdImageCopyMerge(dst, src, dstX, dstY, srcX, srcY, w, h, pct);
            CheckForLibgdError();
        }

        public static void gdImageCopyMergeGray(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY,
            int w, int h, int pct)
        {
            ResetError();
            NativeMethods.gdImageCopyMergeGray(dst, src, dstX, dstY, srcX, srcY, w, h, pct);
            CheckForLibgdError();
        }

        public static void gdImageCopyResized(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY, int dstW, int dstH, int srcW, int srcH)
        {
            ResetError();
            NativeMethods.gdImageCopyResized(dst, src, dstX, dstY, srcX, srcY, dstW, dstH, srcW, srcH);
            CheckForLibgdError();
        }

        public static void gdImageCopyResampled(GdImage* dst, GdImage* src, int dstX, int dstY, int srcX, int srcY,
            int dstW, int dstH, int srcW, int srcH)
        {
            ResetError();
            NativeMethods.gdImageCopyResampled(dst, src, dstX, dstY, srcX, srcY, dstW, dstH, srcW, srcH);
            CheckForLibgdError();
        }

        public static void gdImageCopyRotated(GdImage* dst, GdImage* src, double dstX, double dstY, int srcX, int srcY,
            int srcW, int srcH, int angle)
        {
            ResetError();
            NativeMethods.gdImageCopyRotated(dst, src, dstX, dstY, srcX, srcY, srcW, srcH, angle);
            CheckForLibgdError();
        }

        public static GdImage* gdImageClone(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImageClone(src);
            CheckImageResult(res);
            return res;
        }

        // Palette <-> True color methods
        public static int gdImageTrueColorToPalette(GdImage* im, int ditherFlag, int colorsWanted)
        {
            ResetError();
            var res = NativeMethods.gdImageTrueColorToPalette(im, ditherFlag, colorsWanted);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImagePaletteToTrueColor(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImagePaletteToTrueColor(src);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        //public static int gdImageColorMatch(GdImage* originalImage, GdImage* targetImage)
        //{
        //    ResetError();
        //    var res = NativeMethods.gdImageColorMatch(originalImage, targetImage);
        //    CheckForLibgdError();
        //    if (res != 0)
        //        ThrowException($"Method returned {res}.");
        //    return res;
        //}

        public static int gdImageTrueColorToPaletteSetMethod(GdImage* im, int method, int speed)
        {
            ResetError();
            var res = NativeMethods.gdImageTrueColorToPaletteSetMethod(im, method, speed);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static void gdImageTrueColorToPaletteSetQuality(GdImage* im, int minQuality, int maxQuality)
        {
            ResetError();
            NativeMethods.gdImageTrueColorToPaletteSetQuality(im, minQuality, maxQuality);
            CheckForLibgdError();
        }

        // color replace
        public static int gdImageColorReplace(GdImage* im, int src, int dst)
        {
            ResetError();
            var n = NativeMethods.gdImageColorReplace(im, src, dst);
            CheckForLibgdError();
            return n;
        }

        public static int gdImageColorReplaceThreshold(GdImage* im, int src, int dst, float threshold)
        {
            ResetError();
            var n = NativeMethods.gdImageColorReplaceThreshold(im, src, dst, threshold);
            CheckForLibgdError();
            return n;
        }

        public static int gdImageColorReplaceArray(GdImage* im, int len, int* src, int* dst)
        {
            ResetError();
            var n = NativeMethods.gdImageColorReplaceArray(im, len, src, dst);
            CheckForLibgdError();
            return n;
        }

        public static int gdImagePixelate(GdImage* im, int blockSize, uint mode)
        {
            ResetError();
            var res = NativeMethods.gdImagePixelate(im, blockSize, mode);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageScatter(GdImage* im, int sub, int plus)
        {
            ResetError();
            var res = NativeMethods.gdImageScatter(im, sub, plus);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageScatterColor(GdImage* im, int sub, int plus, int* colors, uint numColors)
        {
            ResetError();
            var res = NativeMethods.gdImageScatterColor(im, sub, plus, colors, numColors);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageScatterEx(GdImage* im, GdScatter* s)
        {
            ResetError();
            var res = NativeMethods.gdImageScatterEx(im, s);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageSmooth(GdImage* im, float weight)
        {
            ResetError();
            var res = NativeMethods.gdImageSmooth(im, weight);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageMeanRemoval(GdImage* im)
        {
            ResetError();
            var res = NativeMethods.gdImageMeanRemoval(im);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageEmboss(GdImage* im)
        {
            ResetError();
            var res = NativeMethods.gdImageEmboss(im);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageGaussianBlur(GdImage* im)
        {
            ResetError();
            var res = NativeMethods.gdImageGaussianBlur(im);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageEdgeDetectQuick(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImageEdgeDetectQuick(src);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageSelectiveBlur(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImageSelectiveBlur(src);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageConvolution(GdImage* src, float* filter, float filterDiv, float offset)
        {
            ResetError();
            var res = NativeMethods.gdImageConvolution(src, filter, filterDiv, offset);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageContrast(GdImage* src, double contrast)
        {
            ResetError();
            var res = NativeMethods.gdImageContrast(src, contrast);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageBrightness(GdImage* src, int brightness)
        {
            ResetError();
            var res = NativeMethods.gdImageContrast(src, brightness);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdImageGrayScale(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImageGrayScale(src);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static GdImage* gdImageCopyGaussianBlurred(GdImage* src, int radius, double sigma)
        {
            ResetError();
            var img = NativeMethods.gdImageCopyGaussianBlurred(src, radius, sigma);
            CheckImageResult(img);
            return img;
        }

        public static int gdImageNegate(GdImage* src)
        {
            ResetError();
            var res = NativeMethods.gdImageNegate(src);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static void gdImageFlipHorizontal(GdImage* im)
        {
            ResetError();
            NativeMethods.gdImageFlipHorizontal(im);
            CheckForLibgdError();
        }

        public static void gdImageFlipVertical(GdImage* im)
        {
            ResetError();
            NativeMethods.gdImageFlipVertical(im);
            CheckForLibgdError();
        }

        public static void gdImageFlipBoth(GdImage* im)
        {
            ResetError();
            NativeMethods.gdImageFlipBoth(im);
            CheckForLibgdError();
        }


        public static GdImage* gdImageCrop(GdImage* src, Rectangle* crop)
        {
            ResetError();
            var res = NativeMethods.gdImageCrop(src, crop);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCropAuto(GdImage* im, CropMode mode)
        {
            ResetError();
            var res = NativeMethods.gdImageCropAuto(im, mode);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageCropThreshold(GdImage* im, int color, float threshold)
        {
            ResetError();
            var res = NativeMethods.gdImageCropThreshold(im, color, threshold);
            CheckImageResult(res);
            return res;
        }


        public static int gdImageSetInterpolationMethod(GdImage* im, InterpolationMethod id)
        {
            ResetError();
            var res = NativeMethods.gdImageSetInterpolationMethod(im, id);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static InterpolationMethod gdImageGetInterpolationMethod(GdImage* im)
        {
            ResetError();
            var res = NativeMethods.gdImageGetInterpolationMethod(im);
            CheckForLibgdError();
            return res;
        }

        public static GdImage* gdImageScale(GdImage* src, uint newWidth, uint newHeight)
        {
            ResetError();
            var res = NativeMethods.gdImageScale(src, newWidth, newHeight);
            CheckImageResult(res);
            return res;
        }

        public static GdImage* gdImageRotateInterpolated(GdImage* src, float angle, int bgcolor)
        {
            ResetError();
            var res = NativeMethods.gdImageRotateInterpolated(src, angle, bgcolor);
            CheckImageResult(res);
            return res;
        }

        public static int gdAffineApplyToPointF(PointF* dst, PointF* src, double* affine)
        {
            ResetError();
            var res = NativeMethods.gdAffineApplyToPointF(dst, src, affine);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineInvert(double* dst, double* src)
        {
            ResetError();
            var res = NativeMethods.gdAffineInvert(dst, src);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineFlip(double* dst_affine, double* src_affine, int flip_h, int flip_v)
        {
            ResetError();
            var res = NativeMethods.gdAffineFlip(dst_affine, src_affine, flip_h, flip_v);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineConcat(double* dst, double* m1, double* m2)
        {
            ResetError();
            var res = NativeMethods.gdAffineConcat(dst, m1, m2);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineIdentity(double* dst)
        {
            ResetError();
            var res = NativeMethods.gdAffineIdentity(dst);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineScale(double* dst, double scaleX, double scaleY)
        {
            ResetError();
            var res = NativeMethods.gdAffineScale(dst, scaleX, scaleY);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineRotate(double* dst, double angle)
        {
            ResetError();
            var res = NativeMethods.gdAffineRotate(dst, angle);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineShearHorizontal(double* dst, double angle)
        {
            ResetError();
            var res = NativeMethods.gdAffineShearHorizontal(dst, angle);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineShearVertical(double* dst, double angle)
        {
            ResetError();
            var res = NativeMethods.gdAffineShearVertical(dst, angle);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdAffineTranslate(double* dst, double offsetX, double offsetY)
        {
            ResetError();
            var res = NativeMethods.gdAffineTranslate(dst, offsetX, offsetY);
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static double gdAffineExpansion(double* src)
        {
            ResetError();
            var res = NativeMethods.gdAffineExpansion(src);
            CheckForLibgdError();
            return res;
        }

        public static int gdAffineRectilinear(double* src)
        {
            ResetError();
            var res = NativeMethods.gdAffineRectilinear(src);
            CheckForLibgdError();
            return res;
        }

        public static int gdAffineEqual(double* matrix1, double* matrix2)
        {
            ResetError();
            var res = NativeMethods.gdAffineEqual(matrix1, matrix2);
            CheckForLibgdError();
            return res;
        }

        public static int gdTransformAffineGetImage(out GdImage* dst, GdImage* src, Rectangle* srcArea, double* affine)
        {
            ResetError();
            var res = NativeMethods.gdTransformAffineGetImage(out dst, src, srcArea, affine);
            CheckImageResult(dst);
            if (res == 0)
            {
                NativeMethods.gdImageDestroy(dst);
                ThrowException("Method returned FALSE.");
            }
            return res;
        }

        public static int gdTransformAffineCopy(GdImage* dst, int dstX, int dstY, GdImage* src, Rectangle* srcRegion, double* affine)
        {
            ResetError();
            var res = NativeMethods.gdTransformAffineCopy(dst, dstX, dstY, src, srcRegion, affine);
            CheckForLibgdError();
            if (res == 0)
                ThrowException("Method returned FALSE.");
            return res;
        }

        public static int gdMajorVersion()
        {
            ResetError();
            var res = NativeMethods.gdMajorVersion();
            CheckForLibgdError();
            return res;
        }

        public static int gdMinorVersion()
        {
            ResetError();
            var res = NativeMethods.gdMinorVersion();
            CheckForLibgdError();
            return res;
        }

        public static int gdReleaseVersion()
        {
            ResetError();
            var res = NativeMethods.gdReleaseVersion();
            CheckForLibgdError();
            return res;
        }

        public static string gdExtraVersion()
        {
            ResetError();
            var res = NativeMethods.gdExtraVersion();
            CheckForLibgdError();
            return res;
        }

        public static string gdVersionString()
        {
            ResetError();
            var res = NativeMethods.gdVersionString();
            CheckForLibgdError();
            return res;
        }

        public static void gdImageFilledArc(GdImage* im, int cx, int cy, int w, int h, int s, int e, int color, int style)
        {
            ResetError();
            NativeMethods.gdImageFilledArc(im, cx, cy, w, h, s, e, color, style);
            CheckForLibgdError();
        }

        public static void gdImageArc(GdImage* im, int cx, int cy, int w, int h, int s, int e, int color)
        {
            ResetError();
            NativeMethods.gdImageArc(im, cx, cy, w, h, s, e, color);
            CheckForLibgdError();
        }

        public static void gdImageFill(GdImage* im, int x, int y, int color)
        {
            ResetError();
            NativeMethods.gdImageFill(im, x, y, color);
            CheckForLibgdError();
        }

        public static void gdImageFillToBorder(GdImage* im, int x, int y, int border, int color)
        {
            ResetError();
            NativeMethods.gdImageFillToBorder(im, x, y, border, color);
            CheckForLibgdError();
        }
        // ReSharper restore InconsistentNaming
    }
}
