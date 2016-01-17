// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    public static class RectangleExtensions
    {
        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Contains(this Rectangle rect, Point p) => rect.Contains(p.X, p.Y);

        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Contains(this Rectangle rect, int x, int y) => x >= rect.X
                                                                          && x < rect.Right
                                                                          && y >= rect.Y
                                                                          && y < rect.Bottom;
        /// <summary>
        /// Whether a rectangle contains another rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Contains(this Rectangle rect, Rectangle other) => other.X >= rect.X
                                                                             && other.Right <= rect.Right
                                                                             && other.Y >= rect.Y
                                                                             && other.Bottom <= rect.Bottom;
        /// <summary>
        /// Whether a rectangle is contained in another rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsContainedIn(this Rectangle rect, Rectangle other) => other.Contains(rect);

        /// <summary>
        /// Inflate a rectangle by a given size in all directions (width is increased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rectangle Inflate(this Rectangle rect, int width, int height) => new Rectangle(rect.X - width, rect.Y - height, rect.Width + 2 * width, rect.Height + 2 * height);

        /// <summary>
        /// Inflate a rectangle by a given size in all directions (width is increased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rectangle Inflate(this Rectangle rect, Size size) => rect.Inflate(size.Width, size.Height);

        /// <summary>
        /// Deflates a rectangle by a given size in all directions (width is decreased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rectangle Deflate(this Rectangle rect, int width, int height) => rect.Inflate(-width, -height);

        /// <summary>
        /// Deflates a rectangle by a given size in all directions (width is decreased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rectangle Deflate(this Rectangle rect, Size size) => rect.Inflate(-size.Width, -size.Height);

        /// <summary>
        /// Gets the instersection of 2 rectangles
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Rectangle Intersect(this Rectangle rect, Rectangle other)
        {
            var x1 = Math.Min(rect.X, other.X);
            var x2 = Math.Min(rect.Right, other.Right);
            var y1 = Math.Min(rect.Y, other.Y);
            var y2 = Math.Min(rect.Bottom, other.Bottom);
            if (x1 <= x2 && y1 <= y2)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            return new Rectangle();
        }

        /// <summary>
        /// Whether 2 rectangles intersect with each other.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IntersectsWith(this Rectangle rect, Rectangle other) => rect.X < other.Right
                                                                                   && other.X < rect.Right
                                                                                   && rect.Y < other.Bottom
                                                                                   && other.Y < rect.Bottom;
        /// <summary>
        /// Offset a rectangle's location by given distance
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Rectangle Offset(this Rectangle rect, Point point) => rect.Offset(point.X, point.Y);

        /// <summary>
        /// Offset a rectangle's location by given distance
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Rectangle Offset(this Rectangle rect, int x, int y) => new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);

        /// <summary>
        /// Gets the union of two rectangles
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Rectangle Union(this Rectangle rect, Rectangle other)
        {
            var x1 = Math.Min(rect.X, other.X);
            var x2 = Math.Max(rect.Right, other.Right);
            var y1 = Math.Min(rect.Y, other.Y);
            var y2 = Math.Max(rect.Bottom, other.Bottom);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Contains(this RectangleF rect, PointF p) => rect.Contains(p.X, p.Y);

        /// <summary>
        /// Whether a point is contained in a rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Contains(this RectangleF rect, double x, double y) => x >= rect.X
                                                                          && x < rect.Right
                                                                          && y >= rect.Y
                                                                          && y < rect.Bottom;

        /// <summary>
        /// Whether a rectangle contains another rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Contains(this RectangleF rect, RectangleF other) => other.X >= rect.X
                                                                             && other.Right <= rect.Right
                                                                             && other.Y >= rect.Y
                                                                             && other.Bottom <= rect.Bottom;

        /// <summary>
        /// Whether a rectangle is contained in another rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsContainedIn(this RectangleF rect, RectangleF other) => other.Contains(rect);

        /// <summary>
        /// Inflate a rectangle by a given size in all directions (width is increased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static RectangleF Inflate(this RectangleF rect, double width, double height) => new RectangleF(rect.X - width, rect.Y - height, rect.Width + 2 * width, rect.Height + 2 * height);

        /// <summary>
        /// Inflate a rectangle by a given size in all directions (width is increased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static RectangleF Inflate(this RectangleF rect, SizeF size) => rect.Inflate(size.Width, size.Height);

        /// <summary>
        /// Deflates a rectangle by a given size in all directions (width is decreased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static RectangleF Deflate(this RectangleF rect, double width, double height) => rect.Inflate(-width, -height);

        /// <summary>
        /// Deflates a rectangle by a given size in all directions (width is decreased by width * 2)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static RectangleF Deflate(this RectangleF rect, SizeF size) => rect.Inflate(-size.Width, -size.Height);

        /// <summary>
        /// Gets the instersection of 2 rectangles
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static RectangleF Intersect(this RectangleF rect, RectangleF other)
        {
            var x1 = Math.Min(rect.X, other.X);
            var x2 = Math.Min(rect.Right, other.Right);
            var y1 = Math.Min(rect.Y, other.Y);
            var y2 = Math.Min(rect.Bottom, other.Bottom);
            if (x1 <= x2 && y1 <= y2)
                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            return new RectangleF();
        }

        /// <summary>
        /// Whether 2 rectangles intersect with each other.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IntersectsWith(this RectangleF rect, RectangleF other) => rect.X < other.Right
                                                                                   && other.X < rect.Right
                                                                                   && rect.Y < other.Bottom
                                                                                   && other.Y < rect.Bottom;

        /// <summary>
        /// Offset a rectangle's location by given distance
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static RectangleF Offset(this RectangleF rect, PointF point) => rect.Offset(point.X, point.Y);

        /// <summary>
        /// Offset a rectangle's location by given distance
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static RectangleF Offset(this RectangleF rect, double x, double y) => new RectangleF(rect.X + x, rect.Y + y, rect.Width, rect.Height);

        /// <summary>
        /// Gets the union of two rectangles
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static RectangleF Union(this RectangleF rect, RectangleF other)
        {
            var x1 = Math.Min(rect.X, other.X);
            var x2 = Math.Max(rect.Right, other.Right);
            var y1 = Math.Min(rect.Y, other.Y);
            var y2 = Math.Max(rect.Bottom, other.Bottom);
            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
