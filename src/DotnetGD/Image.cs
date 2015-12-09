using System;

namespace DotnetGD
{
    public sealed unsafe class Image : IDisposable
    {
        internal Libgd.GdImage* GdImage;

        public const int PaletteQuantizationSpeedBestQuality = 1;
        public const int PaletteQuantizationSpeedBestSpeed = 10;
        public const int PaletteQuantizationUgly = 1;
        public const int PaletteQuantizationPerfect = 100;

        public Image(int width, int height, PixelFormat pixelFormat = PixelFormat.Format32BppArgb)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, $"{nameof(width)} must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, $"{nameof(height)} must be greater than zero.");

            var trueColor = pixelFormat == PixelFormat.Format32BppArgb;
            GdImage = trueColor ? Libgd.NativeMethods.gdImageCreateTrueColor(width, height) : Libgd.NativeMethods.gdImageCreate(width, height);
            if (GdImage == null)
                throw new LibgdException(LibgdException.GetErrorMessage(trueColor ? nameof(Libgd.NativeMethods.gdImageCreateTrueColor) : nameof(Libgd.NativeMethods.gdImageCreate), "A null image pointer was returned."));
        }

        internal Image(Libgd.GdImage* imagePtr)
        {
            if (imagePtr == null) throw new ArgumentNullException(nameof(imagePtr));
            GdImage = imagePtr;
        }

        ~Image()
        {
            Dispose(false);
        }

        public ImageCompareResult CompareTo(Image other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            return Libgd.NativeMethods.gdImageCompare(GdImage, other.GdImage);
        }

        private void CheckObjectDisposed()
        {
            if (GdImage == null)
                throw new ObjectDisposedException(nameof(Libgd.GdImage));
        }

        public int Width
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).Width;
            }
        }

        public int Height
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).Height;
            }
        }

        public PixelFormat PixelFormat
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).TrueColor == 1 ? PixelFormat.Format32BppArgb : PixelFormat.Format8BppIndexed;
            }
        }

        public uint ResolutionX
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).ResolutionX;
            }
            set
            {
                CheckObjectDisposed();
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ResolutionY)} cannot be zero.");
                (*GdImage).ResolutionX = value;
            }
        }

        public uint ResolutionY
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).ResolutionY;
            }
            set
            {
                CheckObjectDisposed();
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ResolutionY)} cannot be zero.");
                (*GdImage).ResolutionY = value;
            }
        }

        public InterpolationMethod InterpolationMethod
        {
            get
            {
                CheckObjectDisposed();
                return (*GdImage).InterpolationId;
            }
            set
            {
                CheckObjectDisposed();
                if (value < InterpolationMethod.Default || value >= InterpolationMethod.Invalid)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Invalid value for {nameof(InterpolationMethod)}.");
                (*GdImage).InterpolationId = value;
            }
        }

        public Rectangle ClipRectangle
        {
            get
            {
                CheckObjectDisposed();
                var p1 = new Point((*GdImage).ClipX1, (*GdImage).ClipY1);
                var p2 = new Point((*GdImage).ClipX2, (*GdImage).ClipY2);
                return new Rectangle(p1, p2);
            }
            set
            {
                CheckObjectDisposed();
                if (value.Left < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Left)} must be greater than or equal zero.");
                if (value.Top < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Top)} must be greater than or equal zero.");
                if (value.Left >= Width)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Left)} must be less than {nameof(Width)} which is {Width}.");
                if (value.Top >= Height)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Top)} must be less than {nameof(Height)} which is {Height}.");

                if (value.Right < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Right)} must be greater than or equal zero.");
                if (value.Bottom < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Bottom)} must be greater than or equal zero.");
                if (value.Right >= Width)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Right)} must be less than {nameof(Width)} which is {Width}.");
                if (value.Bottom >= Height)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ClipRectangle)}.{nameof(ClipRectangle.Bottom)} must be less than {nameof(Height)} which is {Height}.");

                Libgd.NativeMethods.gdImageSetClip(GdImage, value.Left, value.Top, value.Right, value.Bottom);
            }
        }

        public void DrawRecangle(Rectangle rectangle, Color color)
        {
            var resolvedColor = ResolveColor(color);
            Libgd.NativeMethods.gdImageRectangle(GdImage, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, resolvedColor);
        }

        public void DrawFilledRecangle(Rectangle rectangle, Color color)
        {
            var resolvedColor = ResolveColor(color);
            Libgd.NativeMethods.gdImageFilledRectangle(GdImage, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, resolvedColor);
        }

        public void DrawEllipse(Rectangle rectangle, Color color)
        {
            var resolvedColor = ResolveColor(color);
            Libgd.NativeMethods.gdImageEllipse(GdImage, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, resolvedColor);
        }

        public void DrawFilledEllipse(Rectangle rectangle, Color color)
        {
            var resolvedColor = ResolveColor(color);
            Libgd.NativeMethods.gdImageFilledEllipse(GdImage, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, resolvedColor);
        }

        private int ResolveColor(Color color)
        {
            var res = Libgd.NativeMethods.gdImageColorResolveAlpha(GdImage, color.R, color.G, color.B, (255 - color.A) / 2);
            if (res < 0)
                throw new LibgdException(LibgdException.GetErrorMessage(nameof(Libgd.NativeMethods.gdImageColorResolveAlpha), $"Resolve color '{color}' returned {res}."));
            return res;
        }

        private void Dispose(bool isDisposing)
        {
            var image = GdImage;
            if (image == null) return;
            Libgd.NativeMethods.gdImageDestroy(image);
            GdImage = null;
            if (isDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
