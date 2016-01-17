// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.Bidi
{
    /// <summary>
    /// Bidi direction
    /// </summary>
    public enum BidiDirection
    {
        /// <summary> Left-to-right </summary>
        L = 0,

        /// <summary> Left-to-Right Embedding </summary>
        LRE = 1,

        /// <summary> Left-to-Right Override </summary>
        LRO = 2,

        /// <summary> Right-to-Left </summary>
        R = 3,

        /// <summary> Right-to-Left Arabic </summary>
        AL = 4,

        /// <summary> Right-to-Left Embedding </summary>
        RLE = 5,

        /// <summary> Right-to-Left Override </summary>
        RLO = 6,

        /// <summary> Pop Directional Format </summary>
        PDF = 7,

        /// <summary> European Number </summary>
        EN = 8,

        /// <summary> European Number Separator </summary>
        ES = 9,

        /// <summary> European Number Terminator </summary>
        ET = 10,

        /// <summary> Arabic Number </summary>
        AN = 11,

        /// <summary> Common Number Separator </summary>
        CS = 12,

        /// <summary> Non-Spacing Mark </summary>
        NSM = 13,

        /// <summary> Boundary Neutral </summary>
        BN = 14,

        /// <summary> Paragraph Separator </summary>
        B = 15,

        /// <summary> Segment Separator </summary>
        S = 16,

        /// <summary> Whitespace </summary>
        WS = 17,

        /// <summary> Other Neutrals </summary>
        ON = 18,

        /// <summary> Left-to-Right Isolate </summary>
        LRI = 19,

        /// <summary> Right-to-Left Isolate </summary>
        RLI = 20,

        /// <summary> First-Strong Isolate </summary>
        FSI = 21,

        /// <summary> Pop Directional Isolate </summary>
        PDI = 22,

        Unknown = -1,
        TypeMin = L,
        TypeMax = PDI
    }
}