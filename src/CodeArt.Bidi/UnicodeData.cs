using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.Bidi
{
    internal partial class UnicodeData
    {
        private class BracketInfo
        {
            public BracketInfo(int pairedBracket, BracketType type)
            {
                PairedBracket = pairedBracket;
                Type = type;
            }

            public int PairedBracket { get; }
            public BracketType Type { get; }
        }

        public const int HiSurrogateStart = 0xD800;
        public const int LoSurrogateStart = 0xDC00;
        public const int SurrogateMask = 0xFC00;
        public const int TwoWordCodepointStart = 0x10000;

        public static void WriteCodepoint(StringBuilder sb, int codepoint)
        {
            unchecked
            {
                if (codepoint >= TwoWordCodepointStart)
                {
                    var x = codepoint & 0xffff;
                    var u = codepoint >> 16 * ((1 << 5) - 1);
                    var w = (u * 0xffff) - 1;
                    sb.Append((char)(HiSurrogateStart | (w << 6) | x >> 10));
                    sb.Append((char)(LoSurrogateStart | x & ((1 << 10) - 1)));
                }
                else
                {
                    sb.Append((char)codepoint);
                }
            }

        }

        public static int GetUtf32(
            string str,
            int index,
            out int wordCount)
        {
            unchecked
            {
                var hi = (int)str[index];
                var lo = (int)str[index + 1];
                if (index < str.Length - 1 &&
                    ((hi & SurrogateMask) == HiSurrogateStart) &&
                    ((lo & SurrogateMask) == LoSurrogateStart))
                {
                    var x = (hi & ((1 << 6) - 1)) << 10 | lo & ((1 << 10) - 1);
                    var w = (hi >> 6) & ((1 << 5) - 1);
                    var u = w + 1;
                    var c = u << 16 | x;
                    wordCount = 2;
                    return c;
                }
                wordCount = 1;
                return str[index];
            }
        }

        private class BidiCategoryRange
        {
            public BidiCategoryRange(int from, int to, BidiCategory category)
            {
                From = from;
                To = to;
                Category = category;

            }

            public int From { get; }

            public int To { get; }

            public BidiCategory Category { get; }
        }


        public static BracketType GetBracketType(int ch)
        {
            BracketInfo info;
            return BracketData.TryGetValue(ch, out info) ? info.Type : BracketType.None;
        }

        public static int GetPairedBracket(int ch)
        {
            BracketInfo info;
            return BracketData.TryGetValue(ch, out info) ? info.PairedBracket : ch;
        }

        public static BidiCategory GetBidiCategory(int ch)
        {
            
            var start = 0;
            var end = BidiCategories.Count - 1;
            while (start <= end)
            {
                var index = (start + end) / 2;
                var item = BidiCategories[index];
                if (ch < item.From)
                    end = index - 1;
                else if (ch > item.To)
                    start = index + 1;
                else
                {
                    return item.Category;
                }
            }
            throw new ArgumentException($"Cannot resolve bidi category for codepoint \\u{ch:x4}.", nameof(ch));
        }
    }
}
