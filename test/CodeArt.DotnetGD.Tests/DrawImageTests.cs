// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawImageTests
    {
        [Theory]
        [InlineData("Test_1.png")]
        public void DrawImage(string fileName)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                var srcRectangle = new Rectangle(0, 0, image.Width, image.Height);
                using (var targetImage = TestCommon.CreateImageWhiteBackground(width: image.Width*2, height: image.Height*2))
                {
                    targetImage.DrawImage(image, new Point(0, 0), srcRectangle);
                    targetImage.DrawImage(image, new Point(image.Width, 0), srcRectangle);
                    targetImage.DrawImage(image, new Point(0, image.Height), srcRectangle);
                    targetImage.DrawImage(image, new Point(image.Width, image.Height), srcRectangle);

                    targetImage.CompareToReferenceImage(fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png", 0)]
        [InlineData("Test_1.png", 25)]
        [InlineData("Test_1.png", 50)]
        [InlineData("Test_1.png", 75)]
        [InlineData("Test_1.png", 100)]
        public void DrawImageMerge(string fileName, int mergePercentage)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                var srcRectangle = new Rectangle(0, 0, image.Width, image.Height);
                using (var targetImage = TestCommon.CreateImageWhiteBackground(width: image.Width * 2, height: image.Height * 2))
                {
                    targetImage.DrawFilledRectangle(targetImage.Bounds, Color.Yellow);

                    targetImage.DrawImage(image, new Point(0, 0), srcRectangle, mergePercentage);
                    targetImage.DrawImage(image, new Point(image.Width, 0), srcRectangle, mergePercentage);
                    targetImage.DrawImage(image, new Point(0, image.Height), srcRectangle, mergePercentage);
                    targetImage.DrawImage(image, new Point(image.Width, image.Height), srcRectangle, mergePercentage);

                    targetImage.CompareToReferenceImage(mergePercentage + "_" + fileName);
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png", 0)]
        [InlineData("Test_1.png", 25)]
        [InlineData("Test_1.png", 50)]
        [InlineData("Test_1.png", 75)]
        [InlineData("Test_1.png", 100)]
        public void DrawImageMergeGray(string fileName, int mergePercentage)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                var srcRectangle = new Rectangle(0, 0, image.Width, image.Height);
                using (var targetImage = TestCommon.CreateImageWhiteBackground(width: image.Width * 2, height: image.Height * 2))
                {
                    targetImage.DrawFilledRectangle(targetImage.Bounds, Color.Yellow);
                    targetImage.DrawImageGray(image, new Point(0, 0), srcRectangle, mergePercentage);
                    targetImage.DrawImageGray(image, new Point(image.Width, 0), srcRectangle, mergePercentage);
                    targetImage.DrawImageGray(image, new Point(0, image.Height), srcRectangle, mergePercentage);
                    targetImage.DrawImageGray(image, new Point(image.Width, image.Height), srcRectangle, mergePercentage);

                    targetImage.CompareToReferenceImage(mergePercentage + "_" + fileName);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DrawImage(bool resample)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(PixelFormat.Format32BppArgb, 800, 600))
            {
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawEllipse(center, size, Color.Red);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), Color.Blue);

                using (var targetImage = new Image(image.Size/2))
                {
                    targetImage.DrawImage(image, targetImage.Bounds, image.Bounds, resample);
                    targetImage.CompareToReferenceImage(resample ? "Resample" : "Resize");
                }
            }
        }

        [Theory]
        [InlineData("Test_1.png", 0)]
        [InlineData("Test_1.png", 45)]
        [InlineData("Test_1.png", 90)]
        [InlineData("Test_1.png", 135)]
        [InlineData("Test_1.png", 180)]
        [InlineData("Test_1.png", 225)]
        [InlineData("Test_1.png", 270)]
        [InlineData("Test_1.png", 315)]
        public void DrawImageRotated(string fileName, int angle)
        {
            using (var image = TestCommon.GetTestImage(fileName))
            {
                
                using (var targetImage = TestCommon.CreateImageWhiteBackground(PixelFormat.Format32BppArgb, image.Width * 2, image.Height * 2))
                {
                    targetImage.DrawImageRotated(image, image.Width, image.Height, image.Bounds, angle);
                    targetImage.CompareToReferenceImage(angle + "_" + fileName);
                }
            }
        }
    }
}
