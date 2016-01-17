// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArt.Bidi.Tests
{
    public class BidiTestDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            using (var fs = File.OpenRead("BidiCharacterTest.txt"))
            {
                using (var reader = new StreamReader(fs))
                {
                    string line;
                    var n = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        n++;
                        if (string.IsNullOrWhiteSpace(line)
                            || line[0] == '#')
                            continue;
                        var res = ParseLine(line, n);
                        if (res != null)
                            yield return res;
                    }
                }
            }
        }

        private static object[] ParseLine(string line, int n)
        {
            return new object[] { line, n };
        }
    }
}
