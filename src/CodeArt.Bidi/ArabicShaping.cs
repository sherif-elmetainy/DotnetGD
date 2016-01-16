// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

/*
* Arabic sharping implementation ported from the ICU Project
* http://site.icu-project.org/
* Original code license: http://site.icu-project.org/
* Original Code ported from java to C# by Sherif Elmetainy 
* The credits and copyright info for the original code listed below
*/

/*
*******************************************************************************
*   Copyright (C) 2001-2009, International Business Machines
*   Corporation and others.  All Rights Reserved.
*******************************************************************************
*/



using System;
using System.Text;
using CodeArt.Bidi;
// ReSharper disable SuggestBaseTypeForParameter

/**
* Shape Arabic text on a character basis.
*
* <p>ArabicShaping performs basic operations for "shaping" Arabic text. It is most
* useful for use with legacy data formats and legacy display technology
* (simple terminals). All operations are performed on Unicode characters.</p>
*
* <p>Text-based shaping means that some character code points in the text are
* replaced by others depending on the context. It transforms one kind of text
* into another. In comparison, modern displays for Arabic text select
* appropriate, context-dependent font glyphs for each text element, which means
* that they transform text into a glyph vector.</p>
*
* <p>Text transformations are necessary when modern display technology is not
* available or when text needs to be transformed to or from legacy formats that
* use "shaped" characters. Since the Arabic script is cursive, connecting
* adjacent letters to each other, computers select images for each letter based
* on the surrounding letters. This usually results in four images per Arabic
* letter: initial, middle, final, and isolated forms. In Unicode, on the other
* hand, letters are normally stored abstract, and a display system is expected
* to select the necessary glyphs. (This makes searching and other text
* processing easier because the same letter has only one code.) It is possible
* to mimic this with text transformations because there are characters in
* Unicode that are rendered as letters with a specific shape
* (or cursive connectivity). They were included for interoperability with
* legacy systems and codepages, and for unsophisticated display systems.</p>
*
* <p>A second kind of text transformations is supported for Arabic digits:
* For compatibility with legacy codepages that only include European digits,
* it is possible to replace one set of digits by another, changing the
* character code points. These operations can be performed for either
* Arabic-Indic Digits (U+0660...U+0669) or Eastern (Extended) Arabic-Indic
* digits (U+06f0...U+06f9).</p>
*
* <p>Some replacements may result in more or fewer characters (code points).
* By default, this means that the destination buffer may receive text with a
* length different from the source length. Some legacy systems rely on the
* length of the text to be constant. They expect extra spaces to be added
* or consumed either next to the affected character or at the end of the
* text.</p>
* @stable ICU 2.0
*
* @hide
*/
public class ArabicShaping
{
    private readonly ArabicShapingOptions _options;
    private readonly bool _isLogical; // convenience
    private readonly bool _spacesRelativeToTextBeginEnd;
    private readonly char _tailChar;
    public static readonly ArabicShaping Shaper = new ArabicShaping(
            ArabicShapingOptions.TextDirectionLogical |
            ArabicShapingOptions.LengthFixedSpacesNear |
            ArabicShapingOptions.LettersShape |
            ArabicShapingOptions.DigitsNoop);
    /**
     * Convert a range of text in the source array, putting the result
     * into a range of text in the destination array, and return the number
     * of characters written.
     *
     * @param source An array containing the input text
     * @param sourceStart The start of the range of text to convert
     * @param sourceLength The length of the range of text to convert
     * @param dest The destination array that will receive the result.
     *   It may be <code>NULL</code> only if  <code>destSize</code> is 0.
     * @param destStart The start of the range of the destination buffer to use.
     * @param destSize The size (capacity) of the destination buffer.
     *   If <code>destSize</code> is 0, then no output is produced,
     *   but the necessary buffer size is returned ("preflighting").  This
     *   does not validate the text against the options, for example,
     *   if letters are being unshaped, and spaces are being consumed
     *   following lamalef, this will not detect a lamalef without a
     *   corresponding space.  An error will be thrown when the actual
     *   conversion is attempted.
     * @return The number of chars written to the destination buffer.
     *   If an error occurs, then no output was written, or it may be
     *   incomplete.
     * @throws ArabicShapingException if the text cannot be converted according to the options.
     * @stable ICU 2.0
     */

    public int Shape(char[] source, int sourceStart, int sourceLength,
        char[] dest, int destStart, int destSize)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (sourceStart < 0 || sourceLength < 0 || sourceStart + sourceLength > source.Length)
        {
            throw new ArgumentException(
                $"bad source start ({sourceStart}) or length ({sourceLength}) for buffer of length {source.Length}");
        }
        if (destSize != 0)
        {
            if (dest == null)
            {
                throw new ArgumentNullException(nameof(dest));
            }
            if (destStart < 0 || destSize < 0 || destStart + destSize > dest.Length)
            {
                throw new ArgumentException(
                    $"bad dest start ({destStart}) or size ({destSize}) for buffer of length {dest.Length}");
            }
        }

        /* Validate input options */
        if (((_options & ArabicShapingOptions.TashkeelMask) > 0) &&
             !(((_options & ArabicShapingOptions.TashkeelMask) == ArabicShapingOptions.TashkeelBegin) ||
               ((_options & ArabicShapingOptions.TashkeelMask) == ArabicShapingOptions.TashkeelEnd) ||
               ((_options & ArabicShapingOptions.TashkeelMask) == ArabicShapingOptions.TashkeelResize) ||
               ((_options & ArabicShapingOptions.TashkeelMask) == ArabicShapingOptions.TashkeelReplaceByTatweel)))
        {
            throw new ArgumentException("Wrong Tashkeel argument");
        }

