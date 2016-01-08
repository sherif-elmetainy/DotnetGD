// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Pixelate the imae
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

        public void Smooth(float weight)
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageSmooth(ImagePtr, weight);
        }

        public void MeanRemoval()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageMeanRemoval(ImagePtr);
        }

        public void Emboss()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageEmboss(ImagePtr);
        }

        public void GaussianBlur()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageGaussianBlur(ImagePtr);
        }

        public void EdgeDetect()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageEdgeDetectQuick(ImagePtr);
        }

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

        public void Negate()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageNegate(ImagePtr);
        }

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

        public void Contrast(double contrast)
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageContrast(ImagePtr, contrast);
        }

        public void Brightness(int brightness)
        {
            if (brightness < -255 || brightness > 255)
                throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be from -255 to 255.");
            CheckObjectDisposed();
            NativeWrappers.gdImageBrightness(ImagePtr, brightness);
        }

        public Image CopyGaussianBlurred(int radius, double sigma)
        {
            if (radius < 1) throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius must be positive.");
            CheckObjectDisposed();
            var result = NativeWrappers.gdImageCopyGaussianBlurred(ImagePtr, radius, sigma);
            return new Image(result);
        }

        public void FlipHorizontal()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipHorizontal(ImagePtr);
        }

        public void FlipVertical()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipVertical(ImagePtr);
        }

        public void FlipBoth()
        {
            CheckObjectDisposed();
            NativeWrappers.gdImageFlipBoth(ImagePtr);
        }

        public Image Crop(Rectangle cropRectangle)
        {
            CheckObjectDisposed();
            if (!Bounds.Contains(cropRectangle))
                throw new ArgumentException($"The rectangle {nameof(cropRectangle)} {cropRectangle} is outsite the boundaries of the image bound rectangle {Bounds}.", nameof(cropRectangle));
            return new Image(NativeWrappers.gdImageCrop(ImagePtr, &cropRectangle));
        }

        public Image Crop(CropMode mode)
        {
            CheckObjectDisposed();
            if (mode < CropMode.Default || mode > CropMode.Threshold)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid crop mode.");
            return new Image(NativeWrappers.gdImageCropAuto(ImagePtr, mode));
        }

        public Image Crop(Color color, float threshold)
        {
            CheckObjectDisposed();
            var c = ResolveColor(color);
            return new Image(NativeWrappers.gdImageCropThreshold(ImagePtr, c, threshold));
        }

        public Image Resize(Size newSize, InterpolationMethod method = InterpolationMethod.Default)
        {
            if (newSize == Size)
                return Clone();
            if (newSize.Width <= 0 || newSize.Height <= 0)
                throw new ArgumentException($"Invalid image size {newSize}.", nameof(newSize));
            if (method < InterpolationMethod.Default || method >= InterpolationMethod.Invalid)
                throw new ArgumentOutOfRangeException(nameof(method), method, $"Invalid interpolation method.");
            CheckObjectDisposed();

            NativeWrappers.gdImageSetInterpolationMethod(ImagePtr, method);
            return new Image(NativeWrappers.gdImageScale(ImagePtr, (uint)newSize.Width, (uint)newSize.Height));
        }

        public Image Rotate(float angle, Color bgColor, InterpolationMethod method = InterpolationMethod.Default)
        {
            if (method < InterpolationMethod.Default || method >= InterpolationMethod.Invalid)
                throw new ArgumentOutOfRangeException(nameof(method), method, $"Invalid interpolation method.");
            CheckObjectDisposed();
            var c = ResolveColor(bgColor);
            NativeWrappers.gdImageSetInterpolationMethod(ImagePtr, method);
            return new Image(NativeWrappers.gdImageRotateInterpolated(ImagePtr, angle, c));
        }
    }
}
