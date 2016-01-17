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
    /// <summary>
    /// Get information about unicode characters
    /// </summary>
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
                throw new InvalidOperationException("Cannot load unicode data.");
            }
            var dict = new Dictionary<int, long>();
            Debug.Assert(libraryPath != null, "libraryPath != null");
            if (libraryPath == null) throw new InvalidOperationException("Cannot load unicode data.");
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

        /// <summary>
        /// Gets the unicode bidi category (direction) of a character
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static BidiDirection GetDirection(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return (BidiDirection)UnicodeDataHelper.GetDirection(data);
        }

        /// <summary>
        /// Gets the unicode bidi category (direction) of a character
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static BidiDirection GetDirection(char ch) => GetDirection((int)ch);

        /// <summary>
        /// Get bracket type for a character
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static BracketType GetBracketType(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return (BracketType)UnicodeDataHelper.GetBracketType(data);
        }

        /// <summary>
        /// Get bracket type for a character
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static BracketType GetBracketType(char ch) => GetBracketType((int)ch);

        /// <summary>
        /// Get the matching bracket character for a brachet character. if the character is not a bracket character, the character itself is returned.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int GetMatchingBracket(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return data == 0 ? ch : UnicodeDataHelper.GetMatchingBracket(data);
        }

        /// <summary>
        /// Get the matching bracket character for a brachet character. if the character is not a bracket character, the character itself is returned.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int GetMatchingBracket(char ch) => GetMatchingBracket((int)ch);

        /// <summary>
        /// Get the mirror character of a character (for example the mirror character for [ is ])
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int GetMirror(int ch)
        {
            long data;
            Data.TryGetValue(ch, out data);
            return data == 0 ? ch : UnicodeDataHelper.GetMirror(data);
        }

        /// <summary>
        /// Get the mirror character of a character (for example the mirror character for [ is ])
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int GetMirror(char ch) => GetMirror((int)ch);

        public const int HiSurrogateStart = 0xD800;
        public const int LoSurrogateStart = 0xDC00;
        public const int SurrogateMask = 0xFC00;
        /// <summary>
        /// Minimum code point for a unicode characters having 2 words (2 16-bit chars)
        /// </summary>
        public const int TwoWordCodepointStart = 0x10000;

        /// <summary>
        /// Write a 32-bit code point to a string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="codepoint"></param>
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

        /// <summary>
        /// Gets the 32-bit unicode code point of a string at a given index
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="wordCount">2 for characters whose UTF16 is represented as 2 words, 1 otherwise.</param>
        /// <returns></returns>
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
