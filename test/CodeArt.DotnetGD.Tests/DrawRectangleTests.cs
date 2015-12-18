using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawRectangleTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawRectangle(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var rectangle = new Rectangle(0, 0, image.Width - 1, image.Height - 1);
                image.DrawRectangle(rectangle, Color.Red);
                var rectangle2 = new Rectangle(1, 1, image.Width - 3, image.Height - 3);
                image.DrawFilledRectangle(rectangle2, Color.Blue);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedRectangle(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var rectangle = new Rectangle(3, 3, image.Width - 6, image.Height - 6);
                var pen = new Pen(1, new[] { Color.Red, Color.Red, Color.Red, Color.Red, Color.Blue, Color.Blue });
                image.DrawRectangle(rectangle, pen);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawTiledRectangle(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    var rectangle = new Rectangle(0, 0, image.Width - 1, image.Height - 1);
                    image.DrawFilledRectangle(rectangle, tile);
                }

                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedRectangle(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {

                using (var brush = TestCommon.CreateRgbBrush())
                {
                    var rectangle = new Rectangle(3, 3, image.Width - 6, image.Height - 6);
                    image.DrawRectangle(rectangle, brush);
                }

                image.CompareToReferenceImage(format.ToString());
            }
        }
    }
}