        if (((_options & ArabicShapingOptions.LamalefMask) > 0) &&
               !(((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefBegin) ||
                 ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefEnd) ||
                 ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefResize) ||
                  ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefAuto) ||
                  ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefNear)))
        {
            throw new ArgumentException("Wrong Lam Alef argument");
        }
        /* Validate Tashkeel (Tashkeel replacement options should be enabled in shaping mode only)*/
        if (((_options & ArabicShapingOptions.TashkeelMask) > 0) && (_options & ArabicShapingOptions.LettersMask) == ArabicShapingOptions.LettersUnshape)
        {
            throw new ArgumentException("Tashkeel replacement should not be enabled in deshaping mode ");
        }
        return InternalShape(source, sourceStart, sourceLength, dest, destStart, destSize);
    }
    /**
     * Convert a range of text in place.  This may only be used if the Length option
     * does not grow or shrink the text.
     *
     * @param source An array containing the input text
     * @param start The start of the range of text to convert
     * @param length The length of the range of text to convert
     * @throws ArabicShapingException if the text cannot be converted according to the options.
     * @stable ICU 2.0
     */
    public void Shape(char[] source, int start, int length)
    {
        if ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefResize)
        {
            throw new ArabicShapingException("Cannot shape in place with length option resize.");
        }
        Shape(source, start, length, source, start, length);
    }
    /**
     * Convert a string, returning the new string.
     *
     * @param text the string to convert
     * @return the converted string
     * @throws ArabicShapingException if the string cannot be converted according to the options.
     * @stable ICU 2.0
     */
    public String Shape(String text)
    {
        var src = text.ToCharArray();
        var dest = src;
        if (((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefResize) &&
            ((_options & ArabicShapingOptions.LettersMask) == ArabicShapingOptions.LettersUnshape))
        {
            dest = new char[src.Length * 2]; // max
        }
        var len = Shape(src, 0, src.Length, dest, 0, dest.Length);
        return new String(dest, 0, len);
    }
    /**
     * Construct ArabicShaping using the options flags.
     * The flags are as follows:<br>
     * 'LENGTH' flags control whether the text can change size, and if not,
     * how to maintain the size of the text when LamAlef ligatures are
     * formed or broken.<br>
     * 'TEXT_DIRECTION' flags control whether the text is read and written
     * in visual order or in logical order.<br>
     * 'LETTERS_SHAPE' flags control whether conversion is to or from
     * presentation forms.<br>
     * 'DIGITS' flags control whether digits are shaped, and whether from
     * European to Arabic-Indic or vice-versa.<br>
     * 'DIGIT_TYPE' flags control whether standard or extended Arabic-Indic
     * digits are used when performing digit conversion.
     * @stable ICU 2.0
     */
    public ArabicShaping(ArabicShapingOptions options)
    {
        _options = options;
        if ((int)(options & ArabicShapingOptions.DigitsMask) > 0x80)
        {
            throw new ArgumentException("bad DIGITS options");
        }
        _isLogical = (options & ArabicShapingOptions.TextDirectionMask) == ArabicShapingOptions.TextDirectionLogical;
        /* Validate options */
        _spacesRelativeToTextBeginEnd = (options & ArabicShapingOptions.SpacesRelativeToTextMask) == ArabicShapingOptions.SpacesRelativeToTextBeginEnd;
        _tailChar = (options & ArabicShapingOptions.ShapeTailTypeMask) == ArabicShapingOptions.ShapeTailNewUnicode ? NewTailChar : OldTailChar;
    }

    private const char HamzafeChar = '\ufe80';
    private const char Hamza06Char = '\u0621';
    private const char YehHamzaChar = '\u0626';
    private const char YehHamzafeChar = '\uFE89';
    private const char LamalefSpaceSub = '\uffff';
    private const char TashkeelSpaceSub = '\ufffe';
    private const char LamChar = '\u0644';
    private const char SpaceChar = '\u0020';
    private const char SpaceCharForLamalef = '\ufeff'; // XXX: tweak for TextLine use
    private const char ShaddaChar = '\uFE7C';
    private const char TatweelChar = '\u0640';
    private const char ShaddaTatweelChar = '\uFE7D';
    private const char NewTailChar = '\uFE73';
    private const char OldTailChar = '\u200B';
    private const int ShapeMode = 0;
    private const int DeshapeMode = 1;
    /**
     * @stable ICU 2.0
     */
    public override bool Equals(object obj)
    {
        return Equals(obj as ArabicShaping);
    }

    public bool Equals(ArabicShaping other)
    {
        return other?._options == _options;
    }
    /**
     * @stable ICU 2.0
     */
    ///CLOVER:OFF
    public override int GetHashCode()
    {
        return (int)_options;
    }
    /**
     * @stable ICU 2.0
     */
    public override string ToString()
    {
        var buf = new StringBuilder(base.ToString());
        buf.Append('[');
        switch (_options & ArabicShapingOptions.LamalefMask)
        {
            case ArabicShapingOptions.LamalefResize: buf.Append("LamAlef resize"); break;
            case ArabicShapingOptions.LamalefNear: buf.Append("LamAlef spaces at near"); break;
            case ArabicShapingOptions.LamalefBegin: buf.Append("LamAlef spaces at begin"); break;
            case ArabicShapingOptions.LamalefEnd: buf.Append("LamAlef spaces at end"); break;
            case ArabicShapingOptions.LamalefAuto: buf.Append("lamAlef auto"); break;
        }
        switch (_options & ArabicShapingOptions.TextDirectionMask)
        {
            case ArabicShapingOptions.TextDirectionLogical: buf.Append(", logical"); break;
            case ArabicShapingOptions.TextDirectionVisualLtr: buf.Append(", visual"); break;
        }
        switch (_options & ArabicShapingOptions.LettersMask)
        {
            case ArabicShapingOptions.LettersNoop: buf.Append(", no letter shaping"); break;
            case ArabicShapingOptions.LettersShape: buf.Append(", shape letters"); break;
            case ArabicShapingOptions.LettersShapeTashkeelIsolated: buf.Append(", shape letters tashkeel isolated"); break;
            case ArabicShapingOptions.LettersUnshape: buf.Append(", unshape letters"); break;
        }
        switch (_options & ArabicShapingOptions.SeenMask)
        {
            case ArabicShapingOptions.SeenTwocellNear: buf.Append(", Seen at near"); break;
        }
        switch (_options & ArabicShapingOptions.YehhamzaMask)
        {
            case ArabicShapingOptions.YehhamzaTwocellNear: buf.Append(", Yeh Hamza at near"); break;
        }
        switch (_options & ArabicShapingOptions.TashkeelMask)
        {
            case ArabicShapingOptions.TashkeelBegin: buf.Append(", Tashkeel at begin"); break;
            case ArabicShapingOptions.TashkeelEnd: buf.Append(", Tashkeel at end"); break;
            case ArabicShapingOptions.TashkeelReplaceByTatweel: buf.Append(", Tashkeel replace with tatweel"); break;
            case ArabicShapingOptions.TashkeelResize: buf.Append(", Tashkeel resize"); break;
        }
        switch (_options & ArabicShapingOptions.DigitsMask)
        {
            case ArabicShapingOptions.DigitsNoop: buf.Append(", no digit shaping"); break;
            case ArabicShapingOptions.DigitsEN2AN: buf.Append(", shape digits to AN"); break;
            case ArabicShapingOptions.DigitsAN2EN: buf.Append(", shape digits to EN"); break;
            case ArabicShapingOptions.DigitsEN2ANInitLr: buf.Append(", shape digits to AN contextually: default EN"); break;
            case ArabicShapingOptions.DigitsEN2ANInitAL: buf.Append(", shape digits to AN contextually: default AL"); break;
        }
        switch (_options & ArabicShapingOptions.DigitTypeMask)
        {
            case ArabicShapingOptions.DigitTypeAN: buf.Append(", standard Arabic-Indic digits"); break;
            case ArabicShapingOptions.DigitTypeANExtended: buf.Append(", extended Arabic-Indic digits"); break;
        }
        buf.Append("]");
        return buf.ToString();
    }
    ///CLOVER:ON
    //
    // ported api
    //
    private const int Irrelevant = 4;
    private const int Lamtype = 16;
    private const int Aleftype = 32;
    private const int Linkr = 1;
    private const int Linkl = 2;
    private const int LinkMask = 3;
    private readonly int[] _irrelevantPos = { 0x0, 0x2, 0x4, 0x6, 0x8, 0xA, 0xC, 0xE };
    /*
        private readonly char[] convertLamAlef =  {
            '\u0622', // FEF5
            '\u0622', // FEF6
            '\u0623', // FEF7
            '\u0623', // FEF8
            '\u0625', // FEF9
            '\u0625', // FEFA
            '\u0627', // FEFB
            '\u0627'  // FEFC
        };
    */
    private static readonly int[] TailFamilyIsolatedFinal =
    {
        /* FEB1 */ 1,
        /* FEB2 */ 1,
        /* FEB3 */ 0,
        /* FEB4 */ 0,
        /* FEB5 */ 1,
        /* FEB6 */ 1,
        /* FEB7 */ 0,
        /* FEB8 */ 0,
        /* FEB9 */ 1,
        /* FEBA */ 1,
        /* FEBB */ 0,
        /* FEBC */ 0,
        /* FEBD */ 1,
        /* FEBE */ 1
    };

    private static readonly int[] TashkeelMedial =
    {
        /* FE70 */ 0,
        /* FE71 */ 1,
        /* FE72 */ 0,
        /* FE73 */ 0,
        /* FE74 */ 0,
        /* FE75 */ 0,
        /* FE76 */ 0,
        /* FE77 */ 1,
        /* FE78 */ 0,
        /* FE79 */ 1,
        /* FE7A */ 0,
        /* FE7B */ 1,
        /* FE7C */ 0,
        /* FE7D */ 1,
        /* FE7E */ 0,
        /* FE7F */ 1
    };

    private static readonly char[] YehHamzaToYeh =
    {
        /* isolated*/ '\uFEEF',
        /* final   */ '\uFEF0'
    };

    private static readonly char[] ConvertNormalizedLamAlef =
    {
        '\u0622', // 065C
        '\u0623', // 065D
        '\u0625', // 065E
        '\u0627' // 065F
    };

    private static readonly int[] AraLink = {
        1           + 32 + 256 * 0x11,  /*0x0622*/
        1           + 32 + 256 * 0x13,  /*0x0623*/
        1                + 256 * 0x15,  /*0x0624*/
        1           + 32 + 256 * 0x17,  /*0x0625*/
        1 + 2            + 256 * 0x19,  /*0x0626*/
        1           + 32 + 256 * 0x1D,  /*0x0627*/
        1 + 2            + 256 * 0x1F,  /*0x0628*/
        1                + 256 * 0x23,  /*0x0629*/
        1 + 2            + 256 * 0x25,  /*0x062A*/
        1 + 2            + 256 * 0x29,  /*0x062B*/
        1 + 2            + 256 * 0x2D,  /*0x062C*/
        1 + 2            + 256 * 0x31,  /*0x062D*/
        1 + 2            + 256 * 0x35,  /*0x062E*/
        1                + 256 * 0x39,  /*0x062F*/
        1                + 256 * 0x3B,  /*0x0630*/
        1                + 256 * 0x3D,  /*0x0631*/
        1                + 256 * 0x3F,  /*0x0632*/
        1 + 2            + 256 * 0x41,  /*0x0633*/
        1 + 2            + 256 * 0x45,  /*0x0634*/
        1 + 2            + 256 * 0x49,  /*0x0635*/
        1 + 2            + 256 * 0x4D,  /*0x0636*/
        1 + 2            + 256 * 0x51,  /*0x0637*/
        1 + 2            + 256 * 0x55,  /*0x0638*/
        1 + 2            + 256 * 0x59,  /*0x0639*/
        1 + 2            + 256 * 0x5D,  /*0x063A*/
        0, 0, 0, 0, 0,                  /*0x063B-0x063F*/
        1 + 2,                          /*0x0640*/
        1 + 2            + 256 * 0x61,  /*0x0641*/
        1 + 2            + 256 * 0x65,  /*0x0642*/
        1 + 2            + 256 * 0x69,  /*0x0643*/
        1 + 2       + 16 + 256 * 0x6D,  /*0x0644*/
        1 + 2            + 256 * 0x71,  /*0x0645*/
        1 + 2            + 256 * 0x75,  /*0x0646*/
        1 + 2            + 256 * 0x79,  /*0x0647*/
        1                + 256 * 0x7D,  /*0x0648*/
        1                + 256 * 0x7F,  /*0x0649*/
        1 + 2            + 256 * 0x81,  /*0x064A*/
        4, 4, 4, 4,                     /*0x064B-0x064E*/
        4, 4, 4, 4,                     /*0x064F-0x0652*/
        4, 4, 4, 0, 0,                  /*0x0653-0x0657*/
        0, 0, 0, 0,                     /*0x0658-0x065B*/
        1                + 256 * 0x85,  /*0x065C*/
        1                + 256 * 0x87,  /*0x065D*/
        1                + 256 * 0x89,  /*0x065E*/
        1                + 256 * 0x8B,  /*0x065F*/
        0, 0, 0, 0, 0,                  /*0x0660-0x0664*/
        0, 0, 0, 0, 0,                  /*0x0665-0x0669*/
        0, 0, 0, 0, 0, 0,               /*0x066A-0x066F*/
        4,                              /*0x0670*/
        0,                              /*0x0671*/
        1           + 32,               /*0x0672*/
        1           + 32,               /*0x0673*/
        0,                              /*0x0674*/
        1           + 32,               /*0x0675*/
        1, 1,                           /*0x0676-0x0677*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x0678-0x067D*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x067E-0x0683*/
        1+2, 1+2, 1+2, 1+2,             /*0x0684-0x0687*/
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /*0x0688-0x0691*/
        1, 1, 1, 1, 1, 1, 1, 1,         /*0x0692-0x0699*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x069A-0x06A3*/
        1+2, 1+2, 1+2, 1+2,             /*0x069A-0x06A3*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x06A4-0x06AD*/
        1+2, 1+2, 1+2, 1+2,             /*0x06A4-0x06AD*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x06AE-0x06B7*/
        1+2, 1+2, 1+2, 1+2,             /*0x06AE-0x06B7*/
        1+2, 1+2, 1+2, 1+2, 1+2, 1+2,   /*0x06B8-0x06BF*/
        1+2, 1+2,                       /*0x06B8-0x06BF*/
        1,                              /*0x06C0*/
        1+2,                            /*0x06C1*/
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /*0x06C2-0x06CB*/
        1+2,                            /*0x06CC*/
        1,                              /*0x06CD*/
        1+2, 1+2, 1+2, 1+2,             /*0x06CE-0x06D1*/
        1, 1                            /*0x06D2-0x06D3*/
    };

    private static readonly int[] PresLink =
    {
        1 + 2,                        /*0xFE70*/
        1 + 2,                        /*0xFE71*/
        1 + 2, 0, 1+ 2, 0, 1+ 2,      /*0xFE72-0xFE76*/
        1 + 2,                        /*0xFE77*/
        1+ 2, 1 + 2, 1+2, 1 + 2,      /*0xFE78-0xFE81*/
        1+ 2, 1 + 2, 1+2, 1 + 2,      /*0xFE82-0xFE85*/
        0, 0 + 32, 1 + 32, 0 + 32,    /*0xFE86-0xFE89*/
        1 + 32, 0, 1,  0 + 32,        /*0xFE8A-0xFE8D*/
        1 + 32, 0, 2,  1 + 2,         /*0xFE8E-0xFE91*/
        1, 0 + 32, 1 + 32, 0,         /*0xFE92-0xFE95*/
        2, 1 + 2, 1, 0,               /*0xFE96-0xFE99*/
        1, 0, 2, 1 + 2,               /*0xFE9A-0xFE9D*/
        1, 0, 2, 1 + 2,               /*0xFE9E-0xFEA1*/
        1, 0, 2, 1 + 2,               /*0xFEA2-0xFEA5*/
        1, 0, 2, 1 + 2,               /*0xFEA6-0xFEA9*/
        1, 0, 2, 1 + 2,               /*0xFEAA-0xFEAD*/
        1, 0, 1, 0,                   /*0xFEAE-0xFEB1*/
        1, 0, 1, 0,                   /*0xFEB2-0xFEB5*/
        1, 0, 2, 1+2,                 /*0xFEB6-0xFEB9*/
        1, 0, 2, 1+2,                 /*0xFEBA-0xFEBD*/
        1, 0, 2, 1+2,                 /*0xFEBE-0xFEC1*/
        1, 0, 2, 1+2,                 /*0xFEC2-0xFEC5*/
        1, 0, 2, 1+2,                 /*0xFEC6-0xFEC9*/
        1, 0, 2, 1+2,                 /*0xFECA-0xFECD*/
        1, 0, 2, 1+2,                 /*0xFECE-0xFED1*/
        1, 0, 2, 1+2,                 /*0xFED2-0xFED5*/
        1, 0, 2, 1+2,                 /*0xFED6-0xFED9*/
        1, 0, 2, 1+2,                 /*0xFEDA-0xFEDD*/
        1, 0, 2, 1+2,                 /*0xFEDE-0xFEE1*/
        1, 0 + 16, 2 + 16, 1 + 2 +16, /*0xFEE2-0xFEE5*/
        1 + 16, 0, 2, 1+2,            /*0xFEE6-0xFEE9*/
        1, 0, 2, 1+2,                 /*0xFEEA-0xFEED*/
        1, 0, 2, 1+2,                 /*0xFEEE-0xFEF1*/
        1, 0, 1, 0,                   /*0xFEF2-0xFEF5*/
        1, 0, 2, 1+2,                 /*0xFEF6-0xFEF9*/
        1, 0, 1, 0,                   /*0xFEFA-0xFEFD*/
        1, 0, 1, 0,
        1
    };

    private static readonly int[] ConvertFEto06 = {
        /***********0******1******2******3******4******5******6******7******8******9******A******B******C******D******E******F***/
        /*FE7*/   0x64B, 0x64B, 0x64C, 0x64C, 0x64D, 0x64D, 0x64E, 0x64E, 0x64F, 0x64F, 0x650, 0x650, 0x651, 0x651, 0x652, 0x652,
        /*FE8*/   0x621, 0x622, 0x622, 0x623, 0x623, 0x624, 0x624, 0x625, 0x625, 0x626, 0x626, 0x626, 0x626, 0x627, 0x627, 0x628,
        /*FE9*/   0x628, 0x628, 0x628, 0x629, 0x629, 0x62A, 0x62A, 0x62A, 0x62A, 0x62B, 0x62B, 0x62B, 0x62B, 0x62C, 0x62C, 0x62C,
        /*FEA*/   0x62C, 0x62D, 0x62D, 0x62D, 0x62D, 0x62E, 0x62E, 0x62E, 0x62E, 0x62F, 0x62F, 0x630, 0x630, 0x631, 0x631, 0x632,
        /*FEB*/   0x632, 0x633, 0x633, 0x633, 0x633, 0x634, 0x634, 0x634, 0x634, 0x635, 0x635, 0x635, 0x635, 0x636, 0x636, 0x636,
        /*FEC*/   0x636, 0x637, 0x637, 0x637, 0x637, 0x638, 0x638, 0x638, 0x638, 0x639, 0x639, 0x639, 0x639, 0x63A, 0x63A, 0x63A,
        /*FED*/   0x63A, 0x641, 0x641, 0x641, 0x641, 0x642, 0x642, 0x642, 0x642, 0x643, 0x643, 0x643, 0x643, 0x644, 0x644, 0x644,
        /*FEE*/   0x644, 0x645, 0x645, 0x645, 0x645, 0x646, 0x646, 0x646, 0x646, 0x647, 0x647, 0x647, 0x647, 0x648, 0x648, 0x649,
        /*FEF*/   0x649, 0x64A, 0x64A, 0x64A, 0x64A, 0x65C, 0x65C, 0x65D, 0x65D, 0x65E, 0x65E, 0x65F, 0x65F
    };

    private static readonly int[][][] ShapeTable =
    {
        new [] { new [] {0,0,0,0}, new [] {0,0,0,0}, new [] {0,1,0,3}, new [] {0,1,0,1} },
        new [] { new [] {0,0,2,2}, new [] {0,0,1,2}, new [] {0,1,1,2}, new [] {0,1,1,3} },
        new [] { new [] {0,0,0,0}, new [] {0,0,0,0}, new [] {0,1,0,3}, new [] {0,1,0,3} },
        new [] { new [] {0,0,1,2}, new [] {0,0,1,2}, new [] {0,1,1,2}, new [] {0,1,1,3} }
    };

    /*
     * This function shapes European digits to Arabic-Indic digits
     * in-place, writing over the input characters.  Data is in visual
     * order.
     */
    private void ShapeToArabicDigitsWithContext(char[] dest,
                                                int start,
                                                int length,
                                                char digitBase,
                                                bool lastStrongWasAL)
    {
        digitBase -= '0'; // move common adjustment out of loop
        for (var i = start + length; --i >= start;)
        {
            var ch = dest[i];
            switch (UnicodeData.GetDirection(ch))
            {
                case BidiDirection.L:
                case BidiDirection.R:
                    lastStrongWasAL = false;
                    break;
                case BidiDirection.AL:
                    lastStrongWasAL = true;
                    break;
                case BidiDirection.EN:
                    if (lastStrongWasAL && ch <= '\u0039')
                    {
                        dest[i] = (char)(ch + digitBase);
                    }
                    break;
            }
        }
    }

    /*
     * Name    : invertBuffer
     * Function: This function inverts the buffer, it's used
     *           in case the user specifies the buffer to be
     *           TEXT_DIRECTION_LOGICAL
     */
    private static void InvertBuffer(char[] buffer,
                                     int start,
                                     int length)
    {
        for (int i = start, j = start + length - 1; i < j; i++, --j)
        {
            var temp = buffer[i];
            buffer[i] = buffer[j];
            buffer[j] = temp;
        }
    }

    /*
     * Name    : changeLamAlef
     * Function: Converts the Alef characters into an equivalent
     *           LamAlef location in the 0x06xx Range, this is an
     *           intermediate stage in the operation of the program
     *           later it'll be converted into the 0xFExx LamAlefs
     *           in the shaping function.
     */
    private static char ChangeLamAlef(char ch)
    {
        switch (ch)
        {
            case '\u0622': return '\u065C';
            case '\u0623': return '\u065D';
            case '\u0625': return '\u065E';
            case '\u0627': return '\u065F';
            default: return '\u0000'; // not a lamalef
        }
    }

    /*
     * Name    : specialChar
     * Function: Special Arabic characters need special handling in the shapeUnicode
     *           function, this function returns 1 or 2 for these special characters
     */
    private static int SpecialChar(char ch)
    {
        if ((ch > '\u0621' && ch < '\u0626') ||
            (ch == '\u0627') ||
            (ch > '\u062E' && ch < '\u0633') ||
            (ch > '\u0647' && ch < '\u064A') ||
            (ch == '\u0629'))
        {
            return 1;
        }
        if (ch >= '\u064B' && ch <= '\u0652')
        {
            return 2;
        }
        if (ch >= 0x0653 && ch <= 0x0655 ||
            ch == 0x0670 ||
            ch >= 0xFE70 && ch <= 0xFE7F)
        {
            return 3;
        }
        return 0;
    }

    /*
     * Name    : getLink
     * Function: Resolves the link between the characters as
     *           Arabic characters have four forms :
     *           Isolated, Initial, Middle and Final Form
     */
    private static int GetLink(char ch)
    {
        if (ch >= '\u0622' && ch <= '\u06D3')
        {
            return AraLink[ch - '\u0622'];
        }
        if (ch == '\u200D')
        {
            return 3;
        }
        if (ch >= '\u206D' && ch <= '\u206F')
        {
            return 4;
        }
        if (ch >= '\uFE70' && ch <= '\uFEFC')
        {
            return PresLink[ch - '\uFE70'];
        }
        return 0;
    }

    /*
     * Name    : countSpaces
     * Function: Counts the number of spaces
     *           at each end of the logical buffer
     */
    private static int CountSpacesLeft(char[] dest,
                                       int start,
                                       int count)
    {
        for (int i = start, e = start + count; i < e; ++i)
        {
            if (dest[i] != SpaceChar)
            {
                return i - start;
            }
        }
        return count;
    }
    private static int CountSpacesRight(char[] dest,
                                        int start,
                                        int count)
    {
        for (var i = start + count; --i >= start;)
        {
            if (dest[i] != SpaceChar)
            {
                return start + count - 1 - i;
            }
        }
        return count;
    }
    /*
     * Name    : isTashkeelChar
     * Function: Returns true for Tashkeel characters else return false
     */
    private static bool IsTashkeelChar(char ch)
    {
        return ch >= '\u064B' && ch <= '\u0652';
    }
    /*
     *Name     : isSeenTailFamilyChar
     *Function : returns 1 if the character is a seen family isolated character
     *           in the FE range otherwise returns 0
     */
    private static int IsSeenTailFamilyChar(char ch)
    {
        if (ch >= 0xfeb1 && ch < 0xfebf)
        {
            return TailFamilyIsolatedFinal[ch - 0xFEB1];
        }
        return 0;
    }

    /* Name     : isSeenFamilyChar
     * Function : returns 1 if the character is a seen family character in the Unicode
     *            06 range otherwise returns 0
    */
    private static int IsSeenFamilyChar(char ch)
    {
        if (ch >= 0x633 && ch <= 0x636)
        {
            return 1;
        }
        return 0;
    }

    /*
     *Name     : isTailChar
     *Function : returns true if the character matches one of the tail characters
     *           (0xfe73 or 0x200b) otherwise returns false
     */
    private static bool IsTailChar(char ch)
    {
        if (ch == OldTailChar || ch == NewTailChar)
        {
            return true;
        }
        return false;
    }

    /*
     *Name     : isAlefMaksouraChar
     *Function : returns true if the character is a Alef Maksoura Final or isolated
     *           otherwise returns false
     */
    private static bool IsAlefMaksouraChar(char ch)
    {
        return (ch == 0xFEEF) || (ch == 0xFEF0) || (ch == 0x0649);
    }
    /*
     * Name     : isYehHamzaChar
     * Function : returns true if the character is a yehHamza isolated or yehhamza
     *            final is found otherwise returns false
     */
    private static bool IsYehHamzaChar(char ch)
    {
        if ((ch == 0xFE89) || (ch == 0xFE8A))
        {
            return true;
        }
        return false;
    }

    /*
     *Name     : isTashkeelCharFE
     *Function : Returns true for Tashkeel characters in FE range else return false
     */
    private static bool IsTashkeelCharFe(char ch)
    {
        return ch != 0xFE75 && ch >= 0xFE70 && ch <= 0xFE7F;
    }
    /*
     * Name: isTashkeelOnTatweelChar
     * Function: Checks if the Tashkeel Character is on Tatweel or not,if the
     *           Tashkeel on tatweel (FE range), it returns 1 else if the
     *           Tashkeel with shadda on tatweel (FC range)return 2 otherwise
     *           returns 0
     */
    private static int IsTashkeelOnTatweelChar(char ch)
    {
        if (ch >= 0xfe70 && ch <= 0xfe7f && ch != NewTailChar && ch != 0xFE75 && ch != ShaddaTatweelChar)
        {
            return TashkeelMedial[ch - 0xFE70];
        }
        if ((ch >= 0xfcf2 && ch <= 0xfcf4) || (ch == ShaddaTatweelChar))
        {
            return 2;
        }
        return 0;
    }

    /*
     * Name: isIsolatedTashkeelChar
     * Function: Checks if the Tashkeel Character is in the isolated form
     *           (i.e. Unicode FE range) returns 1 else if the Tashkeel
     *           with shadda is in the isolated form (i.e. Unicode FC range)
     *           returns 1 otherwise returns 0
     */
    private static int IsIsolatedTashkeelChar(char ch)
    {
        if (ch >= 0xfe70 && ch <= 0xfe7f && ch != NewTailChar && ch != 0xFE75)
        {
            return 1 - TashkeelMedial[ch - 0xFE70];
        }
        if (ch >= 0xfc5e && ch <= 0xfc63)
        {
            return 1;
        }
        return 0;
    }

    /*
     * Name    : isAlefChar
     * Function: Returns 1 for Alef characters else return 0
     */
    private static bool IsAlefChar(char ch)
    {
        return ch == '\u0622' || ch == '\u0623' || ch == '\u0625' || ch == '\u0627';
    }
    /*
     * Name    : isLamAlefChar
     * Function: Returns true for LamAlef characters else return false
     */
    private static bool IsLamAlefChar(char ch)
    {
        return ch >= '\uFEF5' && ch <= '\uFEFC';
    }
    private static bool IsNormalizedLamAlefChar(char ch)
    {
        return ch >= '\u065C' && ch <= '\u065F';
    }
    /*
     * Name    : calculateSize
     * Function: This function calculates the destSize to be used in preflighting
     *           when the destSize is equal to 0
     */
    private int CalculateSize(char[] source,
                              int sourceStart,
                              int sourceLength)
    {
        var destSize = sourceLength;
        switch (_options & ArabicShapingOptions.LettersMask)
        {
            case ArabicShapingOptions.LettersShape:
            case ArabicShapingOptions.LettersShapeTashkeelIsolated:
                if (_isLogical)
                {
                    for (int i = sourceStart, e = sourceStart + sourceLength - 1; i < e; ++i)
                    {
                        if ((source[i] == LamChar && IsAlefChar(source[i + 1])) || IsTashkeelCharFe(source[i]))
                        {
                            --destSize;
                        }
                    }
                }
                else
                { // visual
                    for (int i = sourceStart + 1, e = sourceStart + sourceLength; i < e; ++i)
                    {
                        if ((source[i] == LamChar && IsAlefChar(source[i - 1])) || IsTashkeelCharFe(source[i]))
                        {
                            --destSize;
                        }
                    }
                }
                break;
            case ArabicShapingOptions.LettersUnshape:
                for (int i = sourceStart, e = sourceStart + sourceLength; i < e; ++i)
                {
                    if (IsLamAlefChar(source[i]))
                    {
                        destSize++;
                    }
                }
                break;
        }
        return destSize;
    }
    /*
     * Name    : countSpaceSub
     * Function: Counts number of times the subChar appears in the array
     */
    public static int CountSpaceSub(char[] dest, int length, char subChar)
    {
        var i = 0;
        var count = 0;
        while (i < length)
        {
            if (dest[i] == subChar)
            {
                count++;
            }
            i++;
        }
        return count;
    }
    /*
     * Name    : shiftArray
     * Function: Shifts characters to replace space sub characters
     */
    public static void ShiftArray(char[] dest, int start, int e, char subChar)
    {
        var w = e;
        var r = e;
        while (--r >= start)
        {
            var ch = dest[r];
            if (ch != subChar)
            {
                --w;
                if (w != r)
                {
                    dest[w] = ch;
                }
            }
        }
    }
    /*
     * Name    : flipArray
     * Function: inverts array, so that start becomes end and vice versa
     */
    public static int FlipArray(char[] dest, int start, int e, int w)
    {
        if (w > start)
        {
            // shift, assume small buffer size so don't use arraycopy
            var r = w;
            w = start;
            while (r < e)
            {
                dest[w++] = dest[r++];
            }
        }
        else
        {
            w = e;
        }
        return w;
    }
    /*
     * Name     : handleTashkeelWithTatweel
     * Function : Replaces Tashkeel as following:
     *            Case 1 :if the Tashkeel on tatweel, replace it with Tatweel.
     *            Case 2 :if the Tashkeel aggregated with Shadda on Tatweel, replace
     *                   it with Shadda on Tatweel.
     *            Case 3: if the Tashkeel is isolated replace it with Space.
     *
     */
    private static int HandleTashkeelWithTatweel(char[] dest, int sourceLength)
    {
        int i;
        for (i = 0; i < sourceLength; i++)
        {
            if (IsTashkeelOnTatweelChar(dest[i]) == 1)
            {
                dest[i] = TatweelChar;
            }
            else if (IsTashkeelOnTatweelChar(dest[i]) == 2)
            {
                dest[i] = ShaddaTatweelChar;
            }
            else if ((IsIsolatedTashkeelChar(dest[i]) == 1) && dest[i] != ShaddaChar)
            {
                dest[i] = SpaceChar;
            }
        }
        return sourceLength;
    }
    /*
     *Name     : handleGeneratedSpaces
     *Function : The shapeUnicode function converts Lam + Alef into LamAlef + space,
     *           and Tashkeel to space.
     *           handleGeneratedSpaces function puts these generated spaces
     *           according to the options the user specifies. LamAlef and Tashkeel
     *           spaces can be replaced at begin, at end, at near or decrease the
     *           buffer size.
     *
     *           There is also Auto option for LamAlef and tashkeel, which will put
     *           the spaces at end of the buffer (or end of text if the user used
     *           the option SPACES_RELATIVE_TO_TEXT_BEGIN_END).
     *
     *           If the text type was visual_LTR and the option
     *           SPACES_RELATIVE_TO_TEXT_BEGIN_END was selected the END
     *           option will place the space at the beginning of the buffer and
     *           BEGIN will place the space at the end of the buffer.
     */
    private int HandleGeneratedSpaces(char[] dest,
              int start,
              int length)
    {
        var lenOptionsLamAlef = _options & ArabicShapingOptions.LamalefMask;
        var lenOptionsTashkeel = _options & ArabicShapingOptions.TashkeelMask;
        var lamAlefOn = false;
        var tashkeelOn = false;
        if (!_isLogical & !_spacesRelativeToTextBeginEnd)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (lenOptionsLamAlef)
            {
                case ArabicShapingOptions.LamalefBegin: lenOptionsLamAlef = ArabicShapingOptions.LamalefEnd; break;
                case ArabicShapingOptions.LamalefEnd: lenOptionsLamAlef = ArabicShapingOptions.LamalefBegin; break;
            }
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (lenOptionsTashkeel)
            {
                case ArabicShapingOptions.TashkeelBegin: lenOptionsTashkeel = ArabicShapingOptions.TashkeelEnd; break;
                case ArabicShapingOptions.TashkeelEnd: lenOptionsTashkeel = ArabicShapingOptions.TashkeelBegin; break;
            }
        }
        if (lenOptionsLamAlef == ArabicShapingOptions.LamalefNear)
        {
            for (int i = start, e = i + length; i < e; ++i)
            {
                if (dest[i] == LamalefSpaceSub)
                {
                    dest[i] = SpaceCharForLamalef;
                }
            }
        }
        else
        {
            var e = start + length;
            var wL = CountSpaceSub(dest, length, LamalefSpaceSub);
            var wT = CountSpaceSub(dest, length, TashkeelSpaceSub);
            if (lenOptionsLamAlef == ArabicShapingOptions.LamalefEnd)
            {
                lamAlefOn = true;
            }
            if (lenOptionsTashkeel == ArabicShapingOptions.TashkeelEnd)
            {
                tashkeelOn = true;
            }
            if (lamAlefOn && (lenOptionsLamAlef == ArabicShapingOptions.LamalefEnd))
            {
                ShiftArray(dest, start, e, LamalefSpaceSub);
                while (wL > start)
                {
                    dest[--wL] = SpaceChar;
                }
            }
            if (tashkeelOn && (lenOptionsTashkeel == ArabicShapingOptions.TashkeelEnd))
            {
                ShiftArray(dest, start, e, TashkeelSpaceSub);
                while (wT > start)
                {
                    dest[--wT] = SpaceChar;
                }
            }
            lamAlefOn = false;
            tashkeelOn = false;
            if (lenOptionsLamAlef == ArabicShapingOptions.LamalefResize)
            {
                lamAlefOn = true;
            }
            if (lenOptionsTashkeel == ArabicShapingOptions.TashkeelResize)
            {
                tashkeelOn = true;
            }
            if (lamAlefOn && (lenOptionsLamAlef == ArabicShapingOptions.LamalefResize))
            {
                ShiftArray(dest, start, e, LamalefSpaceSub);
                wL = FlipArray(dest, start, e, wL);
                length = wL - start;
            }
            if (tashkeelOn && (lenOptionsTashkeel == ArabicShapingOptions.TashkeelResize))
            {
                ShiftArray(dest, start, e, TashkeelSpaceSub);
                wT = FlipArray(dest, start, e, wT);
                length = wT - start;
            }
            lamAlefOn = false;
            tashkeelOn = false;
            if ((lenOptionsLamAlef == ArabicShapingOptions.LamalefBegin) ||
                (lenOptionsLamAlef == ArabicShapingOptions.LamalefAuto))
            {
                lamAlefOn = true;
            }
            if (lenOptionsTashkeel == ArabicShapingOptions.TashkeelBegin)
            {
                tashkeelOn = true;
            }
            if (lamAlefOn && ((lenOptionsLamAlef == ArabicShapingOptions.LamalefBegin) ||
                              (lenOptionsLamAlef == ArabicShapingOptions.LamalefAuto)))
            { // spaces at beginning
                ShiftArray(dest, start, e, LamalefSpaceSub);
                wL = FlipArray(dest, start, e, wL);
                while (wL < e)
                {
                    dest[wL++] = SpaceChar;
                }
            }
            if (tashkeelOn && (lenOptionsTashkeel == ArabicShapingOptions.TashkeelBegin))
            {
                ShiftArray(dest, start, e, TashkeelSpaceSub);
                wT = FlipArray(dest, start, e, wT);
                while (wT < e)
                {
                    dest[wT++] = SpaceChar;
                }
            }
        }
        return length;
    }
    /*
     *Name     :expandCompositCharAtBegin
     *Function :Expands the LamAlef character to Lam and Alef consuming the required
     *         space from beginning of the buffer. If the text type was visual_LTR
     *         and the option SPACES_RELATIVE_TO_TEXT_BEGIN_END was selected
     *         the spaces will be located at end of buffer.
     *         If there are no spaces to expand the LamAlef, an exception is thrown.
    */
    private bool ExpandCompositCharAtBegin(char[] dest, int start, int length,
                               int lacount)
    {
        if (lacount > CountSpacesRight(dest, start, length))
        {
            return true;
        }
        for (int r = start + length - lacount, w = start + length; --r >= start;)
        {
            var ch = dest[r];
            if (IsNormalizedLamAlefChar(ch))
            {
                dest[--w] = LamChar;
                dest[--w] = ConvertNormalizedLamAlef[ch - '\u065C'];
            }
            else
            {
                dest[--w] = ch;
            }
        }
        return false;
    }
    /*
     *Name     : expandCompositCharAtEnd
     *Function : Expands the LamAlef character to Lam and Alef consuming the
     *           required space from end of the buffer. If the text type was
     *           Visual LTR and the option SPACES_RELATIVE_TO_TEXT_BEGIN_END
     *           was used, the spaces will be consumed from begin of buffer. If
     *           there are no spaces to expand the LamAlef, an exception is thrown.
     */
    private static bool ExpandCompositCharAtEnd(char[] dest, int start, int length,
                            int lacount)
    {
        if (lacount > CountSpacesLeft(dest, start, length))
        {
            return true;
        }
        for (int r = start + lacount, w = start, e = start + length; r < e; ++r)
        {
            var ch = dest[r];
            if (IsNormalizedLamAlefChar(ch))
            {
                dest[w++] = ConvertNormalizedLamAlef[ch - '\u065C'];
                dest[w++] = LamChar;
            }
            else
            {
                dest[w++] = ch;
            }
        }
        return false;
    }
    /*
     *Name     : expandCompositCharAtNear
     *Function : Expands the LamAlef character into Lam + Alef, YehHamza character
     *           into Yeh + Hamza, SeenFamily character into SeenFamily character
     *           + Tail, while consuming the space next to the character.
     */
    private bool ExpandCompositCharAtNear(char[] dest, int start, int length,
                                         int yehHamzaOption, int seenTailOption, int lamAlefOption)
    {
        if (IsNormalizedLamAlefChar(dest[start]))
        {
            return true;
        }
        for (var i = start + length; --i >= start;)
        {
            var ch = dest[i];
            if (lamAlefOption == 1 && IsNormalizedLamAlefChar(ch))
            {
                if (i > start && dest[i - 1] == SpaceChar)
                {
                    dest[i] = LamChar;
                    dest[--i] = ConvertNormalizedLamAlef[ch - '\u065C'];
                }
                else
                {
                    return true;
                }
            }
            else if (seenTailOption == 1 && IsSeenTailFamilyChar(ch) == 1)
            {
                if (i > start && dest[i - 1] == SpaceChar)
                {
                    dest[i - 1] = _tailChar;
                }
                else
                {
                    return true;
                }
            }
            else if (yehHamzaOption == 1 && IsYehHamzaChar(ch))
            {
                if (i > start && dest[i - 1] == SpaceChar)
                {
                    dest[i] = YehHamzaToYeh[ch - YehHamzafeChar];
                    dest[i - 1] = HamzafeChar;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    /*
     * Name    : expandCompositChar
     * Function: LamAlef needs special handling as the LamAlef is
     *           one character while expanding it will give two
     *           characters Lam + Alef, so we need to expand the LamAlef
     *           in near or far spaces according to the options the user
     *           specifies or increase the buffer size.
     *           Dest has enough room for the expansion if we are growing.
     *           lamalef are normalized to the 'special characters'
     */
    private int ExpandCompositChar(char[] dest,
                              int start,
                              int length,
                              int lacount,
                              int shapingMode)
    {
        var lenOptionsLamAlef = _options & ArabicShapingOptions.LamalefMask;
        var lenOptionsSeen = _options & ArabicShapingOptions.SeenMask;
        var lenOptionsYehHamza = _options & ArabicShapingOptions.YehhamzaMask;
        bool spaceNotFound;
        if (!_isLogical && !_spacesRelativeToTextBeginEnd)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (lenOptionsLamAlef)
            {
                case ArabicShapingOptions.LamalefBegin: lenOptionsLamAlef = ArabicShapingOptions.LamalefEnd; break;
                case ArabicShapingOptions.LamalefEnd: lenOptionsLamAlef = ArabicShapingOptions.LamalefBegin; break;
            }
        }
        if (shapingMode == 1)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (lenOptionsLamAlef == ArabicShapingOptions.LamalefAuto)
            {
                if (_isLogical)
                {
                    spaceNotFound = ExpandCompositCharAtEnd(dest, start, length, lacount);
                    if (spaceNotFound)
                    {
                        spaceNotFound = ExpandCompositCharAtBegin(dest, start, length, lacount);
                    }
                    if (spaceNotFound)
                    {
                        spaceNotFound = ExpandCompositCharAtNear(dest, start, length, 0, 0, 1);
                    }
                    if (spaceNotFound)
                    {
                        throw new ArabicShapingException("No spacefor lamalef");
                    }
                }
                else
                {
                    spaceNotFound = ExpandCompositCharAtBegin(dest, start, length, lacount);
                    if (spaceNotFound)
                    {
                        spaceNotFound = ExpandCompositCharAtEnd(dest, start, length, lacount);
                    }
                    if (spaceNotFound)
                    {
                        spaceNotFound = ExpandCompositCharAtNear(dest, start, length, 0, 0, 1);
                    }
                    if (spaceNotFound)
                    {
                        throw new ArabicShapingException("No spacefor lamalef");
                    }
                }
            }
            else if (lenOptionsLamAlef == ArabicShapingOptions.LamalefEnd)
            {
                spaceNotFound = ExpandCompositCharAtEnd(dest, start, length, lacount);
                if (spaceNotFound)
                {
                    throw new ArabicShapingException("No spacefor lamalef");
                }
            }
            else if (lenOptionsLamAlef == ArabicShapingOptions.LamalefBegin)
            {
                spaceNotFound = ExpandCompositCharAtBegin(dest, start, length, lacount);
                if (spaceNotFound)
                {
                    throw new ArabicShapingException("No spacefor lamalef");
                }
            }
            else if (lenOptionsLamAlef == ArabicShapingOptions.LamalefNear)
            {
                spaceNotFound = ExpandCompositCharAtNear(dest, start, length, 0, 0, 1);
                if (spaceNotFound)
                {
                    throw new ArabicShapingException("No spacefor lamalef");
                }
            }
            else if (lenOptionsLamAlef == ArabicShapingOptions.LamalefResize)
            {
                for (int r = start + length, w = r + lacount; --r >= start;)
                {
                    var ch = dest[r];
                    if (IsNormalizedLamAlefChar(ch))
                    {
                        dest[--w] = '\u0644';
                        dest[--w] = ConvertNormalizedLamAlef[ch - '\u065C'];
                    }
                    else
                    {
                        dest[--w] = ch;
                    }
                }
                length += lacount;
            }
        }
        else
        {
            if (lenOptionsSeen == ArabicShapingOptions.SeenTwocellNear)
            {
                spaceNotFound = ExpandCompositCharAtNear(dest, start, length, 0, 1, 0);
                if (spaceNotFound)
                {
                    throw new ArabicShapingException("No space for Seen tail expansion");
                }
            }
            // ReSharper disable once InvertIf
            if (lenOptionsYehHamza == ArabicShapingOptions.YehhamzaTwocellNear)
            {
                spaceNotFound = ExpandCompositCharAtNear(dest, start, length, 1, 0, 0);
                if (spaceNotFound)
                {
                    throw new ArabicShapingException("No space for YehHamza expansion");
                }
            }
        }
        return length;
    }
    /* Convert the input buffer from FExx Range into 06xx Range
     * to put all characters into the 06xx range
     * even the lamalef is converted to the special region in
     * the 06xx range.  Return the number of lamalef chars found.
     */
    private static int Normalize(char[] dest, int start, int length)
    {
        var lacount = 0;
        for (int i = start, e = i + length; i < e; ++i)
        {
            var ch = dest[i];
            if (ch >= '\uFE70' && ch <= '\uFEFC')
            {
                if (IsLamAlefChar(ch))
                {
                    ++lacount;
                }
                dest[i] = (char)ConvertFEto06[ch - '\uFE70'];
            }
        }
        return lacount;
    }
    /*
     * Name    : deshapeNormalize
     * Function: Convert the input buffer from FExx Range into 06xx Range
     *           even the lamalef is converted to the special region in the 06xx range.
     *           According to the options the user enters, all seen family characters
     *           followed by a tail character are merged to seen tail family character and
     *           any yeh followed by a hamza character are merged to yehhamza character.
     *           Method returns the number of lamalef chars found.
     */
    private int DeshapeNormalize(char[] dest, int start, int length)
    {
        var lacount = 0;
        var yehHamzaComposeEnabled = (_options & ArabicShapingOptions.YehhamzaMask) == ArabicShapingOptions.YehhamzaTwocellNear ? 1 : 0;
        var seenComposeEnabled = (_options & ArabicShapingOptions.SeenMask) == ArabicShapingOptions.SeenTwocellNear ? 1 : 0;
        for (int i = start, e = i + length; i < e; ++i)
        {
            var ch = dest[i];
            if ((yehHamzaComposeEnabled == 1) && ((ch == Hamza06Char) || (ch == HamzafeChar))
                   && (i < length - 1) && IsAlefMaksouraChar(dest[i + 1]))
            {
                dest[i] = SpaceChar;
                dest[i + 1] = YehHamzaChar;
            }
            else if ((seenComposeEnabled == 1) && IsTailChar(ch) && (i < length - 1)
                          && (IsSeenTailFamilyChar(dest[i + 1]) == 1))
            {
                dest[i] = SpaceChar;
            }
            else if (ch >= '\uFE70' && ch <= '\uFEFC')
            {
                if (IsLamAlefChar(ch))
                {
                    ++lacount;
                }
                dest[i] = (char)ConvertFEto06[ch - '\uFE70'];
            }
        }
        return lacount;
    }
    /*
     * Name    : shapeUnicode
     * Function: Converts an Arabic Unicode buffer in 06xx Range into a shaped
     *           arabic Unicode buffer in FExx Range
     */
    private int ShapeUnicode(char[] dest,
                             int start,
                             int length,
                             int destSize,
                             int tashkeelFlag)
    {
        if (destSize < 0) throw new ArgumentOutOfRangeException(nameof(destSize));
        var lamalefCount = Normalize(dest, start, length);
        // resolve the link between the characters.
        // Arabic characters have four forms: Isolated, Initial, Medial and Final.
        // Tashkeel characters have two, isolated or medial, and sometimes only isolated.
        // tashkeelFlag == 0: shape normally, 1: shape isolated, 2: don't shape
        bool lamalefFound = false, seenfamFound = false;
        bool yehhamzaFound = false, tashkeelFound = false;
        var i = start + length - 1;
        var currLink = GetLink(dest[i]);
        var nextLink = 0;
        var prevLink = 0;
        var lastLink = 0;
        //int prevPos = i;
        var lastPos = i;
        var nx = -2;
        while (i >= 0)
        {
            // If high byte of currLink > 0 then there might be more than one shape
            if ((currLink & '\uFF00') > 0 || IsTashkeelChar(dest[i]))
            {
                var nw = i - 1;
                nx = -2;
                while (nx < 0)
                { // we need to know about next char
                    if (nw == -1)
                    {
                        nextLink = 0;
                        nx = int.MaxValue;
                    }
                    else
                    {
                        nextLink = GetLink(dest[nw]);
                        if ((nextLink & Irrelevant) == 0)
                        {
                            nx = nw;
                        }
                        else
                        {
                            --nw;
                        }
                    }
                }
                if (((currLink & Aleftype) > 0) && ((lastLink & Lamtype) > 0))
                {
                    lamalefFound = true;
                    var wLamalef = ChangeLamAlef(dest[i]); // get from 0x065C-0x065f
                    if (wLamalef != '\u0000')
                    {
                        // replace alef by marker, it will be removed later
                        dest[i] = '\uffff';
                        dest[lastPos] = wLamalef;
                        i = lastPos;
                    }
                    lastLink = prevLink;
                    currLink = GetLink(wLamalef); // requires '\u0000', unfortunately
                }
                if ((i > 0) && (dest[i - 1] == SpaceChar))
                {
                    if (IsSeenFamilyChar(dest[i]) == 1)
                    {
                        seenfamFound = true;
                    }
                    else if (dest[i] == YehHamzaChar)
                    {
                        yehhamzaFound = true;
                    }
                }
                else if (i == 0)
                {
                    if (IsSeenFamilyChar(dest[i]) == 1)
                    {
                        seenfamFound = true;
                    }
                    else if (dest[i] == YehHamzaChar)
                    {
                        yehhamzaFound = true;
                    }
                }
                // get the proper shape according to link ability of neighbors
                // and of character; depends on the order of the shapes
                // (isolated, initial, middle, final) in the compatibility area
                var flag = SpecialChar(dest[i]);
                var shape = ShapeTable[nextLink & LinkMask]
                    [lastLink & LinkMask]
                    [currLink & LinkMask];
                if (flag == 1)
                {
                    shape &= 0x1;
                }
                else if (flag == 2)
                {
                    if (tashkeelFlag == 0 &&
                        ((lastLink & Linkl) != 0) &&
                        ((nextLink & Linkr) != 0) &&
                        dest[i] != '\u064C' &&
                        dest[i] != '\u064D' &&
                        !((nextLink & Aleftype) == Aleftype &&
                          (lastLink & Lamtype) == Lamtype))
                    {
                        shape = 1;
                    }
                    else
                    {
                        shape = 0;
                    }
                }
                if (flag == 2)
                {
                    if (tashkeelFlag == 2)
                    {
                        dest[i] = TashkeelSpaceSub;
                        tashkeelFound = true;
                    }
                    else
                    {
                        dest[i] = (char)('\uFE70' + _irrelevantPos[dest[i] - '\u064B'] + shape);
                    }
                    // else leave tashkeel alone
                }
                else
                {
                    dest[i] = (char)('\uFE70' + (currLink >> 8) + shape);
                }
            }
            // move one notch forward
            if ((currLink & Irrelevant) == 0)
            {
                prevLink = lastLink;
                lastLink = currLink;
                //prevPos = lastPos;
                lastPos = i;
            }
            --i;
            if (i == nx)
            {
                currLink = nextLink;
                nx = -2;
            }
            else if (i != -1)
            {
                currLink = GetLink(dest[i]);
            }
        }
        // If we found a lam/alef pair in the buffer
        // call handleGeneratedSpaces to remove the spaces that were added
        destSize = length;
        if (lamalefFound || tashkeelFound)
        {
            destSize = HandleGeneratedSpaces(dest, start, length);
        }
        if (seenfamFound || yehhamzaFound)
        {
            destSize = ExpandCompositChar(dest, start, destSize, lamalefCount, ShapeMode);
        }
        return destSize;
    }
    /*
     * Name    : deShapeUnicode
     * Function: Converts an Arabic Unicode buffer in FExx Range into unshaped
     *           arabic Unicode buffer in 06xx Range
     */
    private int DeShapeUnicode(char[] dest,
                               int start,
                               int length,
                               int destSize)
    {
        if (destSize < 0) throw new ArgumentOutOfRangeException(nameof(destSize));
        var lamalefCount = DeshapeNormalize(dest, start, length);
        // If there was a lamalef in the buffer call expandLamAlef
        destSize = lamalefCount != 0 ? ExpandCompositChar(dest, start, length, lamalefCount, DeshapeMode) : length;
        return destSize;
    }
    private int InternalShape(char[] source,
                              int sourceStart,
                              int sourceLength,
                              char[] dest,
                              int destStart,
                              int destSize)
    {
        if (sourceLength == 0)
        {
            return 0;
        }
        if (destSize == 0)
        {
            if (((_options & ArabicShapingOptions.LettersMask) != ArabicShapingOptions.LettersNoop) &&
                ((_options & ArabicShapingOptions.LamalefMask) == ArabicShapingOptions.LamalefResize))
            {
                return CalculateSize(source, sourceStart, sourceLength);
            }
            return sourceLength; // by definition
        }
        // always use temp buffer
        var temp = new char[sourceLength * 2]; // all lamalefs requiring expansion
        Array.Copy(source, sourceStart, temp, 0, sourceLength);
        if (_isLogical)
        {
            InvertBuffer(temp, 0, sourceLength);
        }
        var outputSize = sourceLength;
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (_options & ArabicShapingOptions.LettersMask)
        {
            case ArabicShapingOptions.LettersShapeTashkeelIsolated:
                outputSize = ShapeUnicode(temp, 0, sourceLength, destSize, 1);
                break;
            case ArabicShapingOptions.LettersShape:
                if (((_options & ArabicShapingOptions.TashkeelMask) > 0) &&
                    ((_options & ArabicShapingOptions.TashkeelMask) != ArabicShapingOptions.TashkeelReplaceByTatweel))
                {
                    /* Call the shaping function with tashkeel flag == 2 for removal of tashkeel */
                    outputSize = ShapeUnicode(temp, 0, sourceLength, destSize, 2);
                }
                else
                {
                    //default Call the shaping function with tashkeel flag == 1 */
                    outputSize = ShapeUnicode(temp, 0, sourceLength, destSize, 0);
                    /*After shaping text check if user wants to remove tashkeel and replace it with tatweel*/
                    if ((_options & ArabicShapingOptions.TashkeelMask) == ArabicShapingOptions.TashkeelReplaceByTatweel)
                    {
                        outputSize = HandleTashkeelWithTatweel(temp, sourceLength);
                    }
                }
                break;
            case ArabicShapingOptions.LettersUnshape:
                outputSize = DeShapeUnicode(temp, 0, sourceLength, destSize);
                break;
        }
        if (outputSize > destSize)
        {
            throw new ArabicShapingException("not enough room for result data");
        }
        if ((_options & ArabicShapingOptions.DigitsMask) != ArabicShapingOptions.DigitsNoop)
        {
            var digitBase = '\u0030'; // European digits
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_options & ArabicShapingOptions.DigitTypeMask)
            {
                case ArabicShapingOptions.DigitTypeAN:
                    digitBase = '\u0660';  // Arabic-Indic digits
                    break;
                case ArabicShapingOptions.DigitTypeANExtended:
                    digitBase = '\u06f0';  // Eastern Arabic-Indic digits (Persian and Urdu)
                    break;
            }
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_options & ArabicShapingOptions.DigitsMask)
            {
                case ArabicShapingOptions.DigitsEN2AN:
                    {
                        var digitDelta = digitBase - '\u0030';
                        for (var i = 0; i < outputSize; ++i)
                        {
                            var ch = temp[i];
                            if (ch <= '\u0039' && ch >= '\u0030')
                            {
                                temp[i] = (char)(temp[i] + digitDelta);
                            }
                        }
                    }
                    break;
                case ArabicShapingOptions.DigitsAN2EN:
                    {
                        var digitTop = (char)(digitBase + 9);
                        var digitDelta = '\u0030' - digitBase;
                        for (var i = 0; i < outputSize; ++i)
                        {
                            var ch = temp[i];
                            if (ch <= digitTop && ch >= digitBase)
                            {
                                temp[i] = (char)(temp[i] + digitDelta);
                            }
                        }
                    }
                    break;
                case ArabicShapingOptions.DigitsEN2ANInitLr:
                    ShapeToArabicDigitsWithContext(temp, 0, outputSize, digitBase, false);
                    break;
                case ArabicShapingOptions.DigitsEN2ANInitAL:
                    ShapeToArabicDigitsWithContext(temp, 0, outputSize, digitBase, true);
                    break;
            }
        }
        if (_isLogical)
        {
            InvertBuffer(temp, 0, outputSize);
        }
        Array.Copy(temp, 0, dest, destStart, outputSize);
        return outputSize;
    }
    public class ArabicShapingException : Exception
    {
        public ArabicShapingException(string msg) : base(msg)
        {

        }
    }
}
