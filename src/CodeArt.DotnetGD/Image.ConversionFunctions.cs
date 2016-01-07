using System;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public unsafe partial class Image
    {
        public Image Clone()
        {
            var ptr = NativeWrappers.gdImageClone(ImagePtr);
            return new Image(ptr);
        }

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

        
    }
}
