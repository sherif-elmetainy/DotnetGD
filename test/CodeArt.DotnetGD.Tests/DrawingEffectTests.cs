// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class DrawingEffectTests
    {
        [Theory]
        [InlineData(DrawingEffect.Replace)]
        [InlineData(DrawingEffect.AlphaBlend)]
        [InlineData(DrawingEffect.Normal)]
        [InlineData(DrawingEffect.Multiply)]
        [InlineData(DrawingEffect.Overlay)]
        public void TestDrawingEffect(DrawingEffect effect)
        {
            using (var image = TestCommon.CreateImageWhiteBackground())
            {
                var center = new Point(image.Width / 2, image.Height / 2);
                var size = new Size(image.Width - 30, image.Height - 30);
                image.DrawFilledEllipse(center, size.Deflate(20, 20), new Color("#800000ff"));
                image.DrawingEffect = effect;
                image.DrawFilledEllipse(center, size.Deflate(20, 20), new Color("#80ff0000"));
                image.CompareToReferenceImage(effect.ToString());
            }
        }
    }
}
