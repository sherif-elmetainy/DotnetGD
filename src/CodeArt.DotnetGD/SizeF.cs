// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public struct SizeF : IEquatable<SizeF>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public SizeF(double width, double height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// whether the sizes are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SizeF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Whether the objects are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is SizeF)
            {
                return Equals((SizeF)obj);
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
                hash = hash * 31 + Width.GetHashCode();
                hash = hash * 31 + Height.GetHashCode();
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
        public static bool operator ==(SizeF s1, SizeF s2) => s1.Equals(s2);
        /// <summary>
        /// Inequality comparison
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator !=(SizeF s1, SizeF s2) => !s1.Equals(s2);

        /// <summary>
        /// Adds two sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SizeF operator +(SizeF s1, SizeF s2) => new SizeF(s1.Width + s2.Width, s1.Height + s2.Height);
        /// <summary>
        /// Substract 2 sizes
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SizeF operator -(SizeF s1, SizeF s2) => new SizeF(s1.Width - s2.Width, s1.Height - s2.Height);
        /// <summary>
        /// Multiplies a size by  scale (both width and height are multiplied)
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static SizeF operator *(double scale, SizeF s) => new SizeF(scale * s.Width, scale * s.Height);
        // <summary>
        /// Multiplies a size by  scale (both width and height are multiplied)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static SizeF operator *(SizeF s, double scale) => new SizeF(s.Width * scale, s.Height * scale);
        /// <summary>
        /// divides a size by  scale (both width and height are divided)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static SizeF operator /(SizeF s, double scale) => new SizeF(s.Width / scale, s.Height / scale);
        /// <summary>
        /// implicit conversion from <see cref="Size"/> to <see cref="SizeF"/>
        /// </summary>
        /// <param name="size"></param>
        public static implicit operator SizeF(Size size) => new SizeF(size.Width, size.Height);
        /// <summary>
        /// Rounds the <see cref="SizeF"/> to <see cref="Size"/>
        /// </summary>
        /// <param name="size"></param>
        public static explicit operator Size(SizeF size) => new Size((int) size.Width, (int)size.Height);
        
        /// <summary>
        /// Whether the sizes are similar. This differs from <see cref="Equals(CodeArt.DotnetGD.SizeF)"/> in that numbers within 1e-6 of each other are considered simiar.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool AreSimilar(SizeF s1, SizeF s2)
            => Math.Abs(s1.Width - s2.Width) < 1e-6 && Math.Abs(s1.Height - s2.Height) < 1e-6;

    }
}
