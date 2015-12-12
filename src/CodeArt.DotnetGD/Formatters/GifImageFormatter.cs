using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    public class GifImageFormatter : BaseImageFormatter, IAnimatedImageFormatter
    {
        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImageGifCtx(imgPtr, ctx);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImageGifPtr(img, out size);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromGifCtx(ctx);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromGifPtr(size, ptr);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    "gif"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/gif";

        public override bool IsLossy => false;

        public override bool SupportsAnimation => true;

        public IAnimationContext BeginAnimation(Image image, Stream outStream, bool globalColorMap, int loops)
        {
            return new GifAnimationContext(outStream, image, globalColorMap, loops);
        }
    }
}
