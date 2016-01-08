// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    public class WbmpImageFormatter : BaseImageFormatter
    {   
        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImageWBMPCtx(imgPtr, 0, ctx);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImageWBMPPtr(img, out size, 0);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromWBMPCtx(ctx);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromWBMPPtr(size, ptr);
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
