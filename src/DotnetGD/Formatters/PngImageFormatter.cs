using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotnetGD.Formatters
{
    public class PngImageFormatter : BaseImageFormatter
    {
        private int _compressionLevel;
        public const int CompressionLevelNone = 0;
        public const int CompressionLevelSmallest = 9;
        public const int CompressionLevelDefault = CompressionLevelSmallest;

        public PngImageFormatter() : this(CompressionLevelDefault)
        {


        }

        public PngImageFormatter(int compressionLevel)
        {
            CompressionLevel = compressionLevel;
        }

        public int CompressionLevel
        {
            get { return _compressionLevel; }
            set
            {
                if (value < CompressionLevelNone || value > CompressionLevelSmallest)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(CompressionLevel)} must be between {CompressionLevelNone} and {CompressionLevelSmallest}.");
                _compressionLevel = value;
            }
        }

        internal override unsafe void WriteImageToGdIoCtx(Libgd.GdImage* imgPtr, Libgd.GdIoCtx* ctx)
        {
            Libgd.NativeMethods.gdImagePngCtxEx(imgPtr, ctx, CompressionLevel);
        }

        internal override unsafe IntPtr ImageToPtr(Libgd.GdImage* img, out int size)
        {
            return Libgd.NativeMethods.gdImagePngPtrEx(img, out size, CompressionLevel);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromCtx(Libgd.GdIoCtx* ctx)
        {
            return Libgd.NativeMethods.gdImageCreateFromPngCtx(ctx);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return Libgd.NativeMethods.gdImageCreateFromPngPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "png"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/png";

        public override bool IsLossy => false;
    }
}
