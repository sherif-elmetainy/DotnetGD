// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Rectangle structure.
    /// Note: This structure is immutable. operations like inflate, intersect, union, etc would return a new rectangle rather than change the instance.
    /// </summary>
    public struct RectangleF : IEquatable<RectangleF>
    {
        public RectangleF(double x, double y, double width, double height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, $"{nameof(width)} cannot be negative.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, $"{nameof(height)} cannot be negative.");

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(PointF location, SizeF size) : this(location.X, location.Y, size.Width, size.Height)
        {
            
        }

        public RectangleF(PointF p1, PointF p2)
        {
            X = Math.Min(p1.X, p2.X);
            Y = Math.Min(p1.Y, p2.Y);
            Width = Math.Abs(p1.X - p2.X);
            Height = Math.Abs(p1.Y - p2.Y);
        }

        public double X { get; }

        public double Y { get; }

        public double Width { get; }

        public double Height { get; }

        public double Top => Y;

        public double Bottom => Y + Height;

        public double Left => X;

        public double Right => X + Width;

        public PointF Location => new PointF(X, Y);

        public SizeF Size => new SizeF(Width, Height);

        public bool Equals(RectangleF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height && X == other.X && Y == other.Y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                return Equals((RectangleF)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                hash = hash * 31 + Width.GetHashCode();
                hash = hash * 31 + Height.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Width}, {Height})";
        }

        public static RectangleF FromLTRB(double x1, double y1, double x2, double y2)
        {
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static bool operator ==(RectangleF r1, RectangleF r2) => r1.Equals(r2);


        public static bool operator !=(RectangleF r1, RectangleF r2) => !r1.Equals(r2);

        public static bool AreSimilar(RectangleF r1, RectangleF r2)
            => Math.Abs(r1.X - r2.X) < 1e-6 && Math.Abs(r1.Y - r2.Y) < 1e-6
                && Math.Abs(r1.Width - r2.Width) < 1e-6 && Math.Abs(r1.Height - r2.Height) < 1e-6;

    }
}
