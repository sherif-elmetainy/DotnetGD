// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CodeArt.DotnetGD
{
    internal static class ColorNameConverter
    {
        private static readonly Dictionary<string, Color> NameToColorDictionary = InitializeNameToColorDictionary();

        private static Dictionary<string, Color> InitializeNameToColorDictionary()
        {
            var dict = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(p => p.PropertyType == typeof(Color)))
            {
                var color = (Color) property.GetValue(null);
                var name = property.Name.ToLowerInvariant();
                dict.Add(name, color);
                if (name.EndsWith("gray"))
                {
                    dict.Add(name.Replace("gray", "grey"), color);
                }
            }

            return dict;
        }

        private static readonly Dictionary<Color, string> ColorToNameDictionary = InitializeColorToNameDictionary();


        private static Dictionary<Color, string> InitializeColorToNameDictionary()
        {
            var dict = new Dictionary<Color, string>();
            foreach (var property in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(Color)))
            {
                var color = (Color)property.GetValue(null);
                dict.Add(color, property.Name.ToLowerInvariant());
            }
            return dict;
        }

        public static Color FromHtmlColor(string htmlColor)
        {
            if (string.IsNullOrWhiteSpace(htmlColor))
                throw new ArgumentNullException(nameof(htmlColor));
            htmlColor = htmlColor.Trim();
            if (htmlColor[0] == '#')
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (htmlColor.Length == 9) // #aarrggbb
                {
                    return new Color(uint.Parse(htmlColor.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                }
                if (htmlColor.Length == 7) // #rrggbb
                {
                    return new Color(0xff000000 | uint.Parse(htmlColor.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                }
                if (htmlColor.Length == 4) // #rgb
                {
                    return new Color(uint.Parse($"ff{htmlColor[1]}{htmlColor[1]}{htmlColor[2]}{htmlColor[2]}{htmlColor[3]}{htmlColor[3]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                }
            }
            Color color;
            if (NameToColorDictionary.TryGetValue(htmlColor, out color))
            {
                return color;
            }
            throw new ArgumentOutOfRangeException(nameof(htmlColor), htmlColor, "Invalid html color value.");   
        }

        public static string GetName(Color color)
        {
            string name;
            return ColorToNameDictionary.TryGetValue(color, out name) ? name : color.ToString();
        }
    }
}

