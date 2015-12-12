using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace CodeArt.Bidi
{
    public class BidiHelper
    {
        public static string FormatString(string str, sbyte dir)
        {
            var info = new BidiStringInfo(str);
            var bidi = new BidiReference(info.Types, info.BracketTypes, info.BracketValues, dir);
            var reordering = bidi.GetReordering(info.LineBreaks);
            //var levels = bidi.GetLevels(info.LineBreaks);
            //var resultTypes = bidi.GetResultTypes();
            var sb = new StringBuilder(str.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < reordering.Length; i++)
            {
                var index = reordering[i];
                var code = info.Codes[index];
                //var type = resultTypes[index];
                //if (type == BidiCategory.LRE
                //    || type == BidiCategory.LRO
                //    || type == BidiCategory.RLE
                //    || type == BidiCategory.RLO
                //    || type == BidiCategory.PDF
                //    || type == BidiCategory.PDI
                //    || type == BidiCategory.LRI
                //    || type == BidiCategory.RLI
                //    || type == BidiCategory.FSI
                //    || type == BidiCategory.B
                //    || type == BidiCategory.S
                //    || type == BidiCategory.BN
                //    )
                //    continue;
                if (code > UnicodeData.TwoWordCodepointStart)
                {
                    UnicodeData.WriteCodepoint(sb, code);
                }
                else
                {
                    var cat = CharUnicodeInfo.GetUnicodeCategory((char)code);
                    if (cat == UnicodeCategory.Format)
                        continue;
                    sb.Append((char) code);
                }
            }
            return sb.ToString();
        }

        
    }
}
