using System;
using System.IO;
using CodeArt.DotnetGD.Formatters;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawTextTests
    {
        
        [Theory]
        [InlineData("Hello world!", "helloworld_en.png", DrawStringFlags.Default)]
        [InlineData("مرحبا بالعالم!", "helloworld_ar.png", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping)]
        [InlineData("مَرْحَبَاً بِالْعَالَمِ!", "helloworld_tashkeel_ar.png", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping)]
        [InlineData("مَرْحَبَاً بِالْعَالَمِ!", "helloworld_ar.png", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping | DrawStringFlags.RemoveArabicTashkeel)]
        public void DrawText(string text, string referenceImage, DrawStringFlags flags)
        {
            using (var image = new Image(400, 400))
            {
                var white = new Color(0xff, 0xff, 0xff);
                var black = new Color(0, 0, 0);
                image.DrawFilledRectangle(new Rectangle(0, 0, image.Width - 1, image.Height - 1), white);
                image.DrawString(text, new Point(40, 40), "DejaVuSans", 12, 0, black, flags);

                var png = new PngImageFormatter();
                
                using (var reference = png.ReadImageFromFile(referenceImage))
                {
                    var result = image.CompareTo(reference);
                    Assert.Equal(ImageCompareResult.Similar, result);
                }
            }
        }

        [Fact]
        public void DrawTextInvalidFont()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                using (var image = new Image(400, 400))
                {
                    var white = new Color(0xff, 0xff, 0xff);
                    var black = new Color(0, 0, 0);
                    image.DrawFilledRectangle(new Rectangle(0, 0, image.Width - 1, image.Height - 1), white);
                    var f = Guid.NewGuid().ToString("n");

                    image.DrawString(f, new Point(40, 40), f,  12, 0, black);
                }
            });
        }
    }
}
