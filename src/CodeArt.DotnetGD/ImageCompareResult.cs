// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Comparison result when comparing two images
    /// </summary>
    [Flags]
    public enum ImageCompareResult
    {
        /// <summary>
        /// Images are similar
        /// </summary>
        Similar = 0,
        /// <summary>
        /// Image itself is different
        /// </summary>
        Image = 1,
        /// <summary>
        /// Number of colors is different (in case of 8-bit images)
        /// </summary>
        NumberOfColors = 2,
        /// <summary>
        /// Colors of pixels are different
        /// </summary>
        Color = 4,
        /// <summary>
        /// Widths are different
        /// </summary>
        Width = 8,
        /// <summary>
        /// Heights are different
        /// </summary>
        Height = 16,
        /// <summary>
        /// Transparent color is different
        /// </summary>
        TransparentColor = 32,
        /// <summary>
        /// background color is different
        /// </summary>
        Background = 64,
        /// <summary>
        /// interlacing is different
        /// </summary>
        Interlace = 128,
        /// <summary>
        /// Pixel format is different
        /// </summary>
        TrueColor = 256,
        /// <summary>
        /// Possible different that can occur from formatting loss
        /// </summary>
        FormattingLoss = Image | NumberOfColors | Color | TransparentColor | Background | TrueColor | Interlace,
        /// <summary>
        /// different size
        /// </summary>
        DifferentSize = Width | Height,
        /// <summary>
        /// Eveything is different
        /// </summary>
        All = Image | NumberOfColors | Color | Width | Height | TransparentColor | Background | Interlace | TrueColor
    }
}