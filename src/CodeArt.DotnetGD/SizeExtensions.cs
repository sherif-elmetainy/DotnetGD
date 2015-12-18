using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeArt.DotnetGD
{
    public static class SizeExtensions
    {
        public static Size Inflate(this Size size, Size by)
        {
            return new Size(size.Width + by.Width, size.Height + by.Height);
        }

        public static Size Inflate(this Size size, int width, int height)
        {
            return new Size(size.Width + width, size.Height + height);
        }

        public static Size Deflate(this Size size, Size by)
        {
            return new Size(size.Width - by.Width, size.Height - by.Height);
        }

        public static Size Deflate(this Size size, int width, int height)
        {
            return new Size(size.Width - width, size.Height - height);
        }
    }
}
