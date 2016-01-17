// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Rectangle structure.
    /// Note: This structure is immutable. operations like inflate, intersect, union, etc would return a new rectangle rather than change the instance.
    /// </summary>
    public struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(int x, int y, int width, int height)
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
        public Rectangle(Point location, Size size) : this(location.X, location.Y, size.Width, size.Height)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Rectangle(Point p1, Point p2)
        {
            X = Math.Min(p1.X, p2.X);
            Y = Math.Min(p1.Y, p2.Y);
            Width = Math.Abs(p1.X - p2.X);
            Height = Math.Abs(p1.Y - p2.Y);
        }

        /// <summary>
        /// cartesian X coordinate of the left edge
        /// </summary>
        public int X { get; }

        /// <summary>
        /// cartesian Y coordinate of the top edge
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// width of the rectangle
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// height of the rectangle
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// cartesian Y coordinate of the top edge
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// cartesian Y coordinate of the bottom edge
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// cartesian X coordinate of the left edge
        /// </summary>
        public int Left => X;

        /// <summary>
        /// cartesian X coordinate of the right edge
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Location of the top-left corner
        /// </summary>
        public Point Location => new Point(X, Y);

        /// <summary>
        /// Size of the rectangle
        /// </summary>
        public Size Size => new Size(Width, Height);

        /// <summary>
        /// Compares two rectangles for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Rectangle other)
        {
            return Width == other.Width && Height == other.Height && X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Compares rectangle for equality with another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                return Equals((Rectangle)obj);
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
                hash = hash * 31 + X;
                hash = hash * 31 + Y;
                hash = hash * 31 + Width;
                hash = hash * 31 + Height;
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
        public static Rectangle FromLTRB(int x1, int y1, int x2, int y2)
        {
            return new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        /// <summary>
        /// equality comparison
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(Rectangle r1, Rectangle r2) => r1.Equals(r2);

        /// <summary>
        /// inequality comparizein
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(Rectangle r1, Rectangle r2) => !r1.Equals(r2);

    }
}
