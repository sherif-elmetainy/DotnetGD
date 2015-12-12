using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.PlatformAbstractions;

namespace CodeArt.Bidi.CodeGeneration
{
    public static class Program
    {
        // ReSharper disable PossibleNullReferenceException
        private static readonly string Namespace =
            typeof (Program).Namespace.Substring(0, typeof (Program).Namespace.LastIndexOf('.'));
        // ReSharper restore PossibleNullReferenceException
        private static readonly Regex BidiBracketsRegex = new Regex(@"^(?<k>[a-fA-F0-9]{4});\s+(?<v>[a-fA-F0-9]{4});\s+(?<t>o|c)");
        private static readonly Regex BidiCategoryRegex = new Regex(@"^(?<f>[a-fA-F0-9]{4,})(?:\.\.(?<t>[a-fA-F0-9]{4,}))\s+;\s+(?<c>[A-Z]{1,3})");

        public static void Main(string[] args)
        {
            GenerateFile("BidiBrackets.txt", "UnicodeData.Brackets.cs", WriteBracketsCode);
            GenerateFile("DerivedBidiClass.txt", "UnicodeData.BidiCategories.cs", WriteBidiClassCode);
        }

        private static string GetCodeGenerationDirectory()
        {
            var appPath = PlatformServices.Default.Application.ApplicationBasePath;
            var currentDir = new DirectoryInfo(appPath);
            Debug.Assert(currentDir.Parent != null, "currentDir.Parent != null");
            Debug.Assert(currentDir.Parent.Parent != null, "currentDir.Parent.Parent != null");
            return Path.Combine(currentDir.Parent.Parent.FullName, "src", Namespace);
        }

        private static void GenerateFile(string source, string destination,
            Action<TextWriter, TextReader> generateCodeAction)
        {
            using (var sourceStream = File.OpenRead(source))
            {
                using (var streamReader = new StreamReader(sourceStream))
                {
                    var path = Path.Combine(GetCodeGenerationDirectory(), destination);
                    using (var targetStream = File.Create(path))
                    {
                        using (var streamWriter = new StreamWriter(targetStream))
                        {
                            generateCodeAction(streamWriter, streamReader);
                        }
                    }
                }
            }
        }

        private static void WriteBidiClassCode(TextWriter textWriter, TextReader textReader)
        {
            var data = new List<Tuple<int, int, string>>();
            string line;
            while ((line = textReader.ReadLine()) != null)
            {
                var m = BidiCategoryRegex.Match(line);
                if (!m.Success) continue;
                var from = int.Parse(m.Groups["f"].Value, NumberStyles.HexNumber);
                var to = from;
                if (m.Groups["t"].Length > 0)
                {
                    to = int.Parse(m.Groups["t"].Value, NumberStyles.HexNumber);
                }
                var c = m.Groups["c"].Value;
                data.Add(Tuple.Create(from, to, c));
            }
            data.Sort((t1, t2) => t1.Item1 - t2.Item1);

            textWriter.WriteLine("using System.Collections.Generic;");
            textWriter.WriteLine();
            textWriter.WriteLine("namespace " + Namespace);
            textWriter.WriteLine("{");
            textWriter.WriteLine("\tinternal static partial class UnicodeData");
            textWriter.WriteLine("\t{");
            textWriter.WriteLine(
                "\t\tprivate static readonly List<BidiCategoryRange> BidiCategories = new List<BidiCategoryRange>");
            textWriter.WriteLine("\t\t{");
            var first = true;
            foreach (var item in data)
            {
                if (!first)
                {
                    textWriter.WriteLine(",");
                }
                first = false;
                textWriter.Write($"\t\t\tnew BidiCategoryRange(0x{item.Item1:x4}, 0x{item.Item2:x4}, BidiCategory.{item.Item3})");
            }
            textWriter.WriteLine();
            textWriter.WriteLine("\t\t};");
            textWriter.WriteLine("\t}");
            textWriter.WriteLine("}");
        }

        private static void WriteBracketsCode(TextWriter textWriter, TextReader textReader)
        {
            textWriter.WriteLine("using System.Collections.Generic;");
            textWriter.WriteLine();
            textWriter.WriteLine("namespace " + Namespace);
            textWriter.WriteLine("{");
            textWriter.WriteLine("\tinternal static partial class UnicodeData");
            textWriter.WriteLine("\t{");
            textWriter.WriteLine(
                "\t\tprivate static readonly Dictionary<int, BracketInfo> BracketData = new Dictionary<int, BracketInfo>");
            textWriter.WriteLine("\t\t{");
            var first = true;
            string line;
            while ((line = textReader.ReadLine()) != null)
            {
                var m = BidiBracketsRegex.Match(line);
                if (!m.Success) continue;
                if (!first)
                {
                    textWriter.WriteLine(",");
                }
                first = false;
                var key = m.Groups["k"].Value.ToLowerInvariant();
                var val = m.Groups["v"].Value.ToLowerInvariant();
                var type = m.Groups["t"].Value == "o" ? "Opening" : "Closing";
                textWriter.Write($"\t\t\t[0x{key}] = new BracketInfo(0x{val}, BracketType.{type})");
            }
            textWriter.WriteLine();
            textWriter.WriteLine("\t\t};");
            textWriter.WriteLine("\t}");
            textWriter.WriteLine("}");
        }
    }
}
