// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Threading;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Image class (a managed wrapper around libgd Image).
    /// WARNING: This class is NOT Thread safe and as an unsafe wrapper that used unamanged pointers, 
    /// Unexpected results (including host process crashing) can occur if race conditions occur.
    /// I have considered using locks every where the image pointer is access, 
    /// however the most typical use would not have the Image used in multiple threads. 
    /// So I would rather have this warning here and leave it up to the consumer to use locks if needed.
    /// 
    /// It is theoritically safe to have multiple threads read the Image instance 
    /// (For example having the image in a static field and using it as a brush or a tile for other images).
    /// 
    /// So if you are going to use multiple threads to write to the image (very uncommon scenario), then locks must be used.
    /// A race condition can also occur when Disposing the Image in one thread and trying to read it in another thread.
    /// 
    /// </summary>
    public sealed unsafe partial class Image : IDisposable
    {
        static Image()
        {
            NativeWrappers.InitializeLibGd();
        }

        internal GdImage* ImagePtr;
        // when an image is referenced as a brush or a tile for another image  
        // a reference is incremented to prevent the image from being disposed
        private int _references = 1;

        public const int PaletteQuantizationSpeedBestQuality = 1;
        public const int PaletteQuantizationSpeedBestSpeed = 10;
        //public const int PaletteQuantizationUgly = 1;
        //public const int PaletteQuantizationPerfect = 100;

        /// <summary>
        /// Creates a new instance of <see cref="Image" />
        /// </summary>
        /// <param name="size">size of the image</param>
        /// <param name="pixelFormat">format of the image (true color or 256 color indexed)</param>
        public Image(Size size, PixelFormat pixelFormat = PixelFormat.Format32BppArgb) : this(size.Width, size.Height, pixelFormat)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="Image" />
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        /// <param name="pixelFormat">format of the image (true color or 256 color indexed)</param>
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

        /// <summary>
        /// internal constructor to create image given a libgd image pointer
        /// </summary>
        /// <param name="imagePtrPtr"></param>
        internal Image(GdImage* imagePtrPtr)
        {
            if (imagePtrPtr == null) throw new ArgumentNullException(nameof(imagePtrPtr));
            ImagePtr = imagePtrPtr;
        }

        /// <summary>
        /// Finalized to clean native resources (in case dispose was not called)
        /// </summary>
        ~Image()
        {
            Dispose(false);
        }

        internal void AddReference()
        {
            CheckObjectDisposed();
            Interlocked.Increment(ref _references);
        }

        /// <summary>
        /// Returns a pointer to raw pixel data.
        /// </summary>
        /// <returns></returns>
        public void* GetPixels()
        {
            CheckObjectDisposed();
            return ImagePtr->TrueColor == 1 ? ImagePtr->TruecolorPixels : (void*)ImagePtr->Pixels;
        }


        private void CheckObjectDisposed()
        {
            if (ImagePtr == null)
                throw new ObjectDisposedException(nameof(GdImage));
        }

        /// <summary>
        /// Gets image width
        /// </summary>
        public int Width
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->Width;
            }
        }

        /// <summary>
        /// Gets image height
        /// </summary>
        public int Height
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->Height;
            }
        }

        /// <summary>
        /// Gets image size
        /// </summary>
        public Size Size => new Size(Width, Height);

        /// <summary>
        /// Gets image format
        /// </summary>
        public PixelFormat PixelFormat
        {
            get
            {
                CheckObjectDisposed();
                return ImagePtr->TrueColor == 1 ? PixelFormat.Format32BppArgb : PixelFormat.Format8BppIndexed;
            }
        }

        /// <summary>
        /// Gets or sets image horizontal resolution (dpi)
        /// </summary>
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

        /// <summary>
        /// gets or sets image vertical resolution (dpi)
        /// </summary>
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

        /// <summary>
        /// Gets image bounds
        /// </summary>
        public Rectangle Bounds => new Rectangle(new Point(), Size);

        /// <summary>
        /// Gets or sets image's clip rectangle
        /// </summary>
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

        /// <summary>
        /// Dispose of the image cleaning all native resources
        /// </summary>
        /// <param name="isDisposing"></param>
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

        /// <summary>
        /// dispose of the image cleaning all native resources
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Decrement(ref _references) <= 0)
            {
                Dispose(true);
            }
        }
    }
}
