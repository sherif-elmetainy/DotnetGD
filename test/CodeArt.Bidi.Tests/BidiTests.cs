using Xunit;

namespace CodeArt.Bidi.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BidiTests
    {
        [Theory]
        [BidiTestData]
        public void RunBidi(string input, string expected, sbyte dir, string line, int lineNumber)
        {
            var result = BidiHelper.FormatString(input, dir);
            Assert.True(expected == result, $"#{lineNumber}: {expected} != {result} === {line}");
        }
    }
}
