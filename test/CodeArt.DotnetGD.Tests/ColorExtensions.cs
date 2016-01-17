using System;

namespace CodeArt.DotnetGD.Tests
{
    public static class ColorExtensions
    {
        public static float GetSaturation(this Color color)
        {
            var g = color.G / 255f;
            var b = color.B / 255f;
            var r = color.R / 255f;
            var cmax = Math.Max(Math.Max(r, g), b);
            var cmin = Math.Min(Math.Min(r, g), b);
            var delta = cmax - cmin;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return delta == 0 ? 0 : delta / (1 - Math.Abs(cmax + cmin - 1));
        }

        public static float GetBrightness(this Color color)
        {
            var g = color.G / 255f;
            var b = color.B / 255f;
            var r = color.R / 255f;
            var cmax = Math.Max(Math.Max(r, g), b);
            var cmin = Math.Min(Math.Min(r, g), b);
            return (cmax + cmin) / 2f;
        }

        public static float GetHue(this Color color)
        {
            if (color.B == color.G && color.R == color.G)
                return 0f;

            var g = color.G / 255f;
            var b = color.B / 255f;
            var r = color.R / 255f;
            var cmax = Math.Max(Math.Max(r, g), b);
            var cmin = Math.Min(Math.Min(r, g), b);
            var delta = cmax - cmin;
            float res;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (cmax == r)
            {
                res = (g - b) / delta;
            }
            else if (cmax == g)
            {
                res = (b - r) / delta + 2;
            }
            else
            {
                res = (r - g) / delta + 4;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator
            res = res * 60f;
            if (res < 0)
                res += 360f;
            return res;
        }
    }
}
