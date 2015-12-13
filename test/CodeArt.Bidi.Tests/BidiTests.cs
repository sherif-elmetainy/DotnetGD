using System.Globalization;
using Xunit;
using System.Text;
using System.Linq;

namespace CodeArt.Bidi.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BidiTests
    {
        [Theory]
        [InlineData('(', BracketType.Opening)]
        [InlineData('[', BracketType.Opening)]
        [InlineData(')', BracketType.Closing)]
        [InlineData('A', BracketType.None)]
        public void TestBracketType(char ch, BracketType expectedType)
        {
            var result = UnicodeData.GetBracketType(ch);
            Assert.Equal(expectedType, result);
        }

        [Theory]
        [BidiTestData]
        [InlineData("0028 0028 05D0 005B 05D1 0029 05D2 005D 05D3;1;1;1 1 1 1 1 1 1 1 1;8 7 6 5 4 3 2 1 0", 1)]
        public void RunBidiTest(string line, int lineNumber)
        {
            var split1 = line.Split(';');
            var input = new string(split1[0].Split(' ').Select(s => (char)int.Parse(s, NumberStyles.HexNumber)).ToArray());
            var expected = new string(split1[4].Split(' ').Select(int.Parse).Select(i => input[i]).ToArray());
            var dir = sbyte.Parse(split1[1]);

            var result = GetBidi(input, dir);

            Assert.Equal(FormatString(expected), FormatString(result));
        }

        [Theory]
        [InlineData("مرحبا" + " " + "Hello" + " " + "بالعالم" + ".",
            "." + "ملاعلاب" + " " + "Hello" + " " + "ابحرم", 1)]
        public void RunBidi(string input, string expected, int dir)
        {
            var result = GetBidi(input, dir);
            Assert.Equal(FormatString(expected), FormatString(result));
        }

        [Theory]
        [InlineData("مرحبا" + " " + "Hello" + " " + "بالعالم" + ".",
            "\ufee3\ufeae\ufea3\ufe92\ufe8e" + " " + "Hello" + " " + "\ufe91\ufe8e\ufedf\ufecc\ufe8e\ufedf\ufee2" + ".")]
        [InlineData("1234", "\u0661\u0662\u0663\u0664")]
        public void RunArabicShaping(string input, string expected)
        {
            var shapper = new ArabicShaping(ArabicShapingOptions.DigitsEN2AN | ArabicShapingOptions.TextDirectionLogical | ArabicShapingOptions.LengthFixedSpacesNear | ArabicShapingOptions.LettersShape);
            var result = shapper.Shape(input);
            Assert.Equal(FormatString(expected), FormatString(result));
        }

        private static string GetBidi(string str, int dir)
        {
            var result = BidiHelper.FormatString(str, (ParagraphDirection) dir);
            return result;
        }

        private static string FormatString(string str)
        {
            var sb = new StringBuilder(str.Length);
            foreach (var c in str)
            {
                if (c <= ' ')
                {
                    sb.Append($"\\x{(int)c:x2}");
                }
                else if (c >= 0x80)
                {
                    sb.Append($"\\u{(int)c:x4}");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
