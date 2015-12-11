using System;
using System.Threading;
using DotnetGD.Libgd;

namespace DotnetGD
{
    public sealed unsafe partial class Image : IDisposable
    {
        static Image()
        {
            NativeWrappers.InitializeLibGd();
        }

        internal GdImage* ImagePtr;
        private int _references = 1;

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
            ImagePtr = trueColor ? NativeWrappers.gdImageCreateTrueColor(width, height) : NativeWrappers.gdImageCreate(width, height);
            if (!trueColor)
            {
                ResolveColor(new Color(0, 0, 0));
            }
        }

        internal Image(GdImage* imagePtrPtr)
        {
            if (imagePtrPtr == null) throw new ArgumentNullException(nameof(imagePtrPtr));
            ImagePtr = imagePtrPtr;
        }

        ~Image()
        {
            Dispose(false);
        }

        internal void AddReference()
        {
            Interlocked.Increment(ref _references);
        }

        

        private void CheckObjectDisposed()
        {
            if (ImagePtr == null)
                throw new ObjectDisposedException(nameof(GdImage));
        }

        public int Width
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->Width;
            }
        }

        public int Height
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->Height;
            }
        }

        public PixelFormat PixelFormat
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->TrueColor == 1 ? PixelFormat.Format32BppArgb : PixelFormat.Format8BppIndexed;
            }
        }

        public uint ResolutionX
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->ResolutionX;
            }
            set
            {
                CheckObjectDisposed();
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ResolutionY)} cannot be zero.");
                ImagePtr->ResolutionX = value;
            }
        }

        public uint ResolutionY
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->ResolutionY;
            }
            set
            {
                CheckObjectDisposed();
                if (value == 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(ResolutionY)} cannot be zero.");
                ImagePtr->ResolutionY = value;
            }
        }

        public InterpolationMethod InterpolationMethod
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->InterpolationId;
            }
            set
            {
                CheckObjectDisposed();
                if (value < InterpolationMethod.Default || value >= InterpolationMethod.Invalid)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Invalid value for {nameof(InterpolationMethod)}.");
                ImagePtr->InterpolationId = value;
            }
        }

        public Rectangle ClipRectangle
        {
            get
            {
                CheckObjectDisposed();
                var p1 = new Point(ImagePtr->ClipX1, ImagePtr->ClipY1);
                var p2 = new Point(ImagePtr->ClipX2, ImagePtr->ClipY2);
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

                NativeWrappers.gdImageSetClip(ImagePtr, value.Left, value.Top, value.Right, value.Bottom);
            }
        }
        
        private void Dispose(bool isDisposing)
        {
            var image = ImagePtr;
            if (image == null) return;
            NativeWrappers.gdImageDestroy(image);
            ImagePtr = null;
            _pen = null;
            _brush?.Dispose();
            _tile?.Dispose();
            if (isDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Decrement(ref _references) <= 0)
            {
                Dispose(true);
            }
        }
    }
}
