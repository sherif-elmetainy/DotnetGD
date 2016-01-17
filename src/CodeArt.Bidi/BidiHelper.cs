// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.Linq;
using System.Text;

namespace CodeArt.Bidi
{
    /// <summary>
    ///     Bidi helper class
    /// </summary>
    public class BidiHelper
    {
        internal static bool IsBidiControlChar(int c)
        {
            /* check for range 0x200c to 0x200f (ZWNJ, ZWJ, LRM, RLM) or
                               0x202a to 0x202e (LRE, RLE, PDF, LRO, RLO) */
            return ((c & 0xfffffffc) == 0x200c) || ((c >= 0x202a) && (c <= 0x202e))
                   || ((c >= 0x2066) && (c <= 0x2069));
        }

        private static readonly char[] LineBreakChars = {'\r', '\n'};

        /// <summary>
        /// Given an string, run the BidiAlgorithm 
        /// and rearrange the characters within the string to display accoring to Bidi algorithm rules
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="dir">paragraph direction</param>
        /// <returns>The string with characters rearranged.</returns>
        public static string FormatString(string str, ParagraphDirection dir)
        {
            var sb = new StringBuilder(str.Length);
            var index = 0;
            while (index < str.Length)
            {
                if (LineBreakChars.Contains(str[index]))
                {
                    sb.Append(str[index]);
                    index++;
                    continue;
                }
                var len = str.IndexOfAny(LineBreakChars, index);
                if (len == -1)
                    len = str.Length - index;
                else
                    len = len - index;
                var substr = str;
                if (index != 0 || len != str.Length)
                    substr = str.Substring(index, len);
                index += len;
                var info = new BidiStringInfo(substr);
                var bidi = new BidiReference(info.Types, info.BracketTypes, info.BracketValues, dir);
                var reordering = bidi.GetReordering(info.LineBreaks);

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < reordering.Length; i++)
                {
                    var orderIndex = reordering[i];
                    var code = info.Codes[orderIndex];

                    if (code > UnicodeData.TwoWordCodepointStart)
                    {
                        UnicodeData.WriteCodepoint(sb, code);
                    }
                    else
                    {
                        if (IsBidiControlChar(code))
                        {
                            continue;
                        }
                        //var cat = CharUnicodeInfo.GetUnicodeCategory((char) code);
                        //if (cat == UnicodeCategory.Format)
                        //    continue;
                        sb.Append((char) code);
                    }
                }
            }
            return sb.ToString();
        }

        
    }
}
