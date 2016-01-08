// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.Collections.Generic;

namespace CodeArt.Bidi
{
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


        public BidiDirection[] Types { get; }

        public string OriginalString { get; }

        public int[] Codes { get;  }

        public BracketType[] BracketTypes { get; }

        public int[] BracketValues { get; }

        public int[] LineBreaks { get; }
    }
}
