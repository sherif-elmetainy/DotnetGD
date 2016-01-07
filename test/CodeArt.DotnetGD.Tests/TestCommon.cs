using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using CodeArt.DotnetGD.Formatters;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public static class TestCommon
    {
        public static Image CreateImageWhiteBackground(PixelFormat format = PixelFormat.Format32BppArgb, int width = 400,
            int height = 300)
        {
            var image = new Image(width, height, format);
            image.DrawFilledRectangle(new Rectangle(0, 0, width - 1, height - 1), Color.White);

            return image;
        }

        public static Image CreateCheckerTile()
        {
            var tile = new Image(2, 2);
            tile.SetPixel(0, 0, Color.Red);
            tile.SetPixel(1, 1, Color.Red);
            tile.SetPixel(0, 1, Color.Blue);
            tile.SetPixel(1, 0, Color.Blue);
            return tile;
        }

        public static Image CreateRgbBrush()
        {
            var brush = new Image(1, 3);
            brush.SetPixel(0, 0, Color.Red);
            brush.SetPixel(0, 1, Color.Green);
            brush.SetPixel(0, 2, Color.Blue);
            return brush;
        }

        internal static Image GetTestImage(string name)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var imagePath = Path.Combine(basePath, "TestImages", name);
            return ImageFormatter.ReadImageFromFile(imagePath);
        }

        private static void CompareToReferenceImageInternal(Image image, string referenceImage)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var formatter = new PngImageFormatter();
            var imagePath = Path.Combine(basePath, "ReferenceImages", Path.ChangeExtension(referenceImage, formatter.DefaultExtension));

            if (!File.Exists(imagePath))
            {
                var dir = Path.GetDirectoryName(imagePath);
                Debug.Assert(dir != null, "dir != null");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                // Reference image does not exist. This means this is the first time the test is run, in this case generate the reference image and it has to be looked at manually.
                formatter.WriteImageToFile(image, imagePath);
            }
            else
            {
                // Image already exists, meaning the image was manually verified before, and the test is being run to verify that a change didn't break anything
                using (var otherImage = formatter.ReadImageFromFile(imagePath))
                {
                    var result = otherImage.CompareTo(image);
                    Assert.Equal(ImageCompareResult.Similar, result);
                }
            }
        }

        public static void CompareToReferenceImage(this Image image, string testParams = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string testName = null)
        {
            var className = Path.GetFileNameWithoutExtension(callerFile);
            Debug.Assert(className != null, "className != null");
            var referenceImage = Path.Combine(className, $"{testName}_{testParams}");
            CompareToReferenceImageInternal(image, referenceImage);
        }
    }
}
