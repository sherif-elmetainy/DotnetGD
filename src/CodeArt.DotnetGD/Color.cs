using System;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Color structure (similar to the one used in System.Drawing)
    /// </summary>
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

        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !c1.Equals(c2);
        }
    }
}
