using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class ProcessingTests
    {
        [Theory]
        [InlineData("Test_1.png", 6, PixelateMode.UpplerLeft)]
        [InlineData("Test_1.png", 6, PixelateMode.Average)]
        [InlineData("Test_1.png", 12, PixelateMode.UpplerLeft)]
        [InlineData("Test_1.png", 12, PixelateMode.Average)]
        public void Pixelate(string fileName, int blockSize, PixelateMode mode)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Pixelate(blockSize, mode);
                image.CompareToReferenceImage($"{blockSize}_{mode}_{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png", -4, 4)]
        [InlineData("Test_1.png", -10, 10)]
        [InlineData("Test_1.png", -8, 8)]
        [InlineData("Test_1.png", -6, 6)]
        public void Scatter(string fileName, int rangeSub, int rangePlus)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Scatter(rangeSub, rangePlus, null, 0);
                image.CompareToReferenceImage($"{rangePlus}_{fileName}");
            }
        }

        //[Theory]
        //[InlineData("Test_1.png", 100f)]
        //[InlineData("Test_1.png", 0.5f)]
        //[InlineData("Test_1.png", 0.75f)]
        //[InlineData("Test_1.png", 1.0f)]
        //public void Smooth(string fileName, float weight)
        //{
        //    using (var image = TestCommon.GetTestImage(fileName))
        //    {
        //        image.Smooth(weight);
        //        image.CompareToReferenceImage($"{weight * 100}_{fileName}");
        //    }
        //}

        [Theory]
        [InlineData("Test_1.png")]
        public void MeanRemoval(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.MeanRemoval();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void Emboss(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Emboss();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void GaussianBlur(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.GaussianBlur();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void EdgeDetect(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.EdgeDetect();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void SelectiveBlur(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.SelectiveBlur();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void GrayScale(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.GrayScale();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void Negate(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Negate();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void FlipVertical(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.FlipVertical();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void FlipHorizontal(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.FlipHorizontal();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void FlipBoth(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.FlipBoth();
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void Crop(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                using (var target = image.Crop(new Rectangle(new Point(), image.Size/2)))
                {
                    target.CompareToReferenceImage($"{fileName}");
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void CropColor(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                var color = image.GetPixel(0, 0);
                using (var target = image.Crop(color, 20f))
                {
                    target.CompareToReferenceImage($"{fileName}");
                }
            }
        }


        [Theory]
        [InlineData("Test_1.png", 1, 2)]
        [InlineData("Test_1.png", 2, 1)]
        public void Resize(string fileName, int mult, int div)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                using (var target = image.Resize(image.Size * mult / div))
                {
                    target.CompareToReferenceImage($"{mult}_{div}_{fileName}");
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png", 45)]
        [InlineData("Test_1.png", 90)]
        [InlineData("Test_1.png", -45)]
        [InlineData("Test_1.png", -90)]
        public void Rotate(string fileName, float angle)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                using (var target = image.Rotate(angle, Color.Wheat))
                {
                    target.CompareToReferenceImage($"{angle}_{fileName}");
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png")]
        public void Convolution(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                var filter = new [,]
                {
                    {1.5f, 0, 0},
                    {0, 0, 0},
                    {0, 0, -1.5f}
                };
                image.Convolution(filter, 1, 127);
                image.CompareToReferenceImage($"{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png", 10.0d)]
        [InlineData("Test_1.png", 20.0d)]
        [InlineData("Test_1.png", 50.0d)]
        [InlineData("Test_1.png", 80.0d)]
        public void Contrast(string fileName, double contrast)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Contrast(contrast);
                image.CompareToReferenceImage($"{contrast}_{fileName}");
            }
        }

        [Theory]
        [InlineData("Test_1.png", -100)]
        [InlineData("Test_1.png", -50)]
        [InlineData("Test_1.png", 50)]
        [InlineData("Test_1.png", 100)]
        public void Brightness(string fileName, int brightness)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                image.Brightness(brightness);
                image.CompareToReferenceImage($"{brightness}_{fileName}");
            }
        }
    }
}
