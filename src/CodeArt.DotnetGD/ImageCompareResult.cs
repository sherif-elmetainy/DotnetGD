using System;

namespace CodeArt.DotnetGD
{
    [Flags]
    public enum ImageCompareResult
    {
        Similar = 0,
        Image = 1,
        NumberOfColors = 2,
        Color = 4,
        Width = 8,
        Height = 16,
        TransparentColor = 32,
        Background = 64,
        Interlace = 128,
        TrueColor = 256,
        FormattingLoss = Image | NumberOfColors | Color | TransparentColor | Background | TrueColor | Interlace,
        DifferentSize = Width | Height,
        All = Image | NumberOfColors | Color | Width | Height | TransparentColor | Background | Interlace | TrueColor
    }
}