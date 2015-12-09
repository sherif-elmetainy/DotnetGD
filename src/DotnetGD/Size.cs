using System;

namespace DotnetGD
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
    }
}
