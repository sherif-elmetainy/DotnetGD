// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Interpolation method used for resizing images
    /// </summary>
    public enum InterpolationMethod
    {
        Default = 0,
        Bell,
        Bessel,
        BilinearFixed,
        Bicubic,
        BicubicFixed,
        Blackman,
        Box,
        Bspline,
        Catmullrom,
        Gaussian,
        GeneralizedCubic,
        Hermite,
        Hamming,
        Hanning,
        Mitchell,
        NearestNeighbour,
        Power,
        Quadratic,
        Sinc,
        Triangle,
        Weighted4,
        Invalid
    }
}
