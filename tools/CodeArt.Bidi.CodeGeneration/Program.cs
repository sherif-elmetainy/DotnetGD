// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.PlatformAbstractions;

namespace CodeArt.Bidi.CodeGeneration
{
    public class Program
    {
        // ReSharper disable PossibleNullReferenceException
        private static readonly string Namespace =
            typeof(Program).Namespace.Substring(0, typeof(Program).Namespace.LastIndexOf('.'));
        // ReSharper restore PossibleNullReferenceException
        private static readonly Regex BidiBracketsRegex = new Regex(@"^(?<k>[a-fA-F0-9]{4});\s+(?<v>[a-fA-F0-9]{4});\s+(?<t>o|c)");
        private static readonly Regex MirrorRegex = new Regex(@"^(?<k>[a-fA-F0-9]{4});\s+(?<v>[a-fA-F0-9]{4})");
        private static readonly Regex UnicodeDataRegex = new Regex(@"^(?<cp>[a-fA-F0-9]{4,6});(?<n>[^;]*);[^;]*;[^;]*;(?<bd>[^;]*);");
        public static void Main(string[] args)
        {
            var data = new long[UnicodeDataHelper.MaximumUnicodeCodePoint];
            ReadUnicodeData(data);
            ReadBracketData(data);
            ReadMirrorData(data);
            WriteDataFile(data);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static void WriteDataFile(long[] data)
        {
            using (var fs = File.Create(Path.Combine(GetCodeGenerationDirectory(), "Unicode.dat")))
            {
                var binaryWriter = new BinaryWriter(fs);
                for (var index = 0; index < data.Length; index++)
                {
                    var item = data[index];
                    if (item == 0) continue;
                    binaryWriter.Write(index);
                    binaryWriter.Write(item);
                }
            }
        }

        private static string GetCodeGenerationDirectory()
        {
            var appPath = PlatformServices.Default.Application.ApplicationBasePath;
            var currentDir = new DirectoryInfo(appPath);
            Debug.Assert(currentDir.Parent != null, "currentDir.Parent != null");
            Debug.Assert(currentDir.Parent.Parent != null, "currentDir.Parent.Parent != null");
            return Path.Combine(currentDir.Parent.Parent.FullName, "src", Namespace, "Data");
        }

        private static StreamReader OpenDataFile(string name)
        {
            var appPath = PlatformServices.Default.Application.ApplicationBasePath;
            return File.OpenText(Path.Combine(appPath, name));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ReadMirrorData(long[] data)
        {
            using (var reader = OpenDataFile("BidiMirroring.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var m = MirrorRegex.Match(line);
                    if (!m.Success) continue;
                    var key = int.Parse(m.Groups["k"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    var val = int.Parse(m.Groups["v"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    data[key] = UnicodeDataHelper.SetMirror(data[key], val);
                }
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ReadBracketData(long[] data)
        {
            using (var reader = OpenDataFile("BidiBrackets.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var m = BidiBracketsRegex.Match(line);
                    if (!m.Success) continue;
                    var key = int.Parse(m.Groups["k"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    var val = int.Parse(m.Groups["v"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    var type = m.Groups["t"].Value == "o" ? BracketType.Opening : BracketType.Closing;
                    data[key] = UnicodeDataHelper.SetMatchingBracket(data[key], val);
                    data[key] = UnicodeDataHelper.SetBracketType(data[key], (int)type);
                }
            }
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ReadUnicodeData(long[] data)
        {
            using (var reader = OpenDataFile("UnicodeData.txt"))
            {
                var i = 0;
                var wasStart = false;
                string line;
                var lastDirection = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    var m = UnicodeDataRegex.Match(line);
                    if (!m.Success)
                        throw new InvalidOperationException($"Line: {line} does not match.");
                    var cp = int.Parse(m.Groups["cp"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    var dir = (int)(BidiDirection)Enum.Parse(typeof(BidiDirection), m.Groups["bd"].Value, true);
                    var name = m.Groups["n"].Value;
                    var isStart = name.EndsWith("First>", StringComparison.OrdinalIgnoreCase);
                    var isEnd = name.EndsWith("Last>", StringComparison.OrdinalIgnoreCase);

                    if (isEnd)
                    {
                        if (!wasStart)
                            throw new InvalidOperationException($"Line: {line} is end without matching start.");

                        while (i < cp)
                        {
                            data[i] = UnicodeDataHelper.SetDirection(data[i], lastDirection);
                            i++;
                        }
                    }
                    else if (wasStart)
                    {
                        throw new InvalidOperationException($"Line: {line} previous line was start without matching end.");
                    }
                    i = cp;
                    data[i] = UnicodeDataHelper.SetDirection(data[i], dir);
                    lastDirection = dir;
                    wasStart = isStart;
                }
                if (wasStart)
                    throw new InvalidOperationException("Line: Last line was without matching end.");
            }
        }
    }
}
