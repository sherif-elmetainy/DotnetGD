// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public struct Size : IEquatable<Size>
    {
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return Equals((Size)obj);
            }
            return false;
        }

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

        public override string ToString()
        {
            return $"({Width}, {Height})";
        }

        public static bool operator ==(Size s1, Size s2) => s1.Equals(s2);
        public static bool operator !=(Size s1, Size s2) => !s1.Equals(s2);

        public static Size operator +(Size s1, Size s2) => new Size(s1.Width + s2.Width, s1.Height + s2.Height);
        public static Size operator -(Size s1, Size s2) => new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        public static Size operator *(int scale, Size s) => new Size(scale * s.Width, scale * s.Height);
        public static Size operator *(Size s, int scale) => new Size(s.Width * scale, s.Height * scale);
        public static Size operator /(Size s, int scale) => new Size(s.Width / scale, s.Height / scale);
    }
}
