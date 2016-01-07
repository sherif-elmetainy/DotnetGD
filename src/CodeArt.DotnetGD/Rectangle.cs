using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Rectangle structure.
    /// Note: This structure is immutable. operations like inflate, intersect, union, etc would return a new rectangle rather than change the instance.
    /// </summary>
    public struct Rectangle : IEquatable<Rectangle>
    {
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

        public Rectangle(Point location, Size size) : this(location.X, location.Y, size.Width, size.Height)
        {
            
        }

        public Rectangle(Point p1, Point p2)
        {
            X = Math.Min(p1.X, p2.X);
            Y = Math.Min(p1.Y, p2.Y);
            Width = Math.Abs(p1.X - p2.X);
            Height = Math.Abs(p1.Y - p2.Y);
        }

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public int Top => Y;

        public int Bottom => Y + Height;

        public int Left => X;

        public int Right => X + Width;

        public Point Location => new Point(X, Y);

        public Size Size => new Size(Width, Height);

        public bool Equals(Rectangle other)
        {
            return Width == other.Width && Height == other.Height && X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                return Equals((Rectangle)obj);
            }
            return false;
        }

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

        public override string ToString()
        {
            return $"({X}, {Y}, {Width}, {Height})";
        }

        public static Rectangle FromLTRB(int x1, int y1, int x2, int y2)
        {
            return new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static bool operator ==(Rectangle r1, Rectangle r2) => r1.Equals(r2);


        public static bool operator !=(Rectangle r1, Rectangle r2) => !r1.Equals(r2);

    }
}
