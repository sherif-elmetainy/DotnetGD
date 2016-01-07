using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    public class TiffImageFormatter : BaseImageFormatter
    {
        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImageTiffCtx(imgPtr, ctx);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImageTiffPtr(img, out size);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromTiffCtx(ctx);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromTiffPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    ".tiff", ".tif"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/tiff";

        public override bool IsLossy => false;
    }
}
