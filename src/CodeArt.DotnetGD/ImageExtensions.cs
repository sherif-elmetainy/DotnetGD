// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Resizes an image and creates *new* image. The original image is unchanged, the result is returned in a new image.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="size">the new size.</param>
        /// <param name="resample">whether to resampling when copying by smoothly interpolating pixel values so that reducing the size of an image would retain some clarity.</param>
        /// <returns>Resulting image.</returns>
        public static Image Resize(this Image image, Size size, bool resample = true)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (size == image.Size)
                return image.Clone();
            if (size.Width <= 0 || size.Height <= 0)
                throw new ArgumentException($"Invalid image size {size}.", nameof(size));
            var newImage = new Image(size, image.PixelFormat);
            try
            {
                newImage.DrawImage(image, newImage.Bounds, image.Bounds, resample);
                return newImage;
            }
            catch
            {
                newImage.Dispose();
                throw;
            }
        }

        public static void SetPixel(this Image image, int x, int y, Color color)
            => image.SetPixel(new Point(x, y), color);

        public static Color GetPixel(this Image image, int x, int y)
            => image.GetPixel(new Point(x, y));
    }
}
