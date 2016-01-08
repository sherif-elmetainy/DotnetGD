namespace CodeArt.DotnetGD
{
    public static class PointExtensions
    {
        public static Point Add(this Point p, Size size) => p + size;
        public static Point Subtract(this Point p, Size size) => p - size;
        public static Point Offset(this Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
        public static Point Offset(this Point p, int x, int y) => new Point(p.X + x, p.Y + y);
        public static bool IsContainedIn(this Point p, Rectangle rect) => rect.Contains(p);

        public static PointF Add(this PointF p, Size size) => p + size;
        public static PointF Subtract(this PointF p, Size size) => p - size;
        public static PointF Offset(this PointF p1, PointF p2) => new PointF(p1.X + p2.X, p1.Y + p2.Y);
        public static PointF Offset(this PointF p, int x, int y) => new PointF(p.X + x, p.Y + y);
        public static bool IsContainedIn(this PointF p, RectangleF rect) => rect.Contains(p);
    }
}
