// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Sets a pixel in an image to a specified color
        /// </summary>
        /// <param name="image">image</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="color">color</param>
        public static void SetPixel(this Image image, int x, int y, Color color)
            => image.SetPixel(new Point(x, y), color);

        /// <summary>
        /// Gets the color pixel at the specified point.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetPixel(this Image image, int x, int y)
            => image.GetPixel(new Point(x, y));
    }
}
