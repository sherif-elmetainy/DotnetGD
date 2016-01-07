using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
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

        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImageBmpCtx(imgPtr, ctx, UseCompression ? 1 : 0);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImageBmpPtr(img, out size, UseCompression ? 1 : 0);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromBmpCtx(ctx);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromBmpPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    ".bmp", ".dib"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/bmp";

        public override bool IsLossy => false;
    }
}
