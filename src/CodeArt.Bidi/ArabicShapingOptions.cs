using System;

namespace CodeArt.Bidi
{
    /// <summary>
    /// Arabic shaping options
    /// </summary>
    [Flags]
    public enum ArabicShapingOptions
    {
        /* Seen Tail options */
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping mode: The SEEN family character will expand into two characters using space near
         *               the SEEN family character(i.e. the space after the character).
         *               if there are no spaces found, ArabicShapingException will be thrown
         *
         * De-shaping mode: Any Seen character followed by Tail character will be
         *                  replaced by one cell Seen and a space will replace the Tail.
         * Affects: Seen options
         */
        SeenTwocellNear = 0x200000,
        /** Bit mask for Seen memory options. */
        SeenMask = 0x700000,
        /* YehHamza options */
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping mode: The YEHHAMZA character will expand into two characters using space near it
         *              (i.e. the space after the character)
         *               if there are no spaces found, ArabicShapingException will be thrown
         *
         * De-shaping mode: Any Yeh (final or isolated) character followed by Hamza character will be
         *                  replaced by one cell YehHamza and space will replace the Hamza.
         * Affects: YehHamza options
         */
        YehhamzaTwocellNear = 0x1000000,
        /** Bit mask for YehHamza memory options. */
        YehhamzaMask = 0x3800000,
        /* New Tashkeel options */
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping mode: Tashkeel characters will be replaced by spaces.
         *               Spaces will be placed at beginning of the buffer
         *
         * De-shaping mode: N/A
         * Affects: Tashkeel options
         */
        TashkeelBegin = 0x40000,
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping mode: Tashkeel characters will be replaced by spaces.
         *               Spaces will be placed at end of the buffer
         *
         * De-shaping mode: N/A
         * Affects: Tashkeel options
         */
        TashkeelEnd = 0x60000,
        /**
         * Memory option: allow the result to have a different length than the source.
         * Shaping mode: Tashkeel characters will be removed, buffer length will shrink.
         * De-shaping mode: N/A
         *
         * Affects: Tashkeel options
         */
        TashkeelResize = 0x80000,
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping mode: Tashkeel characters will be replaced by Tatweel if it is connected to adjacent
         *               characters (i.e. shaped on Tatweel) or replaced by space if it is not connected.
         *
         * De-shaping mode: N/A
         * Affects: YehHamza options
         */
        TashkeelReplaceByTatweel = 0xC0000,
        /** Bit mask for Tashkeel replacement with Space or Tatweel memory options. */
        TashkeelMask = 0xE0000,
        /* Space location Control options */
        /**
         * This option effects the meaning of BEGIN and END options. if this option is not used the default
         * for BEGIN and END will be as following:
         * The Default (for both Visual LTR, Visual RTL and Logical Text)
         *           1. BEGIN always refers to the start address of physical memory.
         *           2. END always refers to the end address of physical memory.
         *
         * If this option is used it will swap the meaning of BEGIN and END only for Visual LTR text.
         *
         * The affect on BEGIN and END Memory Options will be as following:
         *    A. BEGIN For Visual LTR text: This will be the beginning (right side) of the visual text
         *       (corresponding to the physical memory address end, same as END in default behavior)
         *    B. BEGIN For Logical text: Same as BEGIN in default behavior.
         *    C. END For Visual LTR text: This will be the end (left side) of the visual text. (corresponding to
         *      the physical memory address beginning, same as BEGIN in default behavior)
         *    D. END For Logical text: Same as END in default behavior.
         * Affects: All LamAlef BEGIN, END and AUTO options.
         */
        SpacesRelativeToTextBeginEnd = 0x4000000,
        /** Bit mask for swapping BEGIN and END for Visual LTR text */
        SpacesRelativeToTextMask = 0x4000000,
        /**
         * If this option is used, shaping will use the new Unicode code point for TAIL (i.e. 0xFE73).
         * If this option is not specified (Default), old unofficial Unicode TAIL code point is used (i.e. 0x200B)
         * De-shaping will not use this option as it will always search for both the new Unicode code point for the
         * TAIL (i.e. 0xFE73) or the old unofficial Unicode TAIL code point (i.e. 0x200B) and de-shape the
         * Seen-Family letter accordingly.
         *
         * Shaping Mode: Only shaping.
         * De-shaping Mode: N/A.
         * Affects: All Seen options
         */
        ShapeTailNewUnicode = 0x8000000,
        /** Bit mask for new Unicode Tail option */
        ShapeTailTypeMask = 0x8000000,
        /**
         * Memory option: allow the result to have a different length than the source.
         * @stable ICU 2.0
         */
        LengthGrowShrink = 0,
        /**
         * Memory option: allow the result to have a different length than the source.
         * Affects: LamAlef options
         * This option is an alias to LENGTH_GROW_SHRINK
         */
        LamalefResize = 0,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces next to modified characters.
         * @stable ICU 2.0
         */
        LengthFixedSpacesNear = 1,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces next to modified characters.
         * Affects: LamAlef options
         * This option is an alias to LENGTH_FIXED_SPACES_NEAR
         */
        LamalefNear = 1,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces at the end of the text.
         * @stable ICU 2.0
         */
        LengthFixedSpacesAtEnd = 2,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces at the end of the text.
         * Affects: LamAlef options
         * This option is an alias to LENGTH_FIXED_SPACES_AT_END
         */
        LamalefEnd = 2,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces at the beginning of the text.
         * @stable ICU 2.0
         */
        LengthFixedSpacesAtBeginning = 3,
        /**
         * Memory option: the result must have the same length as the source.
         * If more room is necessary, then try to consume spaces at the beginning of the text.
         * Affects: LamAlef options
         * This option is an alias to LENGTH_FIXED_SPACES_AT_BEGINNING
         */
        LamalefBegin = 3,
        /**
         * Memory option: the result must have the same length as the source.
         * Shaping Mode: For each LAMALEF character found, expand LAMALEF using space at end.
         *               If there is no space at end, use spaces at beginning of the buffer. If there
         *               is no space at beginning of the buffer, use spaces at the near (i.e. the space
         *               after the LAMALEF character).
         *
         * Deshaping Mode: Perform the same function as the flag equals LAMALEF_END.
         * Affects: LamAlef options
         */
        LamalefAuto = 0x10000,
        /**
         * Bit mask for memory options.
         * @stable ICU 2.0
         */
        LengthMask = 0x10003,
        /** Bit mask for LamAlef memory options. */
        LamalefMask = 0x10003,
        /**
         * Direction indicator: the source is in logical (keyboard) order.
         * @stable ICU 2.0
         */
        TextDirectionLogical = 0,
        /**
         * Direction indicator:the source is in visual RTL order,
         * the rightmost displayed character stored first.
         * This option is an alias to U_SHAPE_TEXT_DIRECTION_LOGICAL
         */
        TextDirectionVisualRtl = 0,
        /**
         * Direction indicator: the source is in visual (display) order, that is,
         * the leftmost displayed character is stored first.
         * @stable ICU 2.0
         */
        TextDirectionVisualLtr = 4,
        /**
         * Bit mask for direction indicators.
         * @stable ICU 2.0
         */
        TextDirectionMask = 4,
        /**
         * Letter shaping option: do not perform letter shaping.
         * @stable ICU 2.0
         */
        LettersNoop = 0,
        /**
         * Letter shaping option: replace normative letter characters in the U+0600 (Arabic) block,
         * by shaped ones in the U+FE70 (Presentation Forms B) block. Performs Lam-Alef ligature
         * substitution.
         * @stable ICU 2.0
         */
        LettersShape = 8,
        /**
         * Letter shaping option: replace shaped letter characters in the U+FE70 (Presentation Forms B) block
         * by normative ones in the U+0600 (Arabic) block.  Converts Lam-Alef ligatures to pairs of Lam and
         * Alef characters, consuming spaces if required.
         * @stable ICU 2.0
         */
        LettersUnshape = 0x10,
        /**
         * Letter shaping option: replace normative letter characters in the U+0600 (Arabic) block,
         * except for the TASHKEEL characters at U+064B...U+0652, by shaped ones in the U+Fe70
         * (Presentation Forms B) block.  The TASHKEEL characters will always be converted to
         * the isolated forms rather than to their correct shape.
         * @stable ICU 2.0
         */
        LettersShapeTashkeelIsolated = 0x18,
        /**
         * Bit mask for letter shaping options.
         * @stable ICU 2.0
         */
        LettersMask = 0x18,
        /**
         * Digit shaping option: do not perform digit shaping.
         * @stable ICU 2.0
         */
        DigitsNoop = 0,
        /**
         * Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
         * @stable ICU 2.0
         */
        DigitsEN2AN = 0x20,
        /**
         * Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
         * @stable ICU 2.0
         */
        DigitsAN2EN = 0x40,
        /**
         * Digit shaping option:
         * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
         * if the most recent strongly directional character
         * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
         * The initial state at the start of the text is assumed to be not an Arabic,
         * letter, so European digits at the start of the text will not change.
         * Compare to DIGITS_ALEN2AN_INIT_AL.
         * @stable ICU 2.0
         */
        DigitsEN2ANInitLr = 0x60,
        /**
         * Digit shaping option:
         * Replace European digits (U+0030...U+0039) by Arabic-Indic digits
         * if the most recent strongly directional character
         * is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
         * The initial state at the start of the text is assumed to be an Arabic,
         * letter, so European digits at the start of the text will change.
         * Compare to DIGITS_ALEN2AN_INT_LR.
         * @stable ICU 2.0
         */
        DigitsEN2ANInitAL = 0x80,
        /** Not a valid option value. */
        //private const int DIGITS_RESERVED = 0xa0,
        /**
         * Bit mask for digit shaping options.
         * @stable ICU 2.0
         */
        DigitsMask = 0xe0,
        /**
         * Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
         * @stable ICU 2.0
         */
        DigitTypeAN = 0,
        /**
         * Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
         * @stable ICU 2.0
         */
        DigitTypeANExtended = 0x100,
        /**
         * Bit mask for digit type options.
         * @stable ICU 2.0
         */
        DigitTypeMask = 0x0100, // 0x3f00?
        /**
                                                  * some constants
                                                  */
    }
}
