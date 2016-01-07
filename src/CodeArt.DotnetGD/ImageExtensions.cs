using System;

namespace CodeArt.DotnetGD
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Crops an image and creates *new* image. The original image is unchanged, the result is returned in a new image.
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="cropRectangle">Crop rectangle.</param>
        /// <returns>Resulting image.</returns>
        public static Image Crop(this Image image, Rectangle cropRectangle)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (cropRectangle == image.Bounds)
                return image.Clone();
            if (!image.Bounds.Contains(cropRectangle))
                throw new ArgumentException($"The rectangle {nameof(cropRectangle)} {cropRectangle} is outsite the boundaries of the image bound rectangle {image.Bounds}.", nameof(cropRectangle));
            var newImage = new Image(cropRectangle.Size, image.PixelFormat);
            try
            {
                newImage.DrawImage(image, new Point(), cropRectangle);
                return newImage;
            }
            catch
            {
                newImage.Dispose();
                throw;
            }
        }

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
