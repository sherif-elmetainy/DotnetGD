using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        public Image Transform(Rectangle srcRectangle, Matrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            CheckObjectDisposed();
            if (!Bounds.Contains(srcRectangle)) throw new ArgumentOutOfRangeException(nameof(srcRectangle), srcRectangle, "Rectangle is outside image bounds.");
            fixed (double* d = matrix.Data)
            {
                GdImage* result;
                NativeWrappers.gdTransformAffineGetImage(out result, ImagePtr, &srcRectangle, d);
                return new Image(result);
            }
        }

        public void DrawImage(Image image, Point dstPoint, Rectangle srcRectangle, Matrix matrix)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (!image.Bounds.Contains(srcRectangle)) throw new ArgumentOutOfRangeException(nameof(srcRectangle), srcRectangle, "Rectangle is outside image bounds.");
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            fixed (double* d = matrix.Data)
            {
                NativeWrappers.gdTransformAffineCopy(ImagePtr, dstPoint.X, dstPoint.Y, image.ImagePtr, &srcRectangle, d);
            }
        }
    }
}
