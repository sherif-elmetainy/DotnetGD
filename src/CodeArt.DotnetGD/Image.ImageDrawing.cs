// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Draws an image by copying pixels to the target image
        /// </summary>
        /// <param name="image">image to draw</param>
        /// <param name="destPoint">top left corner of the destination rectangle within the destination image</param>
        /// <param name="sourceRectangle">source image rectangle.</param>
        public void DrawImage(Image image, Point destPoint, Rectangle sourceRectangle)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            NativeWrappers.gdImageCopy(ImagePtr, image.ImagePtr, destPoint.X, destPoint.Y, sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height);
        }

        /// <summary>
        /// Draws an image by copying pixels and resizing to the target image
        /// </summary>
        /// <param name="image">image to draw</param>
        /// <param name="destRectangle">destination rectangle</param>
        /// <param name="sourceRectangle">source image rectangle.</param>
        /// <param name="resample">whether to resampling when copying by smoothly interpolating pixel values so that reducing the size of an image would retain some clarity.</param>
        public void DrawImage(Image image, Rectangle destRectangle, Rectangle sourceRectangle, bool resample = true)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (destRectangle.Size == sourceRectangle.Size)
                DrawImage(image, destRectangle.Location, sourceRectangle);
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            if (resample)
            {
                NativeWrappers.gdImageCopyResampled(ImagePtr, image.ImagePtr, destRectangle.X, destRectangle.Y, sourceRectangle.X, sourceRectangle.Y, destRectangle.Width, destRectangle.Height, sourceRectangle.Width, sourceRectangle.Height);
            }
            else
            {
                NativeWrappers.gdImageCopyResized(ImagePtr, image.ImagePtr, destRectangle.X, destRectangle.Y, sourceRectangle.X, sourceRectangle.Y, destRectangle.Width, destRectangle.Height, sourceRectangle.Width, sourceRectangle.Height);
            }
        }

        public void DrawImageRotated(Image image, double dstX, double dstY, Rectangle sourceRectangle, int angle)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            NativeWrappers.gdImageCopyRotated(ImagePtr, image.ImagePtr, dstX, dstY, sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height, angle);
        }

        public void DrawImage(Image image, Point destPoint, Rectangle sourceRectangle, int mergePercentage)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            NativeWrappers.gdImageCopyMerge(ImagePtr, image.ImagePtr, destPoint.X, destPoint.Y, sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height, mergePercentage);
        }

        public void DrawImageGray(Image image, Point destPoint, Rectangle sourceRectangle, int mergePercentage)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            CheckObjectDisposed();
            image.CheckObjectDisposed();
            NativeWrappers.gdImageCopyMergeGray(ImagePtr, image.ImagePtr, destPoint.X, destPoint.Y, sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height, mergePercentage);
        }
    }
}
