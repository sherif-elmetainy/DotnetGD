// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using CodeArt.DotnetGD.Libgd;

namespace CodeArt.DotnetGD
{
    public partial class Image
    {
        public unsafe ImageCompareResult CompareTo(Image other)
        {
            if (other == null)
                return ImageCompareResult.All;
            CheckObjectDisposed();
            other.CheckObjectDisposed();
            return ReferenceEquals(this, other) ? ImageCompareResult.Similar : NativeWrappers.gdImageCompare(ImagePtr, other.ImagePtr);
        }

        public static ImageCompareResult CompareImages(Image im1, Image im2)
        {
            if (im1 == null && im2 == null)
                return ImageCompareResult.Similar;
            if (im1 == null || im2 == null)
                return ImageCompareResult.All;
            return im1.CompareTo(im2);
        }
    }
}
