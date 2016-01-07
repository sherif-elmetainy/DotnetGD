using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Immutable Point structure representing x and y coordinates of a 2D point.
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        /// <summary>
        /// Compares 2 points
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Compares point to another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return Equals((Point) obj);
            }
            return false;
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + X;
                hash = hash * 31 + Y;
                return hash;
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(Point p1, Point p2) => p1.Equals(p2);
        public static bool operator !=(Point p1, Point p2) => !p1.Equals(p2);
        public static Point operator + (Point p, Size s) => new Point(p.X + s.Width, p.Y + s.Height);
        public static Point operator -(Point p, Size s) => new Point(p.X - s.Width, p.Y - s.Height);
    }
}
