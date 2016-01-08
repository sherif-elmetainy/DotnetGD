// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawEllipseTests
    {
        [Theory]
        [PixelFormatsData]
        public void DrawEllipse(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            { 
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawEllipse(center, size, Color.Red);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), Color.Blue);
                
                image.CompareToReferenceImage(format.ToString());
            }
        }

        // Removed because style works with line drawing functions (gdImageLine, gdImageRectangle, gdImagePolygon, etc)
        //[Theory]
        //[PixelFormatsData]
        //public void DrawDashedEllipse(PixelFormat format)
        //{
        //    Assert.True(false, "Need to check libgd Styled drawing for Ellipses");
        //    using (var image = TestCommon.CreateImageWhiteBackground(format))
        //    {
        //        var center = new Point(image.Width / 2, image.Height / 2);
        //        var size = new Size(image.Width - 30, image.Height - 30);
        //        var pen = new Pen(4, new[] { Color.Red, Color.Blue });
        //        image.DrawEllipse(center, size, pen);
        //        image.CompareToReferenceImage(format.ToString());
        //    }
        //}

        [Theory]
        [PixelFormatsData]
        public void DrawTiledEllipse(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    var center = new Point(image.Width / 2, image.Height / 2);
                    var size = new Size(image.Width - 30, image.Height - 30);
                    image.DrawFilledEllipse(center, size, tile);
                }
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawBrushedEllipse(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var brush = TestCommon.CreateRgbBrush())
                {
                    var center = new Point(image.Width / 2, image.Height / 2);
                    var size = new Size(image.Width - 30, image.Height - 30);
                    image.DrawEllipse(center, size, brush);
                }

                image.CompareToReferenceImage(format.ToString());
            }
        }
    }
}
