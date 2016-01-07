namespace CodeArt.DotnetGD
{
    public static class SizeExtensions
    {
        public static Size Inflate(this Size size, Size by) => size + by;
        public static Size Inflate(this Size size, int width, int height) => new Size(size.Width + width, size.Height + height);
        public static Size Deflate(this Size size, Size by) => size - by;
        public static Size Deflate(this Size size, int width, int height) => new Size(size.Width - width, size.Height - height);
        public static Size Subtract(this Size s1, Size s2) => s1 - s2;

        public static Size Add(this Size s1, Size s2) => s1 + s2;
        public static Size Multiply(this Size s, int scale) => s*scale;
        public static Size Divide(this Size s, int scale) => s / scale;
    }
}
