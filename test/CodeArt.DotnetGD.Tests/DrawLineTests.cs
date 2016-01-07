using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawLineTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawLine(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                image.DrawLine(new Point(0, 0), new Point(image.Width - 1, 0), Color.Red);
                image.DrawLine(new Point(0, 1), new Point(image.Width - 1, 1), Color.Blue);
                
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedLine(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                
                var pen = new Pen(3, new [] { Color.Red, Color.Red, Color.Red, Color.Blue});
                
                image.DrawLine(new Point(0, 3), new Point(image.Width - 1, 3), pen);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedLine(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var brush = TestCommon.CreateRgbBrush())
                {
                    image.DrawLine(new Point(3, 3), new Point(image.Width - 3, 3), brush);
                    image.DrawLine(new Point(3, 30), new Point(image.Width - 3, 30), brush);
                    image.CompareToReferenceImage(format.ToString());
                }
            }
        }

    }
}
