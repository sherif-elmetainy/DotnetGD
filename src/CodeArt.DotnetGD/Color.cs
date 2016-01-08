// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Color structure (similar to the one used in System.Drawing)
    /// </summary>
    /// <remarks>
    ///     In libgd true color is represented as 32 bit signed integer. The sign bit is ignored and always zero.
    ///     Also in libgd alpha = 0 means opaque while alpha = 0x7f means transparent. 
    ///     This Color structure has alpha=0 means transparent and alpha = 0xff means opaque. 
    ///     The reason behind the difference is that most .NET developers are familiar with Color structure in System.Drawing
    ///     and also because in HTML alpha=0xff also means opaque. Since the target audience of this wrapper is most likely developing
    ///     web application for ASP.NET 5 and/or a .NET developer familiar Color in system.Drawing, I choose to make it different than libgd 
    ///     and have the library internally convert the alpha part.
    /// 
    ///     The above is only relevant in because of the following:
    ///     1- There is a small loss in alpha resolution cause by libgd (it's 7 bits instead of 8 bits)
    ///     2- When accessing image raw pointer, colors will not work like this structurce and will rather be libgd true color in case of truecolor image
    ///     and color index in case of non true color images.
    /// </remarks>
    public struct Color : IEquatable<Color>
    {
        private const byte AlphaOpaque = 0xff;
        
        public byte A => (byte) ((Argb & 0xFF000000) >> 24);
        public byte R => (byte) ((Argb & 0x00FF0000) >> 16);
        public byte G => (byte)((Argb & 0x0000FF00) >> 8);
        public byte B => (byte)(Argb & 0x000000FF);
        public uint Argb { get; }

        public Color(uint argb)
        {
            Argb = argb;
        }

        public Color(byte r, byte g, byte b) : this(AlphaOpaque, r, g, b)
        {
            
        }

        public Color(byte a, byte r, byte g, byte b)
        {
            Argb = ((r & 0xffu) << 16) | ((g & 0xffu) << 8) | (b & 0xffu) | ((a & (uint)AlphaOpaque) << 24);
        }

        public Color(string htmlColor)
        {
            this = ColorNameConverter.FromHtmlColor(htmlColor);
        }

        public override bool Equals(object obj)
        {
            return (obj as Color?)?.Argb == Argb;
        }

        public override int GetHashCode()
        {
            return unchecked((int)Argb);
        }

        public bool Equals(Color other)
        {
            return other.Argb == Argb;
        }

        public override string ToString()
        {
            return $"#{Argb:x8}";
        }

        public string Name => ColorNameConverter.GetName(this);

        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !c1.Equals(c2);
        }

        public static Color Transparent => new Color(0x00ffffff);
        public static Color AliceBlue => new Color(0xfff0f8ff);
        public static Color AntiqueWhite => new Color(0xfffaebd7);
        public static Color Aqua => new Color(0xff00ffff);
        public static Color Aquamarine => new Color(0xff7fffd4);
        public static Color Azure => new Color(0xfff0ffff);
        public static Color Beige => new Color(0xfff5f5dc);
        public static Color Bisque => new Color(0xffffe4c4);
        public static Color Black => new Color(0xff000000);
        public static Color BlanchedAlmond => new Color(0xffffebcd);
        public static Color Blue => new Color(0xff0000ff);
        public static Color BlueViolet => new Color(0xff8a2be2);
        public static Color Brown => new Color(0xffa52a2a);
        public static Color BurlyWood => new Color(0xffdeb887);
        public static Color CadetBlue => new Color(0xff5f9ea0);
        public static Color Chartreuse => new Color(0xff7fff00);
        public static Color Chocolate => new Color(0xffd2691e);
        public static Color Coral => new Color(0xffff7f50);
        public static Color CornflowerBlue => new Color(0xff6495ed);
        public static Color Cornsilk => new Color(0xfffff8dc);
        public static Color Crimson => new Color(0xffdc143c);
        public static Color Cyan => new Color(0xff00ffff);
        public static Color DarkBlue => new Color(0xff00008b);
        public static Color DarkCyan => new Color(0xff008b8b);
        public static Color DarkGoldenrod => new Color(0xffb8860b);
        public static Color DarkGray => new Color(0xffa9a9a9);
        public static Color DarkGreen => new Color(0xff006400);
        public static Color DarkKhaki => new Color(0xffbdb76b);
        public static Color DarkMagenta => new Color(0xff8b008b);
        public static Color DarkOliveGreen => new Color(0xff556b2f);
        public static Color DarkOrange => new Color(0xffff8c00);
        public static Color DarkOrchid => new Color(0xff9932cc);
        public static Color DarkRed => new Color(0xff8b0000);
        public static Color DarkSalmon => new Color(0xffe9967a);
        public static Color DarkSeaGreen => new Color(0xff8fbc8b);
        public static Color DarkSlateBlue => new Color(0xff483d8b);
        public static Color DarkSlateGray => new Color(0xff2f4f4f);
        public static Color DarkTurquoise => new Color(0xff00ced1);
        public static Color DarkViolet => new Color(0xff9400d3);
        public static Color DeepPink => new Color(0xffff1493);
        public static Color DeepSkyBlue => new Color(0xff00bfff);
        public static Color DimGray => new Color(0xff696969);
        public static Color DodgerBlue => new Color(0xff1e90ff);
        public static Color Firebrick => new Color(0xffb22222);
        public static Color FloralWhite => new Color(0xfffffaf0);
        public static Color ForestGreen => new Color(0xff228b22);
        public static Color Fuchsia => new Color(0xffff00ff);
        public static Color Gainsboro => new Color(0xffdcdcdc);
        public static Color GhostWhite => new Color(0xfff8f8ff);
        public static Color Gold => new Color(0xffffd700);
        public static Color Goldenrod => new Color(0xffdaa520);
        public static Color Gray => new Color(0xff808080);
        public static Color Green => new Color(0xff008000);
        public static Color GreenYellow => new Color(0xffadff2f);
        public static Color Honeydew => new Color(0xfff0fff0);
        public static Color HotPink => new Color(0xffff69b4);
        public static Color IndianRed => new Color(0xffcd5c5c);
        public static Color Indigo => new Color(0xff4b0082);
        public static Color Ivory => new Color(0xfffffff0);
        public static Color Khaki => new Color(0xfff0e68c);
        public static Color Lavender => new Color(0xffe6e6fa);
        public static Color LavenderBlush => new Color(0xfffff0f5);
        public static Color LawnGreen => new Color(0xff7cfc00);
        public static Color LemonChiffon => new Color(0xfffffacd);
        public static Color LightBlue => new Color(0xffadd8e6);
        public static Color LightCoral => new Color(0xfff08080);
        public static Color LightCyan => new Color(0xffe0ffff);
        public static Color LightGoldenrodYellow => new Color(0xfffafad2);
        public static Color LightGreen => new Color(0xff90ee90);
        public static Color LightGray => new Color(0xffd3d3d3);
        public static Color LightPink => new Color(0xffffb6c1);
        public static Color LightSalmon => new Color(0xffffa07a);
        public static Color LightSeaGreen => new Color(0xff20b2aa);
        public static Color LightSkyBlue => new Color(0xff87cefa);
        public static Color LightSlateGray => new Color(0xff778899);
        public static Color LightSteelBlue => new Color(0xffb0c4de);
        public static Color LightYellow => new Color(0xffffffe0);
        public static Color Lime => new Color(0xff00ff00);
        public static Color LimeGreen => new Color(0xff32cd32);
        public static Color Linen => new Color(0xfffaf0e6);
        public static Color Magenta => new Color(0xffff00ff);
        public static Color Maroon => new Color(0xff800000);
        public static Color MediumAquamarine => new Color(0xff66cdaa);
        public static Color MediumBlue => new Color(0xff0000cd);
        public static Color MediumOrchid => new Color(0xffba55d3);
        public static Color MediumPurple => new Color(0xff9370db);
        public static Color MediumSeaGreen => new Color(0xff3cb371);
        public static Color MediumSlateBlue => new Color(0xff7b68ee);
        public static Color MediumSpringGreen => new Color(0xff00fa9a);
        public static Color MediumTurquoise => new Color(0xff48d1cc);
        public static Color MediumVioletRed => new Color(0xffc71585);
        public static Color MidnightBlue => new Color(0xff191970);
        public static Color MintCream => new Color(0xfff5fffa);
        public static Color MistyRose => new Color(0xffffe4e1);
        public static Color Moccasin => new Color(0xffffe4b5);
        public static Color NavajoWhite => new Color(0xffffdead);
        public static Color Navy => new Color(0xff000080);
        public static Color OldLace => new Color(0xfffdf5e6);
        public static Color Olive => new Color(0xff808000);
        public static Color OliveDrab => new Color(0xff6b8e23);
        public static Color Orange => new Color(0xffffa500);
        public static Color OrangeRed => new Color(0xffff4500);
        public static Color Orchid => new Color(0xffda70d6);
        public static Color PaleGoldenrod => new Color(0xffeee8aa);
        public static Color PaleGreen => new Color(0xff98fb98);
        public static Color PaleTurquoise => new Color(0xffafeeee);
        public static Color PaleVioletRed => new Color(0xffdb7093);
        public static Color PapayaWhip => new Color(0xffffefd5);
        public static Color PeachPuff => new Color(0xffffdab9);
        public static Color Peru => new Color(0xffcd853f);
        public static Color Pink => new Color(0xffffc0cb);
        public static Color Plum => new Color(0xffdda0dd);
        public static Color PowderBlue => new Color(0xffb0e0e6);
        public static Color Purple => new Color(0xff800080);
        public static Color Red => new Color(0xffff0000);
        public static Color RosyBrown => new Color(0xffbc8f8f);
        public static Color RoyalBlue => new Color(0xff4169e1);
        public static Color SaddleBrown => new Color(0xff8b4513);
        public static Color Salmon => new Color(0xfffa8072);
        public static Color SandyBrown => new Color(0xfff4a460);
        public static Color SeaGreen => new Color(0xff2e8b57);
        public static Color SeaShell => new Color(0xfffff5ee);
        public static Color Sienna => new Color(0xffa0522d);
        public static Color Silver => new Color(0xffc0c0c0);
        public static Color SkyBlue => new Color(0xff87ceeb);
        public static Color SlateBlue => new Color(0xff6a5acd);
        public static Color SlateGray => new Color(0xff708090);
        public static Color Snow => new Color(0xfffffafa);
        public static Color SpringGreen => new Color(0xff00ff7f);
        public static Color SteelBlue => new Color(0xff4682b4);
        public static Color Tan => new Color(0xffd2b48c);
        public static Color Teal => new Color(0xff008080);
        public static Color Thistle => new Color(0xffd8bfd8);
        public static Color Tomato => new Color(0xffff6347);
        public static Color Turquoise => new Color(0xff40e0d0);
        public static Color Violet => new Color(0xffee82ee);
        public static Color Wheat => new Color(0xfff5deb3);
        public static Color White => new Color(0xffffffff);
        public static Color WhiteSmoke => new Color(0xfff5f5f5);
        public static Color Yellow => new Color(0xffffff00);
        public static Color YellowGreen => new Color(0xff9acd32);
    }
}
