// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD.Formatters
{
    public class JpegImageFormatter : BaseImageFormatter
    {
        private int _quality;
        public const int QualityUgly = 0;
        public const int QualityBest = 100;
        public const int QualityDefault = QualityBest;

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

        internal override unsafe void WriteImageToGdIoCtx(GdImage* imgPtr, GdIoCtx* ctx)
        {
            NativeWrappers.gdImageJpegCtx(imgPtr, ctx, Quality);
        }

        internal override unsafe IntPtr ImageToPtr(GdImage* img, out int size)
        {
            return NativeWrappers.gdImageJpegPtr(img, out size, Quality);
        }

        internal override unsafe GdImage* ImageCreateFromCtx(GdIoCtx* ctx)
        {
            return NativeWrappers.gdImageCreateFromJpegCtxEx(ctx, 1);
        }

        internal override unsafe GdImage* ImageCreateFromPtr(int size, IntPtr ptr)
        {
            return NativeWrappers.gdImageCreateFromJpegPtrEx(size, ptr, 1);
        }

        private static readonly IReadOnlyList<string> SupportedExtensionsList = new ReadOnlyCollection<string>(
                new []
                {
                    ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi"
                }
            );

        public override IReadOnlyList<string> SupportedExtensions => SupportedExtensionsList;

        public override string MimeType => "image/jpeg";

        public override bool IsLossy => true;
    }
}
