// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// creates a *new* image by applying a matrix transform to a rectangle in the source image and returns a *new* image.
        /// </summary>
        /// <param name="srcRectangle">source image rectangle</param>
        /// <param name="matrix">matrix to apply</param>
        /// <returns></returns>
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

        /// <summary>
        /// Copies a the pixels from a source image after applying matrix transformation
        /// </summary>
        /// <param name="image">source image</param>
        /// <param name="dstPoint">destination point in the target image.</param>
        /// <param name="srcRectangle">Source pixels in the source image.</param>
        /// <param name="matrix">matrix to apply</param>
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
