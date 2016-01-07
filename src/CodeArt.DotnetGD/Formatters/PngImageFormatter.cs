using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
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

        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImagePngCtxEx(imgPtr, ctx, CompressionLevel);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImagePngPtrEx(img, out size, CompressionLevel);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromPngCtx(ctx);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromPngPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    ".png"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/png";

        public override bool IsLossy => false;
    }
}
