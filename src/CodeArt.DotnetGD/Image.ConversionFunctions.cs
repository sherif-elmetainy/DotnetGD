// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        /// <summary>
        /// Clones the image. The target image will have same size, same number of colors (in case of 8-bit indexed) and same pixels.
        /// </summary>
        /// <returns></returns>
        public Image Clone()
        {
            CheckObjectDisposed();
            var ptr = NativeWrappers.gdImageClone(ImagePtr);
            return new Image(ptr);
        }

        /// <summary>
        /// Changes the format of an image. This is an in place replacement. No new image is created.
        /// </summary>
        /// <param name="newPixelFormat">The new image format. If the image format is the same, the method does nothing.</param>
        /// <param name="quantizationMethod">Quantization method to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="quantizationSpeed">Quantization speed to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="dither">Dither flag to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="numberOfColorsWanted">Number of target colors to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        public void ChangeFormat(PixelFormat newPixelFormat,
            PaletteQuantizationMethod quantizationMethod = PaletteQuantizationMethod.Default,
            int quantizationSpeed = PaletteQuantizationSpeedBestQuality,
            //int minQuantizationQuality = PaletteQuantizationUgly,
            //int maxQuantizationQuality = PaletteQuantizationPerfect,
            bool dither = false,
            int numberOfColorsWanted = 256)
        {
            if (newPixelFormat != PixelFormat.Format8BppIndexed && newPixelFormat != PixelFormat.Format32BppArgb)
                throw new ArgumentOutOfRangeException(nameof(newPixelFormat), newPixelFormat, "Invalid pixel format.");
            CheckObjectDisposed();
            if (newPixelFormat == PixelFormat)
                return;
            if (newPixelFormat == PixelFormat.Format8BppIndexed)
            {
                if (quantizationMethod < PaletteQuantizationMethod.Default
                    || quantizationMethod >= PaletteQuantizationMethod.Invalid
                    )
                {
                    throw new ArgumentOutOfRangeException(nameof(quantizationMethod), quantizationMethod,
                        "Invalid quantization method.");
                }
                if (quantizationSpeed < PaletteQuantizationSpeedBestQuality ||
                    quantizationSpeed > PaletteQuantizationSpeedBestSpeed)
                    throw new ArgumentOutOfRangeException(nameof(quantizationSpeed), quantizationSpeed,
                        $"Quantization speed must be from {PaletteQuantizationSpeedBestQuality} to {PaletteQuantizationSpeedBestSpeed}.");
                //if (minQuantizationQuality >= maxQuantizationQuality)
                //    throw new ArgumentException("Minimum quality must be less than maximum quality.",
                //        nameof(minQuantizationQuality));
                //if (minQuantizationQuality < PaletteQuantizationUgly || minQuantizationQuality > PaletteQuantizationPerfect)
                //    throw new ArgumentOutOfRangeException(nameof(minQuantizationQuality), minQuantizationQuality,
                //        $"{nameof(minQuantizationQuality)} must be from {PaletteQuantizationUgly} and {PaletteQuantizationPerfect}.");
                //if (maxQuantizationQuality < PaletteQuantizationUgly || maxQuantizationQuality > PaletteQuantizationPerfect)
                //    throw new ArgumentOutOfRangeException(nameof(maxQuantizationQuality), maxQuantizationQuality,
                //        $"{nameof(maxQuantizationQuality)} must be from {PaletteQuantizationUgly} and {PaletteQuantizationPerfect}.");
                if (numberOfColorsWanted < 1 || numberOfColorsWanted > 256)
                    throw new ArgumentOutOfRangeException(nameof(numberOfColorsWanted), numberOfColorsWanted,
                        "{nameof(numberOfColorsWanted)} must be from 1 and 256.");

                NativeWrappers.gdImageTrueColorToPaletteSetMethod(ImagePtr, (int)quantizationMethod,
                    quantizationSpeed);
                //NativeWrappers.gdImageTrueColorToPaletteSetQuality(ImagePtr, minQuantizationQuality, maxQuantizationQuality);
                NativeWrappers.gdImageTrueColorToPalette(ImagePtr, dither ? 1 : 0, numberOfColorsWanted);
            }
            else
            {
                NativeWrappers.gdImagePaletteToTrueColor(ImagePtr);
            }

            
        }

        /// <summary>
        /// Creates a new image based on the image with a new format.
        /// </summary>
        /// <param name="newPixelFormat">The new image format. If the image format is the same, the method is the same as <see cref="Clone"/>.</param>
        /// <param name="quantizationMethod">Quantization method to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="quantizationSpeed">Quantization speed to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="dither">Dither flag to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        /// <param name="numberOfColorsWanted">Number of target colors to use when converting from truecolor to 8-bit. This is ignored when converting to true color.</param>
        public Image Copy(PixelFormat newPixelFormat, PaletteQuantizationMethod quantizationMethod = PaletteQuantizationMethod.Default, 
            int quantizationSpeed = PaletteQuantizationSpeedBestQuality,
            //int minQuantizationQuality = PaletteQuantizationUgly, int maxQuantizationQuality = PaletteQuantizationPerfect,
            bool dither = false,
            int numberOfColorsWanted = 256
            )
        {
            if (PixelFormat == newPixelFormat)
                return Clone();
            var newImage = Clone();
            try
            {
                newImage.ChangeFormat(newPixelFormat, quantizationMethod, quantizationSpeed /*, minQuantizationQuality, maxQuantizationQuality */, dither, numberOfColorsWanted);
                return newImage;
            }
            catch
            {
                newImage.Dispose();
                throw;
            }
            
        }

        //public void ColorMatch(Image other)
        //{
        //    if (other == null)
        //        throw new ArgumentNullException(nameof(other));
        //    CheckObjectDisposed();
        //    other.CheckObjectDisposed();
        //    if (other.PixelFormat != PixelFormat.Format8BppIndexed)
        //        throw new ArgumentException($"Other image must have {PixelFormat.Format8BppIndexed} pixel format.", nameof(other));
        //    if (PixelFormat != PixelFormat.Format8BppIndexed)
        //        throw new InvalidOperationException($"Image must have {PixelFormat.Format8BppIndexed} pixel format.");
        //    if (Size != other.Size)
        //        throw new ArgumentException("Other image has different size.", nameof(other));
        //    NativeWrappers.gdImageColorMatch(other.ImagePtr, ImagePtr);
        //}
    }
}
