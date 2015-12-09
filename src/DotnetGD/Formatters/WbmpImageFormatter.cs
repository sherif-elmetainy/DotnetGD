using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotnetGD.Formatters
{
    public class WbmpImageFormatter : BaseImageFormatter
    {   
        internal override unsafe void WriteImageToGdIoCtx(Libgd.GdImage* imgPtr, Libgd.GdIoCtx* ctx)
        {
            Libgd.NativeMethods.gdImageWBMPCtx(imgPtr, 0, ctx);
        }

        internal override unsafe IntPtr ImageToPtr(Libgd.GdImage* img, out int size)
        {
            return Libgd.NativeMethods.gdImageWBMPPtr(img, out size, 0);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromCtx(Libgd.GdIoCtx* ctx)
        {
            return Libgd.NativeMethods.gdImageCreateFromWBMPCtx(ctx);
        }

        internal override unsafe Libgd.GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return Libgd.NativeMethods.gdImageCreateFromWBMPPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "wbmp"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/vnd.wap.wbmp";

        public override bool IsLossy => false;
    }
}
