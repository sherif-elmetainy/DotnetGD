// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Pixelate the image
        /// </summary>
        /// <param name="blockSize">block size for pixels</param>
        /// <param name="mode">pixelate mode</param>
        public void Pixelate(int blockSize, PixelateMode mode = PixelateMode.Average)
        {
            if (blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "Invalid block size");
            if (mode < PixelateMode.UpplerLeft && mode >= PixelateMode.UpplerLeft)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid pixelate mode.");
            CheckObjectDisposed();
            NativeWrappers.gdImagePixelate(ImagePtr, blockSize, (uint)mode);
        }


        /// <summary>
        /// Scatters pixels in random directions without changing the colors of the pixels
        /// </summary>
        /// <param name="rangeSub">min scatter range </param>
        /// <param name="rangePlus">maximum scatter range</param>
        public void Scatter(int rangeSub, int rangePlus)
        {
            if (rangeSub >= rangePlus)
                throw new ArgumentException($"{nameof(rangeSub)} must be less than {nameof(rangePlus)}.", nameof(rangeSub));
            CheckObjectDisposed();
            NativeWrappers.gdImageScatter(ImagePtr, rangeSub, rangePlus);
        }

        /// <summary>
        /// Scatters pixels in random directions without changing the colors of the pixels
        /// </summary>
        /// <param name="rangeSub">min scatter range (determines how far pixel can move)</param>
        /// <param name="rangePlus">maximum scatter range (determines how far pixel can move)</param>
        /// <param name="colors">move only pixes matching colors in this array. </param>
        public void Scatter(int rangeSub, int rangePlus, Color[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                Scatter(rangeSub, rangePlus);
                return;
            }

            if (rangeSub >= rangePlus)
                throw new ArgumentException($"{nameof(rangeSub)} must be less than {nameof(rangePlus)}.", nameof(rangeSub));
            CheckObjectDisposed();
            var c = new int[colors.Length];
            for (var i = 0; i < colors.Length; i++)
            {
                c[i] = ResolveColor(colors[i]);
            }
            fixed (int* cs = c)
            {
                NativeWrappers.gdImageScatterColor(ImagePtr, rangeSub, rangePlus, cs, unchecked((uint)c.Length));
            }
        }

        /// <summary>
        /// Scatters pixels in random directions without changing the colors of the pixels
        /// </summary>
        /// <param name="rangeSub">min scatter range (determines how far pixel can move)</param>
        /// <param name="rangePlus">maximum scatter range (determines how far pixel can move)</param>
        /// <param name="colors">move only pixes matching colors in this array. </param>
        /// <param name="randomSeed">random seed useful to produce same scattering effect.</param>
        public void Scatter(int rangeSub, int rangePlus, Color[] colors, int randomSeed)
        {
            if (rangeSub >= rangePlus)
                throw new ArgumentException($"{nameof(rangeSub)} must be less than {nameof(rangePlus)}.", nameof(rangeSub));
            CheckObjectDisposed();
            var scatter = new GdScatter
            {
                Sub = rangeSub,
                Plus = rangePlus,
                Seed = unchecked((uint)randomSeed)
            };
            if (colors != null && colors.Length != 0)
            {
                var c = new int[colors.Length];
                for (var i = 0; i < colors.Length; i++)
                {
                    c[i] = ResolveColor(colors[i]);
                }
                fixed (int* cs = c)
                {
                    scatter.Colors = cs;
                    scatter.NumberOfColors = unchecked((uint)c.Length);
                    NativeWrappers.gdImageScatterEx(ImagePtr, &scatter);
                }
            }
            else
            {
                NativeWrappers.gdImageScatterEx(ImagePtr, &scatter);
            }
        }

        /// <summary>
        /// Smoothes the image (noise removal)
        /// </summary>
        /// <param name="weight"></param>
        public void Smooth(float weight)
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageSmooth(ImagePtr, weight);
        }

        /// <summary>
        /// Deblurs the image https://xjaphx.wordpress.com/2011/06/22/image-processing-mean-removal-effect/
        /// </summary>
        public void MeanRemoval()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageMeanRemoval(ImagePtr);
        }

        /// <summary>
        /// Each pixel of an image is replaced either by a highlight or a shadow, depending on light/dark boundaries on the original image.
        /// https://en.wikipedia.org/wiki/Image_embossing
        /// </summary>
        public void Emboss()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageEmboss(ImagePtr);
        }

        /// <summary>
        /// Perform gaussian blur (https://en.wikipedia.org/wiki/Gaussian_blur)
        /// </summary>
        public void GaussianBlur()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageGaussianBlur(ImagePtr);
        }

        /// <summary>
        /// Detect points at which image brighness changes sharply. 
        /// https://en.wikipedia.org/wiki/Edge_detection
        /// </summary>
        public void EdgeDetect()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageEdgeDetectQuick(ImagePtr);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SelectiveBlur()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageSelectiveBlur(ImagePtr);
        }

        public void GrayScale()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageGrayScale(ImagePtr);
        }

        /// <summary>
        /// Negate the image
        /// </summary>
        public void Negate()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageNegate(ImagePtr);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Kernel_(image_processing)#Convolution
        /// </summary>
        /// <param name="filter">convolution matrix</param>
        /// <param name="filterDiv"></param>
        /// <param name="offset"></param>
        public void Convolution(float[,] filter, float filterDiv, float offset)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (filter.GetLength(0) != 3 && filter.GetLength(1) != 3)
                throw new ArgumentException("Filter must be a 3x3 2D array.", nameof(filter));

            CheckObjectDisposed();
            fixed (float* f = filter)
            {
                NativeWrappers.gdImageConvolution(ImagePtr, f, filterDiv, offset);
            }
        }

        /// <summary>
        /// Sets the contrast of an image
        /// </summary>
        /// <param name="contrast">0 for lowest contract, 100 for highest contrast. Valid values are from 0 inclusive to 100 inclusive.</param>
        public void Contrast(double contrast)
        {
            if (contrast < 0.0d || contrast > 100.0d)
                throw new ArgumentOutOfRangeException(nameof(contrast), contrast, "Constrat must be from 0 to 100.");
            CheckObjectDisposed();
            NativeWrappers.gdImageContrast(ImagePtr, contrast);
        }

        /// <summary>
        /// Set brighness of the image.
        /// </summary>
        /// <param name="brightness">brightness value. Zero keeps the image unchanged, negative values darkens the image, positive values increase the brightness. Valid values are from -255 inclusive to 255 inclusive.</param>
        public void Brightness(int brightness)
        {
            if (brightness < -255 || brightness > 255)
                throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be from -255 to 255.");
            CheckObjectDisposed();
            NativeWrappers.gdImageBrightness(ImagePtr, brightness);
        }

        /// <summary>
        /// Create a *new* copy of an image by applying guassian blur https://en.wikipedia.org/wiki/Gaussian_blur
        /// </summary>
        /// <param name="radius">radius of the gaussian blur</param>
        /// <param name="sigma">he standard deviation of the Gaussian distribution</param>
        /// <returns></returns>
        public Image CopyGaussianBlurred(int radius, double sigma)
        {
            if (radius < 1) throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius must be positive.");
            CheckObjectDisposed();
            var result = NativeWrappers.gdImageCopyGaussianBlurred(ImagePtr, radius, sigma);
            return new Image(result);
        }

        /// <summary>
        /// Flip the image horizontally
        /// </summary>
        public void FlipHorizontal()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipHorizontal(ImagePtr);
        }

        /// <summary>
        /// Flip the image vertically
        /// </summary>
        public void FlipVertical()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipVertical(ImagePtr);
        }

        /// <summary>
        /// Flip the image vertically and horizontally
        /// </summary>
        public void FlipBoth()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipBoth(ImagePtr);
        }

        /// <summary>
        /// Creates a *new* image by cropping the source image using the speficied rectangle an returns a *new* cropped image. The source image is not modified.
        /// </summary>
        /// <param name="cropRectangle">crop rectangle</param>
        /// <returns>the newly created image</returns>
        public Image Crop(Rectangle cropRectangle)
        {
            CheckObjectDisposed();
            if (!Bounds.Contains(cropRectangle))
                throw new ArgumentException($"The rectangle {nameof(cropRectangle)} {cropRectangle} is outsite the boundaries of the image bound rectangle {Bounds}.", nameof(cropRectangle));
            return new Image(NativeWrappers.gdImageCrop(ImagePtr, &cropRectangle));
        }

        /// <summary>
        /// Creates a *new* image by cropping the source image using a specified crop mode and returns a *new* Image. The source image is not modified.
        /// </summary>
        /// <param name="mode"><see cref="CropMode"/> to use</param>
        /// <returns>A new cropped image.</returns>
        public Image Crop(CropMode mode)
        {
            CheckObjectDisposed();
            if (mode < CropMode.Default || mode > CropMode.Sides)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid crop mode.");
            return new Image(NativeWrappers.gdImageCropAuto(ImagePtr, mode));
        }

        /// <summary>
        /// Creates a *new* image by cropping the source image using a specified corner color. The source image is not modified.
        /// </summary>
        /// <param name="color">the corner color to use for cropping, pixels are cropped as long as they match the color or their distance is within the threshold.</param>
        /// <param name="threshold">Threshold to use to determine if the color matches the corner color specified. A threshold of zero means exact match. The threshold is the distance between the two colors in HSL color space.</param>
        /// <returns>A new cropped image.</returns>
        public Image Crop(Color color, float threshold)
        {
            CheckObjectDisposed();
            var c = ResolveColor(color);
            return new Image(NativeWrappers.gdImageCropThreshold(ImagePtr, c, threshold));
        }

        /// <summary>
        /// Creates a *new* image by cropping the source image using a specified corner color. The source image is not modified.
        /// </summary>
        /// <param name="newSize">size of the new image.</param>
        /// <param name="method">The interpolation method to use.</param>
        /// <returns>A new cropped image.</returns>
        public Image Resize(Size newSize, InterpolationMethod method = InterpolationMethod.Default)
        {
            if (newSize == Size)
                return Clone();
            if (newSize.Width <= 0 || newSize.Height <= 0)
                throw new ArgumentException($"Invalid image size {newSize}.", nameof(newSize));
            if (method < InterpolationMethod.Default || method >= InterpolationMethod.Invalid)
                throw new ArgumentOutOfRangeException(nameof(method), method, "Invalid interpolation method.");
            CheckObjectDisposed();

            NativeWrappers.gdImageSetInterpolationMethod(ImagePtr, method);
            return new Image(NativeWrappers.gdImageScale(ImagePtr, (uint)newSize.Width, (uint)newSize.Height));
        }

        /// <summary>
        /// Create a *new* image by rotating the source image. The source image is not modified.
        /// </summary>
        /// <param name="angle">rotation angle anti-clockwise in degrees.</param>
        /// <param name="bgColor">background color of the new image</param>
        /// <param name="method">interpolation method to use</param>
        /// <returns>A new rotated image.</returns>
        public Image Rotate(float angle, Color bgColor, InterpolationMethod method = InterpolationMethod.Default)
        {
            if (method < InterpolationMethod.Default || method >= InterpolationMethod.Invalid)
                throw new ArgumentOutOfRangeException(nameof(method), method, "Invalid interpolation method.");
            CheckObjectDisposed();
            var c = ResolveColor(bgColor);
            NativeWrappers.gdImageSetInterpolationMethod(ImagePtr, method);
            return new Image(NativeWrappers.gdImageRotateInterpolated(ImagePtr, angle, c));
        }
    }
}
