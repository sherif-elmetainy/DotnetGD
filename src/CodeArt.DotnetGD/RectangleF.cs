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
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public RectangleF(PointF location, SizeF size) : this(location.X, location.Y, size.Width, size.Height)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public RectangleF(PointF p1, PointF p2)
        {
            X = Math.Min(p1.X, p2.X);
            Y = Math.Min(p1.Y, p2.Y);
            Width = Math.Abs(p1.X - p2.X);
            Height = Math.Abs(p1.Y - p2.Y);
        }

        /// <summary>
        /// cartesian X coordinate of the left edge
        /// </summary>
        public double X { get; }

        /// <summary>
        /// cartesian Y coordinate of the top edge
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// width of the rectangle
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// height of the rectangle
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// cartesian Y coordinate of the top edge
        /// </summary>
        public double Top => Y;

        /// <summary>
        /// cartesian Y coordinate of the bottom edge
        /// </summary>
        public double Bottom => Y + Height;

        /// <summary>
        /// cartesian X coordinate of the left edge
        /// </summary>
        public double Left => X;

        /// <summary>
        /// cartesian X coordinate of the right edge
        /// </summary>
        public double Right => X + Width;

        /// <summary>
        /// Location of the top-left corner
        /// </summary>
        public PointF Location => new PointF(X, Y);

        /// <summary>
        /// Size of the rectangle
        /// </summary>
        public SizeF Size => new SizeF(Width, Height);

        /// <summary>
        /// Compares two rectangles for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RectangleF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return Width == other.Width && Height == other.Height && X == other.X && Y == other.Y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Compares rectangle for equality with another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                return Equals((RectangleF)obj);
            }
            return false;
        }

        /// <summary>
        /// Get hash code of the rectangle
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Scting representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Width}, {Height})";
        }

        /// <summary>
        /// From 2 points
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static RectangleF FromLTRB(double x1, double y1, double x2, double y2)
        {
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        /// <summary>
        /// equality comparison
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(RectangleF r1, RectangleF r2) => r1.Equals(r2);


        /// <summary>
        /// inequality comparizein
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(RectangleF r1, RectangleF r2) => !r1.Equals(r2);

        /// <summary>
        /// Whether the rectangles are similar. This differs from <see cref="Equals(CodeArt.DotnetGD.RectangleF)"/> 
        /// in that if the rectangles points are within 1e-6 from each other, they are considered similar.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool AreSimilar(RectangleF r1, RectangleF r2)
            => Math.Abs(r1.X - r2.X) < 1e-6 && Math.Abs(r1.Y - r2.Y) < 1e-6
                && Math.Abs(r1.Width - r2.Width) < 1e-6 && Math.Abs(r1.Height - r2.Height) < 1e-6;

    }
}
