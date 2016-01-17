// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

/// <summary>
/// Enumeration for Crop method for cropping images using the borders colors
/// </summary>
public enum CropMode
{
    /// <summary>
    /// Same as Sides. Crop using the colors at the 4 corners
    /// </summary>
    Default = 0,
    /// <summary>
    /// Crop using the transparent color. Transparent pixels at border are cropped.
    /// </summary>
    Transparent = 1,
    /// <summary>
    /// Crop using the black color. Black pixels at border are cropped.
    /// </summary>
    Black = 2,
    /// <summary>
    /// Crop using the white color. White pixels at border are cropped.
    /// </summary>
    White = 3,
    /// <summary>
    /// Crop using the colors at the 4 corners
    /// </summary>
    Sides = 4,
}