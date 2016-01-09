using System.Runtime.InteropServices;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawArcTests
    {
        [Theory]
        [InlineData(ArcStyles.Arc)]
        [InlineData(ArcStyles.Chord)]
        [InlineData(ArcStyles.NoFill)]
        [InlineData(ArcStyles.NoFill | ArcStyles.Chord)]
        [InlineData(ArcStyles.Edged)]
        public void DrawFilledArc(ArcStyles style)
        {
            using (var image = TestCommon.CreateImageWhiteBackground())
            {
                image.DrawFilledArc(new Point(image.Width / 2, image.Height / 2), image.Size.Deflate(20, 20), 0, 135, Color.Red, style);
                image.CompareToReferenceImage(style.ToString());
            }
        }

        [Theory]
        [InlineData(ArcStyles.Arc)]
        [InlineData(ArcStyles.Chord)]
        [InlineData(ArcStyles.NoFill)]
        [InlineData(ArcStyles.NoFill | ArcStyles.Chord)]
        [InlineData(ArcStyles.Edged)]
        public void DrawFilledArcTiled(ArcStyles style)
        {
            using (var image = TestCommon.CreateImageWhiteBackground())
            {
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    image.DrawFilledArc(new Point(image.Width/2, image.Height/2), image.Size.Deflate(20, 20), 0, 135, tile, style);
                    image.CompareToReferenceImage(style.ToString());
                }
            }
        }


        [Theory]
        [PixelFormatsData]
        public void DrawArc(PixelFormat pixelFormat)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(pixelFormat))
            {
                image.DrawArc(new Point(image.Width / 2, image.Height / 2), image.Size.Deflate(20, 20), 0, 135, Color.Red);
                image.CompareToReferenceImage(pixelFormat.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawArcThick(PixelFormat pixelFormat)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(pixelFormat))
            {
                image.DrawArc(new Point(image.Width / 2, image.Height / 2), image.Size.Deflate(20, 20), 0, 135, new Pen(5, Color.Red));
                image.CompareToReferenceImage(pixelFormat.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void DrawArcBrush(PixelFormat pixelFormat)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(pixelFormat))
            {
                using (var brush = TestCommon.CreateRgbBrush())
                {
                    image.DrawArc(new Point(image.Width/2, image.Height/2), image.Size.Deflate(20, 20), 0, 135, brush);
                    image.CompareToReferenceImage(pixelFormat.ToString());
                }
            }
        }
    }
}
