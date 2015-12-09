using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotnetGD.Formatters
{
    public class BmpImageFormatter : BaseImageFormatter
    {
        
        public BmpImageFormatter() : this(true)
        {


        }

        public BmpImageFormatter(bool useCompression)
        {
            UseCompression = useCompression;
        }

        public bool UseCompression
        {
            get; set;
        }

        internal override unsafe void WriteImageToGdIoCtx(Libgd.GdImage* imgPtr, Libgd.GdIoCtx* ctx)
        {
            Libgd.NativeMethods.gdImageBmpCtx(imgPtr, ctx, UseCompression ? 1 : 0);
        }

        internal override unsafe IntPtr ImageToPtr(Libgd.GdImage* img, out int size)
        {
            return Libgd.NativeMethods.gdImageBmpPtr(img, out size, UseCompression ? 1 : 0);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromCtx(Libgd.GdIoCtx* ctx)
        {
            return Libgd.NativeMethods.gdImageCreateFromBmpCtx(ctx);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return Libgd.NativeMethods.gdImageCreateFromBmpPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "bmp", "dib"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/bmp";

        public override bool IsLossy => false;
    }
}
