// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

namespace CodeArt.Bidi
{
    /// <summary>
    ///     Helper class for reading and writing unicode data to Unicode.dat included in the library
    /// </summary>
    internal static class UnicodeDataHelper
    {
        //private const int MinimumUnicodeCodePoint = 0;
        public const int MaximumUnicodeCodePoint = 0x110000;

        private const int CodePointBits = 21;

        private const int MirrorShift = 0;
        private const long MirrorMaskWrite = (1L << CodePointBits) - 1;
        private const long MirrorMaskRead = MirrorMaskWrite << MirrorShift;

        private const int MathchingBracketShift = MirrorShift + CodePointBits;
        private const long MatchingBracketMaskWrite = (1L << CodePointBits) - 1;
        private const long MatchingBracketMaskRead = MatchingBracketMaskWrite << MathchingBracketShift;

        private const int DirectionBits = 5;
        private const int DirectionShift = MathchingBracketShift + CodePointBits;
        private const long DirectionMaskWrite = (1L << DirectionBits) - 1;
        private const long DirectionMaskRead = DirectionMaskWrite << DirectionShift;

        private const int BracketTypeBits = 2;
        private const int BracketTypeShift = DirectionShift + DirectionBits;
        private const long BracketTypeMaskWrite = (1L << BracketTypeBits) - 1;
        private const long BracketTypeMaskRead = BracketTypeMaskWrite << BracketTypeShift;


        public static int GetMirror(long data) => unchecked((int)((data & MirrorMaskRead) >> MirrorShift));
        public static int GetMatchingBracket(long data) => unchecked((int)((data & MatchingBracketMaskRead) >> MathchingBracketShift));
        public static int GetDirection(long data) => unchecked((int)((data & DirectionMaskRead) >> DirectionShift));
        public static int GetBracketType(long data) => unchecked((int)((data & BracketTypeMaskRead) >> BracketTypeShift));

        public static long SetMirror(long data, int value) => ((value & MirrorMaskWrite) << MirrorShift) | data;
        public static long SetMatchingBracket(long data, int value) => ((value & MatchingBracketMaskWrite) << MathchingBracketShift) | data;
        public static long SetDirection(long data, int value) => ((value & DirectionMaskWrite) << DirectionShift) | data;
        public static long SetBracketType(long data, int value) => ((value & BracketTypeMaskWrite) << BracketTypeShift) | data;


    }
}
