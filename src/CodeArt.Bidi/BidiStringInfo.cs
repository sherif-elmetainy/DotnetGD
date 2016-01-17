// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.Collections.Generic;

namespace CodeArt.Bidi
{
    /// <summary>
    /// Parses a string and gets data about character unicode bidi categories and bracket types 
    /// To pass to the BidiAlgorithm
    /// </summary>
    internal class BidiStringInfo
    {
        public BidiStringInfo(string str)
        {
            OriginalString = str;
            var codes = new List<int>(str.Length);
            var lbs = new List<int>();
            var types=  new List<BidiDirection>(str.Length);
            var bracketsTypes = new List<BracketType>(str.Length);
            var bracketValues = new List<int>(str.Length);
            int numWords;
            for (var i = 0; i < str.Length; i += numWords)
            {
                var code = (int)str[i];
                numWords = 1;
                if ((code & UnicodeData.SurrogateMask) == UnicodeData.HiSurrogateStart)
                {
                    code = UnicodeData.GetUtf32(str, i, out numWords);
                }
                codes.Add(code);
                types.Add(UnicodeData.GetDirection(code));
                var bt = UnicodeData.GetBracketType(code);
                bracketsTypes.Add(bt);
                bracketValues.Add(bt == BracketType.Opening ? UnicodeData.GetMatchingBracket(code) : code);
                if (code == '\r' || code == '\n')
                    lbs.Add(i);
            }
            lbs.Add(codes.Count);

            Codes = codes.ToArray();
            Types = types.ToArray();
            BracketTypes = bracketsTypes.ToArray();
            BracketValues = bracketValues.ToArray();
            LineBreaks = lbs.ToArray();
        }

        /// <summary>
        /// Bidi direction for all the characters in the string
        /// </summary>
        public BidiDirection[] Types { get; }

        /// <summary>
        /// The original string
        /// </summary>
        public string OriginalString { get; }

        /// <summary>
        /// 32 bit unicode code points of all characters in the string
        /// </summary>
        public int[] Codes { get;  }

        /// <summary>
        /// Bracket type (closing, opening or none)
        /// </summary>
        public BracketType[] BracketTypes { get; }

        /// <summary>
        /// 32 bit unicode Code points for bracket values
        /// </summary>
        public int[] BracketValues { get; }

        /// <summary>
        /// indexes of line breaks in the string
        /// </summary>
        public int[] LineBreaks { get; }
    }
}
