using System;
using System.IO;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawTextTests
    {
        
        [Theory]
        [InlineData("Hello world!", "HelloWorldEn", DrawStringFlags.Default)]
        [InlineData("مرحبا بالعالم!", "HelloWorldAr", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping)]
        [InlineData("مَرْحَبَاً بِالْعَالَمِ!", "HelloWorldArTashkeel", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping)]
        [InlineData("مَرْحَبَاً بِالْعَالَمِ!", "HelloWorldArRemoveTashkeel", DrawStringFlags.RunBidi | DrawStringFlags.ArabicShaping | DrawStringFlags.RemoveArabicTashkeel)]
        public void DrawText(string text, string referenceImage, DrawStringFlags flags)
        {
            using (var image = TestCommon.CreateImageWhiteBackground())
            {
                image.DrawString(text, new Point(40, 40), Path.Combine("Fonts", "DejaVuSans"), 12, 0, Color.Black, flags);
                image.CompareToReferenceImage(referenceImage);
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
