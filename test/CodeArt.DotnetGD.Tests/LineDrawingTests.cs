using CodeArt.DotnetGD;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class LineDrawingTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawLine(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                image.DrawLine(new Point(0, 0), new Point(image.Width - 1, 0), red);
                image.DrawLine(new Point(0, 1), new Point(image.Width - 1, 1), blue);
                
                
                Assert.Equal(red, image.GetPixel(0, 0));
                Assert.Equal(red, image.GetPixel(image.Width - 2, 0));

                Assert.Equal(blue, image.GetPixel(0, 1));
                Assert.Equal(blue, image.GetPixel(image.Width - 2, 1));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedLine(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                var red = new Color(0xff, 0, 0);
                var blue = new Color(0, 0, 0xff);

                var pen = new Pen(3, new [] { red, red, red, blue, blue});
                
                image.DrawLine(new Point(0, 0), new Point(image.Width - 1, 0), pen);

                Assert.Equal(red, image.GetPixel(0, 0));
                Assert.Equal(red, image.GetPixel(1, 0));
                Assert.Equal(red, image.GetPixel(2, 0));
                Assert.Equal(blue, image.GetPixel(3, 0));
                Assert.Equal(blue, image.GetPixel(4, 0));
                Assert.Equal(red, image.GetPixel(5, 0));

                Assert.Equal(red, image.GetPixel(0, 1));
                Assert.Equal(red, image.GetPixel(1, 1));
                Assert.Equal(red, image.GetPixel(2, 1));
                Assert.Equal(blue, image.GetPixel(3, 1));
                Assert.Equal(blue, image.GetPixel(4, 1));
                Assert.Equal(red, image.GetPixel(5, 1));
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedLine(PixelFormat format)
        {
            using (var image = new Image(20, 20, format))
            {
                using (var brush = new Image(3, 3))
                {
                    var red = new Color(0xff, 0, 0);
                    var blue = new Color(0, 0, 0xff);
                    for (var i = 0; i < brush.Width; i++)
                    {
                        for (var j = 0; j < brush.Height; j++)
                        {
                            var color = (j + i * brush.Width)%2 == 1 ? red : blue;
                            brush.SetPixel(i, j, color);
                        }
                    }

                    image.DrawLine(new Point(0, 1), new Point(15, 1), brush);

                    for (var i = 0; i < 17; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            var color = i < 15 ? (j % 2 == 0 ? blue : red)  : ((j + i * brush.Width) % 2 == 1 ? red : blue);
                            Assert.Equal(color, image.GetPixel(i, j));
                        }
                    }

                }
            }
        }

    }
}
