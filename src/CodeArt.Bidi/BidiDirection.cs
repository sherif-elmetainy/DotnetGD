// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.Bidi
{
    /// <summary>
    /// Bidi direction
    /// </summary>
    public enum BidiDirection
    {
        /** Left-to-right */
        L = 0,

        /** Left-to-Right Embedding */
        LRE = 1,

        /** Left-to-Right Override */
        LRO = 2,

        /** Right-to-Left */
        R = 3,

        /** Right-to-Left Arabic */
        AL = 4,

        /** Right-to-Left Embedding */
        RLE = 5,

        /** Right-to-Left Override */
        RLO = 6,

        /** Pop Directional Format */
        PDF = 7,

        /** European Number */
        EN = 8,

        /** European Number Separator */
        ES = 9,

        /** European Number Terminator */
        ET = 10,

        /** Arabic Number */
        AN = 11,

        /** Common Number Separator */
        CS = 12,

        /** Non-Spacing Mark */
        NSM = 13,

        /** Boundary Neutral */
        BN = 14,

        /** Paragraph Separator */
        B = 15,

        /** Segment Separator */
        S = 16,

        /** Whitespace */
        WS = 17,

        /** Other Neutrals */
        ON = 18,

        /** Left-to-Right Isolate */
        LRI = 19,

        /** Right-to-Left Isolate */
        RLI = 20,

        /** First-Strong Isolate */
        FSI = 21,

        /** Pop Directional Isolate */
        PDI = 22,

        Unknown = -1,
        TypeMin = L,
        TypeMax = PDI
    }
}