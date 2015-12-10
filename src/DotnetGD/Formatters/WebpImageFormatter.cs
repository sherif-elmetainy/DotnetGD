#if WEBP
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotnetGD.Formatters
{
    public class WebpImageFormatter : BaseImageFormatter
    {
        private int _quality;
        public const int QualityUgly = 0;
        public const int QualityBest = 100;
        public const int QualityDefault = 50;

        public WebpImageFormatter() : this(QualityDefault)
        {


        }

        public WebpImageFormatter(int quality)
        {
            Quality = quality;
        }

        public int Quality
        {
            get { return _quality; }
            set
            {
                if (value < QualityUgly || value > QualityBest)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(Quality)} must be between {QualityUgly} and {QualityBest}.");
                _quality = value;
            }
        }

        internal override unsafe void WriteImageToGdIoCtx(Libgd.GdImage* imgPtr, Libgd.GdIoCtx* ctx)
        {
            Libgd.NativeMethods.gdImageWebpCtx(imgPtr, ctx, Quality);
        }

        internal override unsafe IntPtr ImageToPtr(Libgd.GdImage* img, out int size)
        {
            return Libgd.NativeMethods.gdImageWebpPtrEx(img, out size, Quality);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromCtx(Libgd.GdIoCtx* ctx)
        {
            return Libgd.NativeMethods.gdImageCreateFromWebpCtx(ctx);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return Libgd.NativeMethods.gdImageCreateFromWebpPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "webp"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/webp";

        public override bool IsLossy => true;
    }
}

#endif