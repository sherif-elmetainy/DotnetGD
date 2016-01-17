// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public static class SizeExtensions
    {
        /// <summary>
        /// Inflate a size by given size in both directions (width and height are increased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Size Inflate(this Size size, Size by) => size + by * 2;
        /// <summary>
        /// Inflate a size by given size in both directions (width and height are increased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Size Inflate(this Size size, int width, int height) => new Size(size.Width + width * 2, size.Height + height * 2);
        /// <summary>
        /// Deflate a size by given size in both directions (width and height are decreased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Size Deflate(this Size size, Size by) => size - by * 2;
        /// <summary>
        /// Deflate a size by given size in both directions (width and height are decreased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Size Deflate(this Size size, int width, int height) => new Size(size.Width - width * 2, size.Height - height * 2);
        /// <summary>
        /// Subtract 2 sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Size Subtract(this Size s1, Size s2) => s1 - s2;
        /// <summary>
        /// Adds two sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Size Add(this Size s1, Size s2) => s1 + s2;
        /// <summary>
        /// Multiplies a size by a scale
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Size Multiply(this Size s, int scale) => s*scale;
        /// <summary>
        /// Divide a size by a scale
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Size Divide(this Size s, int scale) => s / scale;


        /// <summary>
        /// Inflate a size by given size in both directions (width and height are increased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static SizeF Inflate(this SizeF size, SizeF by) => size + by;
        /// <summary>
        /// Inflate a size by given size in both directions (width and height are increased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SizeF Inflate(this SizeF size, double width, double height) => new SizeF(size.Width + width, size.Height + height);
        /// <summary>
        /// Deflate a size by given size in both directions (width and height are decreased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static SizeF Deflate(this SizeF size, SizeF by) => size - by;
        /// <summary>
        /// Deflate a size by given size in both directions (width and height are decreased by other size width and height multiplied by 2)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SizeF Deflate(this SizeF size, double width, double height) => new SizeF(size.Width - width, size.Height - height);
        /// <summary>
        /// Subtract 2 sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SizeF Subtract(this SizeF s1, SizeF s2) => s1 - s2;

        /// <summary>
        /// Adds two sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SizeF Add(this SizeF s1, SizeF s2) => s1 + s2;
        /// <summary>
        /// Multiplies a size by a scale
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static SizeF Multiply(this SizeF s, double scale) => s * scale;
        /// <summary>
        /// Divide a size by a scale
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static SizeF Divide(this SizeF s, double scale) => s / scale;

        /// <summary>
        /// Rounds a size by converting width and height to the nearest integer values
        /// </summary>
        /// <param name="s"></param>
        /// <param name="mode">rounding moed</param>
        /// <returns></returns>
        public static Size Round(this SizeF s, MidpointRounding mode = MidpointRounding.ToEven) => new Size((int)Math.Round(s.Width, mode), (int)Math.Round(s.Height, mode));

        /// <summary>
        /// Rounds a size 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="digits">decimal digits</param>
        /// <param name="mode">rounding mode</param>
        /// <returns></returns>
        public static SizeF Round(this SizeF s, int digits, MidpointRounding mode = MidpointRounding.ToEven) => new SizeF(Math.Round(s.Width, digits, mode), Math.Round(s.Height, digits, mode));

        /// <summary>
        /// Rounds a size by converting width and height to the next lower integer values
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Size Floor(this SizeF s) => new Size((int)Math.Floor(s.Width), (int)Math.Floor(s.Height));

        /// <summary>
        /// Rounds a size by removing decimal points from width and height 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Size Truncate(this SizeF s) => new Size((int)s.Width, (int)s.Height);

        /// <summary>
        /// Rounds a size by converting width and height to the next heigher integer values
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Size Ceiling(this SizeF s) => new Size((int)Math.Ceiling(s.Width), (int)Math.Ceiling(s.Height));
    }
}
