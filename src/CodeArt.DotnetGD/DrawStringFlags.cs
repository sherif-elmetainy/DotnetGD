// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    ///     Draw string flags. These mostly handle RTL languages handling that's not handled properly by LIBGD
    /// </summary>
    [Flags]
    public enum DrawStringFlags
    {
        Default = 0,
        /// <summary>
        ///     Run bidi algorithm to perform logical-to-visual character reordering and remove formatting control characters
        /// </summary>
        RunBidi = 1,
        /// <summary>
        ///     A hint for the bidi algorithm about the direction of the text (Left to Right). Ignored when RunBidi is not set.
        /// </summary>
        IsLtr = 2,
        /// <summary>
        ///     A hint for the bidi algorithm about the direction of the text (Right to Left). Ignored when RunBidi is not set.
        /// </summary>
        IsRtl = 4,
        /// <summary>
        ///     Perform Arabic letter shaping
        /// </summary>
        ArabicShaping = 8,
        /// <summary>
        ///     Shape arabic numbers. Ignored when ArabicShaping is not set
        /// </summary>
        ShapeArabicNumbers = 0x10,
        /// <summary>
        ///     Remove arabic tashkeel. Ignored when ArabicShaping is not set
        /// </summary>
        RemoveArabicTashkeel = 0x20
    }
}
