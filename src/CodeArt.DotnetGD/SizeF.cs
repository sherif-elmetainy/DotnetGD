using System;

namespace CodeArt.DotnetGD
{
    public struct SizeF : IEquatable<SizeF>
    {
        public SizeF(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; }

        public double Height { get; }

        public bool Equals(SizeF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeF)
            {
                return Equals((SizeF)obj);
            }
            return false;
        }

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

        public override string ToString()
        {
            return $"({Width}, {Height})";
        }

        public static bool operator ==(SizeF s1, SizeF s2) => s1.Equals(s2);
        public static bool operator !=(SizeF s1, SizeF s2) => !s1.Equals(s2);

        public static SizeF operator +(SizeF s1, SizeF s2) => new SizeF(s1.Width + s2.Width, s1.Height + s2.Height);
        public static SizeF operator -(SizeF s1, SizeF s2) => new SizeF(s1.Width - s2.Width, s1.Height - s2.Height);
        public static SizeF operator *(double scale, SizeF s) => new SizeF(scale * s.Width, scale * s.Height);
        public static SizeF operator *(SizeF s, double scale) => new SizeF(s.Width * scale, s.Height * scale);
        public static SizeF operator /(SizeF s, double scale) => new SizeF(s.Width / scale, s.Height / scale);

        public static implicit operator SizeF(Size size) => new SizeF(size.Width, size.Height);
        public static explicit operator Size(SizeF size) => new Size((int) size.Width, (int)size.Height);
        

        public static bool AreSimilar(SizeF s1, SizeF s2)
            => Math.Abs(s1.Width - s2.Width) < 1e-6 && Math.Abs(s1.Height - s2.Height) < 1e-6;

    }
}
