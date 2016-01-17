// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// The pixel effect used for various drawing functions such as DrawLine, etc.
    /// </summary>
    public enum DrawingEffect
    {
        /// <summary>
        /// Default mode, replace the destination pixel with the source color.
        /// </summary>
        Replace = 0,
        /// <summary>
        /// Performs alpha blending
        /// </summary>
        AlphaBlend = 1,
        /// <summary>
        /// Same as <see cref="AlphaBlend"/>
        /// </summary>
        Normal = 2,
        /// <summary>
        /// use overlay blend
        /// </summary>
        Overlay = 3,
        /// <summary>
        /// use multiply blend
        /// </summary>
        Multiply = 4,

        Invalid = 5
    }
}
