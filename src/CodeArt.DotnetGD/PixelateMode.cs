// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Mode to use in <see cref="Image.Pixelate"/>
    /// </summary>
    public enum PixelateMode
    {
        /// <summary>
        /// color the block with the color of the upper left pixel of the block
        /// </summary>
        UpplerLeft,
        /// <summary>
        /// Color the block with the average color of all pixels in the block
        /// </summary>
        Average,
    }
}
