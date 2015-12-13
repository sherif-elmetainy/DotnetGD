using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawEllipseTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawEllipse(PixelFormat format)
        {
            using (var image = new Image(21, 21, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                var center = new Point(10, 10);
                var size = new Size(20, 20);
                image.DrawEllipse(center, size, red);
                image.DrawFilledEllipse(center, new Size(19, 19), blue);
                
                Assert.Equal(red, image.GetPixel(10, 0));
                Assert.Equal(blue, image.GetPixel(10, 1));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedEllipse(PixelFormat format)
        {
            using (var image = new Image(21, 21, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                var pen = new Pen(1, new[] { red, red, red, blue, blue });
                var center = new Point(10, 10);
                var size = new Size(20, 20);
                image.DrawEllipse(center, size, pen);

                
                Assert.Equal(red, image.GetPixel(9, 0));
                Assert.Equal(blue, image.GetPixel(11, 0));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawTiledEllipse(PixelFormat format)
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
                    var center = new Point(10, 10);
                    var size = new Size(20, 20);
                    image.DrawFilledEllipse(center, size, tile);
                }

               

                Assert.Equal(red, image.GetPixel(10, 0));
                Assert.Equal(blue, image.GetPixel(9, 0));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedEllipse(PixelFormat format)
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

                    var center = new Point(10, 10);
                    var size = new Size(20, 20);
                    image.DrawEllipse(center, size, brush);
                }

                Assert.Equal(red, image.GetPixel(10, 1));
                Assert.Equal(blue, image.GetPixel(10, 0));
               
            }
        }
    }
}
