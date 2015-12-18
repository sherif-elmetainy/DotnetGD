using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawPolygonTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10)};
                image.DrawPolygon(polygon, Color.Red);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawDashedPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10) };
                var pen = new Pen(1, new[] { Color.Red, Color.Red, Color.Red, Color.Red, Color.Blue, Color.Blue });
                image.DrawPolygon(polygon, pen);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10) };
                using (var brush = TestCommon.CreateRgbBrush())
                {
                    image.DrawPolygon(polygon, brush);
                }
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawTiledPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10) };
                    image.DrawFilledPolygon(polygon, tile);
                }
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawFilledPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10) };
                image.DrawFilledPolygon(polygon, Color.Blue);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawOpenPolygon(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                var polygon = new[] { new Point(10, 10), new Point(image.Width - 20, 10), new Point(image.Width / 2, image.Height - 10) };
                image.DrawOpenPolygon(polygon, Color.Blue);
                image.CompareToReferenceImage(format.ToString());
            }
        }
    }
}
