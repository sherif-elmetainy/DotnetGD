using System;

namespace CodeArt.DotnetGD
{
    public static class RectangleExtensions
    {
        public static bool Contains(this Rectangle rect, Point p) => rect.Contains(p.X, p.Y);

        public static bool Contains(this Rectangle rect, int x, int y) => x >= rect.X
                                                                          && x < rect.Right
                                                                          && y >= rect.Y
                                                                          && y < rect.Bottom;

        public static bool Contains(this Rectangle rect, Rectangle other) => other.X >= rect.X
                                                                             && other.Right <= rect.Right
                                                                             && other.Y >= rect.Y
                                                                             && other.Bottom <= rect.Bottom;

        public static Rectangle Inflate(this Rectangle rect, int width, int height) => new Rectangle(rect.X - width, rect.Y - height, rect.Width + 2 * width, rect.Height + 2 * height);

        public static Rectangle Inflate(this Rectangle rect, Size size) => rect.Inflate(size.Width, size.Height);

        public static Rectangle Deflate(this Rectangle rect, int width, int height) => rect.Inflate(-width, -height);

        public static Rectangle Deflate(this Rectangle rect, Size size) => rect.Inflate(-size.Width, -size.Height);

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

        public static bool IntersectsWith(this Rectangle rect, Rectangle other) => rect.X < other.Right
                                                                                   && other.X < rect.Right
                                                                                   && rect.Y < other.Bottom
                                                                                   && other.Y < rect.Bottom;

        public static Rectangle Offset(this Rectangle rect, Point point) => rect.Offset(point.X, point.Y);

        public static Rectangle Offset(this Rectangle rect, int x, int y) => new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);

        public static Rectangle Union(this Rectangle rect, Rectangle other)
        {
            var x1 = Math.Min(rect.X, other.X);
            var x2 = Math.Max(rect.Right, other.Right);
            var y1 = Math.Min(rect.Y, other.Y);
            var y2 = Math.Max(rect.Bottom, other.Bottom);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        
    }
}
