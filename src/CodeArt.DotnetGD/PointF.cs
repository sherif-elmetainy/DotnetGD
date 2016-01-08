// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Immutable Point structure representing x and y coordinates of a 2D point.
    /// </summary>
    public struct PointF : IEquatable<PointF>
    {
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        /// <summary>
        /// Compares 2 points
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PointF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return X == other.X && Y == other.Y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
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
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
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

        public static bool operator ==(PointF p1, PointF p2) => p1.Equals(p2);
        public static bool operator !=(PointF p1, PointF p2) => !p1.Equals(p2);
        public static PointF operator + (PointF p, Size s) => new PointF(p.X + s.Width, p.Y + s.Height);
        public static PointF operator -(PointF p, Size s) => new PointF(p.X - s.Width, p.Y - s.Height);

        public static implicit operator PointF(Point p) => new PointF(p.X, p.Y);
        public static explicit operator Point(PointF p) => new Point((int)p.X, (int)p.Y);


        public static bool AreSimilar(PointF p1, PointF p2)
            => Math.Abs(p1.X - p2.X) < 1e-6 && Math.Abs(p1.Y - p2.Y) < 1e-6;

    }
}
