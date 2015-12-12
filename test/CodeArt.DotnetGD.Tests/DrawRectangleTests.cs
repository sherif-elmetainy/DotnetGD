using CodeArt.DotnetGD;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawRectangleTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawRectangle(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                var rectangle = new Rectangle(0, 0, image.Width - 1, image.Height - 1);
                image.DrawRectangle(rectangle, red);
                var rectangle2 = new Rectangle(1, 1, image.Width - 3, image.Height - 3);
                image.DrawFilledRectangle(rectangle2, blue);

                Assert.Equal(red, image.GetPixel(0, 0));
                Assert.Equal(blue, image.GetPixel(1, 1));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedRectangle(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                var pen = new Pen(1, new[] { red, red, red, blue, blue });

                var rectangle = new Rectangle(0, 0, image.Width - 1, image.Height - 1);
                image.DrawRectangle(rectangle, pen);
                
                
                Assert.Equal(red, image.GetPixel(0, 0));
                Assert.Equal(blue, image.GetPixel(4, 0));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawTiledRectangle(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);
                
                using (var tile = new Image(2, 2))
                {
                    tile.SetPixel(0, 0, red);
                    tile.SetPixel(1, 1, red);
                    tile.SetPixel(0, 1, blue);
                    tile.SetPixel(1, 0, blue);
                    var rectangle = new Rectangle(0, 0, image.Width - 1, image.Height - 1);
                    image.DrawFilledRectangle(rectangle, tile);
                }


                Assert.Equal(image.GetPixel(0, 0), red);
                Assert.Equal(image.GetPixel(1, 1), red);
                Assert.Equal(image.GetPixel(0, 1), blue);
                Assert.Equal(image.GetPixel(1, 0), blue);
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedRectangle(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                using (var brush = new Image(1, 3))
                {
                    brush.SetPixel(0, 0, red);
                    brush.SetPixel(0, 1, blue);
                    brush.SetPixel(0, 2, red);

                    var rectangle = new Rectangle(1, 1, image.Width - 3, image.Height - 3);
                    image.DrawRectangle(rectangle, brush);
                }

                
                Assert.Equal(red, image.GetPixel(1, 0));
                Assert.Equal(blue, image.GetPixel(2, 1));
               
            }
        }
    }
}
