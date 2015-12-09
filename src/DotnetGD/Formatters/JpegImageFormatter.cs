using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotnetGD.Formatters
{
    public class JpegImageFormatter : BaseImageFormatter
    {
        private int _quality;
        public const int QualityUgly = 0;
        public const int QualityBest = 100;
        public const int QualityDefault = 50;

        public JpegImageFormatter() : this(QualityDefault)
        {


        }

        public JpegImageFormatter(int quality)
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
            Libgd.NativeMethods.gdImageJpegCtx(imgPtr, ctx, Quality);
        }

        internal override unsafe IntPtr ImageToPtr(Libgd.GdImage* img, out int size)
        {
            return Libgd.NativeMethods.gdImageJpegPtr(img, out size, Quality);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromCtx(Libgd.GdIoCtx* ctx)
        {
            return Libgd.NativeMethods.gdImageCreateFromJpegCtxEx(ctx, 1);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return Libgd.NativeMethods.gdImageCreateFromJpegPtrEx(size, ptr, 1);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "jpg", "jpeg", "jpe", "jif", "jfif", ".jfi"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/jpeg";

        public override bool IsLossy => true;
    }
}
