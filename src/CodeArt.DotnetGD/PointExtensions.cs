// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Helper methods for <see cref="Point"/> and <see cref="PointF"/>
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Adds point and size
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Point Add(this Point p, Size size) => p + size;
        /// <summary>
        /// Subtract size from a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Point Subtract(this Point p, Size size) => p - size;
        /// <summary>
        /// Offset a point by another point
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point Offset(this Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
        /// <summary>
        /// Offset a point by x and y
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Point Offset(this Point p, int x, int y) => new Point(p.X + x, p.Y + y);
        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsContainedIn(this Point p, Rectangle rect) => rect.Contains(p);

        /// <summary>
        /// Adds point and size
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static PointF Add(this PointF p, Size size) => p + size;
        /// <summary>
        /// Subtract size from a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static PointF Subtract(this PointF p, Size size) => p - size;
        /// <summary>
        /// Offset a point by another point
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static PointF Offset(this PointF p1, PointF p2) => new PointF(p1.X + p2.X, p1.Y + p2.Y);
        /// <summary>
        /// Offset a point by x and y
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PointF Offset(this PointF p, int x, int y) => new PointF(p.X + x, p.Y + y);
        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsContainedIn(this PointF p, RectangleF rect) => rect.Contains(p);
    }
}
