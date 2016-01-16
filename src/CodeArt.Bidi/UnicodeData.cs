// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;

namespace CodeArt.Bidi
{
    public static class UnicodeData
    {
        private static readonly Dictionary<int, long> Data = InitializeData();

        private static Dictionary<int, long> InitializeData()
        {
            var name = typeof(UnicodeData).GetTypeInfo().Assembly.GetName().Name;
            var library = PlatformServices.Default.LibraryManager.GetLibrary(name);
            var libraryPath = library.Path;
            if (string.Equals(library.Type, "Project", StringComparison.OrdinalIgnoreCase))
            {
                libraryPath = Path.GetDirectoryName(libraryPath);
            }
            else if (string.Equals(library.Type, "Assembly", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cannot load unicod data.");
            }
            var dict = new Dictionary<int, long>();
            Debug.Assert(libraryPath != null, "libraryPath != null");
            using (var fs = File.OpenRead(Path.Combine(libraryPath, "Data", "Unicode.dat")))
            {
                var binaryReader = new BinaryReader(fs);
                do
                {
                    int key;
                    try
                    {
                        key = binaryReader.ReadInt32();
                    }
                    catch (EndOfStreamException)
                    {
                        break;
                    }
                    var val = binaryReader.ReadInt64();
                    dict.Add(key, val);
                } while (true);
            }
            return dict;
        }

        public static BidiDirection GetDirection(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return (BidiDirection)UnicodeDataHelper.GetDirection(data);
        }

        public static BidiDirection GetDirection(char ch) => GetDirection((int)ch);

        public static BracketType GetBracketType(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return (BracketType)UnicodeDataHelper.GetBracketType(data);
        }

        public static BracketType GetBracketType(char ch) => GetBracketType((int)ch);

        public static int GetMatchingBracket(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return data == 0 ? ch : UnicodeDataHelper.GetMatchingBracket(data);
        }

        public static int GetMatchingBracket(char ch) => GetMatchingBracket((int)ch);

        public static int GetMirror(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return data == 0 ? ch : UnicodeDataHelper.GetMirror(data);
        }

        public static int GetMirror(char ch) => GetMirror((int)ch);

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
                    var w = u * 0xffff - 1;
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
    }
}
