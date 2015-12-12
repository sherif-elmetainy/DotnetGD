using System;

namespace CodeArt.DotnetGD.Libgd
{
    internal struct GdFont
    {
#pragma warning disable 649
        public int NumberOfCharacters;
        public int Offset;
        public int CharacterWidth;
        public int CharacterHeight;
        public IntPtr Data;
#pragma warning restore 649
    }
}
