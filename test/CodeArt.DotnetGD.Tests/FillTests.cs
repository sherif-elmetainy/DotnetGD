using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class FillTests
    {
        [Theory]
        [PixelFormatsData]
        public void Fill(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                image.Fill(new Point(10, 10), Color.Orange);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void FillTile(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    image.Fill(new Point(10, 10), tile);
                    image.CompareToReferenceImage(format.ToString());
                }
            }
        }

        [Theory]
        [PixelFormatsData]
        public void FillBorder(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                image.DrawRectangle(new Rectangle(1, 1, 200, 200), Color.Blue);
                image.Fill(new Point(10, 10), Color.Orange, Color.Blue);
                image.CompareToReferenceImage(format.ToString());
            }
        }

        [Theory]
        [PixelFormatsData]
        public void FillBorderTile(PixelFormat format)
        {
            using (var image = TestCommon.CreateImageWhiteBackground(format))
            {
                using (var tile = TestCommon.CreateCheckerTile())
                {
                    image.DrawRectangle(new Rectangle(1, 1, 200, 200), Color.Blue);
                    image.Fill(new Point(10, 10), tile, Color.Blue);
                    image.CompareToReferenceImage(format.ToString());
                }
            }
        }
    }
}
