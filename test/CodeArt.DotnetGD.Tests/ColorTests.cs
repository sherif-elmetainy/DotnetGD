using System;
using Xunit;

namespace CodeArt.DotnetGD.Tests
{
    public class ColorTests
    {
        [Theory]
        [InlineData("black", "#ff000000", 0, 0, 0, 0f, 0f, 0f)]
        [InlineData("white", "#ffffffff", 255, 255, 255, 0f, 0f, 1f)]
        [InlineData("red", "#ffff0000", 255, 0, 0, 0f, 1f, .5f)]
        [InlineData("lime", "#ff00ff00", 0, 255, 0, 120f, 1f, .5f)]
        [InlineData("blue", "#ff0000ff", 0, 0, 255, 240f, 1f, .5f)]
        [InlineData("yellow", "#ffffff00", 255, 255, 0, 60f, 1f, .5f)]
        [InlineData("cyan", "#ff00ffff", 0, 255, 255, 180f, 1f, .5f)]
        [InlineData("magenta", "#ffff00ff", 255, 0, 255, 300f, 1f, .5f)]
        [InlineData("silver", "#ffc0c0c0", 192, 192, 192, 0f, 0f, .75f)]
        [InlineData("gray", "#ff808080", 128, 128, 128, 0f, 0f, .5f)]
        [InlineData("maroon", "#ff800000", 128, 0, 0, 0f, 1f, .25f)]
        [InlineData("olive", "#ff808000", 128, 128, 0, 60, 1f, .25f)]
        [InlineData("green", "#ff008000", 0, 128, 0, 120f, 1f, .25f)]
        [InlineData("purple", "#ff800080", 128, 0, 128, 300f, 1f, .25f)]
        [InlineData("teal", "#ff008080", 0, 128, 128, 180f, 1f, .25f)]
        [InlineData("navy", "#ff000080", 0, 0, 128, 240f, 1f, .25f)]
        public void ColorValues(string name, string expectedHex, int r, int g, int b, float h, float s, float l)
        {
            var color = new Color(name);
            Assert.Equal(expectedHex, color.ToString());
            Assert.Equal(r, color.R);
            Assert.Equal(g, color.G);
            Assert.Equal(b, color.B);
            Assert.Equal(h, color.GetHue());
            Assert.Equal(s, color.GetSaturation(), 2);
            Assert.Equal(l, color.GetBrightness(), 2);

            var color2 = new Color(h, s, l);
            Assert.True(Math.Abs(r - color2.R) <= 1);
            Assert.True(Math.Abs(g - color2.G) <= 1);
            Assert.True(Math.Abs(b - color2.B) <= 1);
        }
    }
}
