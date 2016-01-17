// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;

namespace CodeArt.Bidi
{
    /// <summary>
    /// Arabic shaping options
    /// </summary>
    [Flags]
    public enum ArabicShapingOptions
    {
        // Seen Tail options 
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping mode: The SEEN family character will expand into two characters using space near
        ///               the SEEN family character(i.e. the space after the character).
        ///               if there are no spaces found, ArabicShapingException will be thrown
        /// De-shaping mode: Any Seen character followed by Tail character will be
        ///                  replaced by one cell Seen and a space will replace the Tail.
        /// Affects: Seen options
        /// </summary>
        SeenTwocellNear = 0x200000,
        /// <summary> Bit mask for Seen memory options. </summary>
        SeenMask = 0x700000,
        // YehHamza options 
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping mode: The YEHHAMZA character will expand into two characters using space near it
        ///              (i.e. the space after the character)
        ///               if there are no spaces found, ArabicShapingException will be thrown
        // De-shaping mode: Any Yeh (final or isolated) character followed by Hamza character will be
        ///                  replaced by one cell YehHamza and space will replace the Hamza.
        /// Affects: YehHamza options
        /// </summary>
        YehhamzaTwocellNear = 0x1000000,
        /// <summary> Bit mask for YehHamza memory options. /// </summary>
        YehhamzaMask = 0x3800000,
        // New Tashkeel options 
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping mode: Tashkeel characters will be replaced by spaces.
        ///               Spaces will be placed at beginning of the buffer
        /// De-shaping mode: N/A
        /// Affects: Tashkeel options
        /// </summary>
        TashkeelBegin = 0x40000,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping mode: Tashkeel characters will be replaced by spaces.
        ///               Spaces will be placed at end of the buffer
        /// De-shaping mode: N/A
        /// Affects: Tashkeel options
        /// </summary>
        TashkeelEnd = 0x60000,
        /// <summary>
        /// Memory option: allow the result to have a different length than the source.
        /// Shaping mode: Tashkeel characters will be removed, buffer length will shrink.
        /// De-shaping mode: N/A
        /// Affects: Tashkeel options
        /// </summary>
        TashkeelResize = 0x80000,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping mode: Tashkeel characters will be replaced by Tatweel if it is connected to adjacent
        ///               characters (i.e. shaped on Tatweel) or replaced by space if it is not connected.
        /// De-shaping mode: N/A
        /// Affects: YehHamza options
        /// </summary>
        TashkeelReplaceByTatweel = 0xC0000,
        /// <summary> Bit mask for Tashkeel replacement with Space or Tatweel memory options. /// </summary>
        TashkeelMask = 0xE0000,
        // Space location Control options 
        /// <summary>
        /// This option effects the meaning of BEGIN and END options. if this option is not used the default
        /// for BEGIN and END will be as following:
        /// The Default (for both Visual LTR, Visual RTL and Logical Text)
        ///           1. BEGIN always refers to the start address of physical memory.
        ///           2. END always refers to the end address of physical memory.
        /// If this option is used it will swap the meaning of BEGIN and END only for Visual LTR text.
        /// The affect on BEGIN and END Memory Options will be as following:
        ///    A. BEGIN For Visual LTR text: This will be the beginning (right side) of the visual text
        ///       (corresponding to the physical memory address end, same as END in default behavior)
        ///    B. BEGIN For Logical text: Same as BEGIN in default behavior.
        ///    C. END For Visual LTR text: This will be the end (left side) of the visual text. (corresponding to
        ///      the physical memory address beginning, same as BEGIN in default behavior)
        ///    D. END For Logical text: Same as END in default behavior.
        /// Affects: All LamAlef BEGIN, END and AUTO options.
        /// </summary>
        SpacesRelativeToTextBeginEnd = 0x4000000,
        /// <summary> Bit mask for swapping BEGIN and END for Visual LTR text /// </summary>
        SpacesRelativeToTextMask = 0x4000000,
        /// <summary>
        /// If this option is used, shaping will use the new Unicode code point for TAIL (i.e. 0xFE73).
        /// If this option is not specified (Default), old unofficial Unicode TAIL code point is used (i.e. 0x200B)
        /// De-shaping will not use this option as it will always search for both the new Unicode code point for the
        /// TAIL (i.e. 0xFE73) or the old unofficial Unicode TAIL code point (i.e. 0x200B) and de-shape the
        /// Seen-Family letter accordingly.
        /// Shaping Mode: Only shaping.
        /// De-shaping Mode: N/A.
        /// Affects: All Seen options
        /// </summary>
        ShapeTailNewUnicode = 0x8000000,
        /// <summary> Bit mask for new Unicode Tail option /// </summary>
        ShapeTailTypeMask = 0x8000000,
        /// <summary>
        /// Memory option: allow the result to have a different length than the source.
        /// @stable ICU 2.0
        /// </summary>
        LengthGrowShrink = 0,
        /// <summary>
        /// Memory option: allow the result to have a different length than the source.
        /// Affects: LamAlef options
        /// This option is an alias to LENGTH_GROW_SHRINK
        /// </summary>
        LamalefResize = 0,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces next to modified characters.
        /// @stable ICU 2.0
        /// </summary>
        LengthFixedSpacesNear = 1,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces next to modified characters.
        /// Affects: LamAlef options
        /// This option is an alias to LENGTH_FIXED_SPACES_NEAR
        /// </summary>
        LamalefNear = 1,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces at the end of the text.
        /// @stable ICU 2.0
        /// </summary>
        LengthFixedSpacesAtEnd = 2,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces at the end of the text.
        /// Affects: LamAlef options
        /// This option is an alias to LENGTH_FIXED_SPACES_AT_END
        /// </summary>
        LamalefEnd = 2,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces at the beginning of the text.
        /// @stable ICU 2.0
        /// </summary>
        LengthFixedSpacesAtBeginning = 3,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// If more room is necessary, then try to consume spaces at the beginning of the text.
        /// Affects: LamAlef options
        /// This option is an alias to LENGTH_FIXED_SPACES_AT_BEGINNING
        /// </summary>
        LamalefBegin = 3,
        /// <summary>
        /// Memory option: the result must have the same length as the source.
        /// Shaping Mode: For each LAMALEF character found, expand LAMALEF using space at end.
        ///               If there is no space at end, use spaces at beginning of the buffer. If there
        ///               is no space at beginning of the buffer, use spaces at the near (i.e. the space
        ///               after the LAMALEF character).
        /// Deshaping Mode: Perform the same function as the flag equals LAMALEF_END.
        /// Affects: LamAlef options
        /// </summary>
        LamalefAuto = 0x10000,
        /// <summary>
        /// Bit mask for memory options.
        /// @stable ICU 2.0
        /// </summary>
        LengthMask = 0x10003,
        /// <summary> Bit mask for LamAlef memory options. /// </summary>
        LamalefMask = 0x10003,
        /// <summary>
        /// Direction indicator: the source is in logical (keyboard) order.
        /// @stable ICU 2.0
        /// </summary>
        TextDirectionLogical = 0,
        /// <summary>
        /// Direction indicator:the source is in visual RTL order,
        /// the rightmost displayed character stored first.
        /// This option is an alias to U_SHAPE_TEXT_DIRECTION_LOGICAL
        /// </summary>
        TextDirectionVisualRtl = 0,
        /// <summary>
        /// Direction indicator: the source is in visual (display) order, that is,
        /// the leftmost displayed character is stored first.
        /// @stable ICU 2.0
        /// </summary>
        TextDirectionVisualLtr = 4,
        /// <summary>
        /// Bit mask for direction indicators.
        /// @stable ICU 2.0
        /// </summary>
        TextDirectionMask = 4,
        /// <summary>
        /// Letter shaping option: do not perform letter shaping.
        /// @stable ICU 2.0
        /// </summary>
        LettersNoop = 0,
        /// <summary>
        /// Letter shaping option: replace normative letter characters in the U+0600 (Arabic) block,
        /// by shaped ones in the U+FE70 (Presentation Forms B) block. Performs Lam-Alef ligature
        /// substitution.
        /// @stable ICU 2.0
        /// </summary>
        LettersShape = 8,
        /// <summary>
        /// Letter shaping option: replace shaped letter characters in the U+FE70 (Presentation Forms B) block
        /// by normative ones in the U+0600 (Arabic) block.  Converts Lam-Alef ligatures to pairs of Lam and
        /// Alef characters, consuming spaces if required.
        /// @stable ICU 2.0
        /// </summary>
        LettersUnshape = 0x10,
        /// <summary>
        /// Letter shaping option: replace normative letter characters in the U+0600 (Arabic) block,
        /// except for the TASHKEEL characters at U+064B...U+0652, by shaped ones in the U+Fe70
        /// (Presentation Forms B) block.  The TASHKEEL characters will always be converted to
        /// the isolated forms rather than to their correct shape.
        /// @stable ICU 2.0
        /// </summary>
        LettersShapeTashkeelIsolated = 0x18,
        /// <summary>
        /// Bit mask for letter shaping options.
        /// @stable ICU 2.0
        /// </summary>
        LettersMask = 0x18,
        /// <summary>
        /// Digit shaping option: do not perform digit shaping.
        /// @stable ICU 2.0
        /// </summary>
        DigitsNoop = 0,
        /// <summary>
        /// Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
        /// @stable ICU 2.0
        /// </summary>
        DigitsEN2AN = 0x20,
        /// <summary>
        /// Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
        /// @stable ICU 2.0
        /// </summary>
        DigitsAN2EN = 0x40,
        /// <summary>
        /// Digit shaping option:
        /// Replace European digits (U+0030...U+0039) by Arabic-Indic digits
        /// if the most recent strongly directional character
        /// is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
        /// The initial state at the start of the text is assumed to be not an Arabic,
        /// letter, so European digits at the start of the text will not change.
        /// Compare to DIGITS_ALEN2AN_INIT_AL.
        /// @stable ICU 2.0
        /// </summary>
        DigitsEN2ANInitLr = 0x60,
        /// <summary>
        /// Digit shaping option:
        /// Replace European digits (U+0030...U+0039) by Arabic-Indic digits
        /// if the most recent strongly directional character
        /// is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
        /// The initial state at the start of the text is assumed to be an Arabic,
        /// letter, so European digits at the start of the text will change.
        /// Compare to DIGITS_ALEN2AN_INT_LR.
        /// @stable ICU 2.0
        /// </summary>
        DigitsEN2ANInitAL = 0x80,
        /// <summary> Not a valid option value. /// </summary>
        //private const int DIGITS_RESERVED = 0xa0,
        /// <summary>
        /// Bit mask for digit shaping options.
        /// @stable ICU 2.0
        /// </summary>
        DigitsMask = 0xe0,
        /// <summary>
        /// Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
        /// @stable ICU 2.0
        /// </summary>
        DigitTypeAN = 0,
        /// <summary>
        /// Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
        /// @stable ICU 2.0
        /// </summary>
        DigitTypeANExtended = 0x100,
        /// <summary>
        /// Bit mask for digit type options.
        /// @stable ICU 2.0
        /// </summary>
        DigitTypeMask = 0x0100, // 0x3f00?
    }
}
