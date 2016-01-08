using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class ReplaceColorTests
    {
        [Theory]
        [PixelFormatsData]
        public void ReplaceColors(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawEllipse(center, size, Color.Red);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), Color.Blue);

                image.ReplacePixels(Color.Red, Color.Yellow);
                image.ReplacePixels(Color.Blue, Color.Green);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void ReplaceColorsThreshold(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawEllipse(center, size, Color.Red);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), Color.Blue);

                image.ReplacePixels(Color.DarkRed, Color.Yellow, 20.0f);
                image.ReplacePixels(Color.DarkBlue, Color.Green, 20.0f);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void ReplaceColorsArray(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawEllipse(center, size, Color.Red);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), Color.Blue);

                image.ReplacePixels(new[] { Color.Red, Color.Blue}, new[] { Color.Yellow, Color.Green });
                image.CompareToReferenceImage(format.ToString());
            }
        }
    }
}
