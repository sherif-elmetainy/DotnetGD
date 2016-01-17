// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// whether the sizes are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Whether the objects are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return Equals((Size)obj);
            }
            return false;
        }

        /// <summary>
        /// Hash code of the size
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + Width;
                hash = hash * 31 + Height;
                return hash;
            }
        }

        /// <summary>
        /// String representation of the size
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({Width}, {Height})";
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(Size s1, Size s2) => s1.Equals(s2);
        /// <summary>
        /// Inequality comparison
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator !=(Size s1, Size s2) => !s1.Equals(s2);

        /// <summary>
        /// Adds two sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Size operator +(Size s1, Size s2) => new Size(s1.Width + s2.Width, s1.Height + s2.Height);
        /// <summary>
        /// Substract 2 sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Size operator -(Size s1, Size s2) => new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        /// <summary>
        /// Multiplies a size by  scale (both width and height are multiplied)
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Size operator *(int scale, Size s) => new Size(scale * s.Width, scale * s.Height);
        /// <summary>
        /// Multiplies a size by  scale (both width and height are multiplied)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Size operator *(Size s, int scale) => new Size(s.Width * scale, s.Height * scale);
        /// <summary>
        /// divides a size by  scale (both width and height are divided)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Size operator /(Size s, int scale) => new Size(s.Width / scale, s.Height / scale);
    }
}
