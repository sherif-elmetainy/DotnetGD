// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

/*
* Bidi reference implementation from http://www.unicode.org/Public/PROGRAMS/BidiReferenceJava/
* Original Code ported from java to C# by Sherif Elmetainy
* The credits and copyright info for the original code listed below
*/
/*
* Last Revised: 2013-09-02
*
* Credits:
* Originally written by Doug Felt
* 
* Updated for Unicode 6.3 by Roozbeh Pournader, with feedback by Aharon Lanin
* 
* Updated by Asmus Freytag to implement the Paired Bracket Algorithm (PBA)
*
* Disclaimer and legal rights:
* (C) Copyright IBM Corp. 1999, All Rights Reserved
* (C) Copyright Google Inc. 2013, All Rights Reserved
* (C) Copyright ASMUS, Inc. 2013. All Rights Reserved
*
* Distributed under the Terms of Use in http://www.unicode.org/copyright.html.
*/


/*
 * Revision info (2013-09-16)
 * Changed MAX_DEPTH to 125
 * 
 * Revision info (2013-06-02):
 * <p>
 * The core part of the Unicode Paired Bracket Algorithm (PBA) 
 * is implemented in a new BidiPBAReference class.
 * <p>
 * Changed convention for default paragraph embedding level from -1 to 2.
 */
using System;
using System.Linq;
using static CodeArt.Bidi.BidiDirection;

namespace CodeArt.Bidi
{
    


    /**
     * Reference implementation of the Unicode Bidirectional Algorithm (UAX #9).
     *
     * <p>
     * This implementation is not optimized for performance. It is intended as a
     * reference implementation that closely follows the specification of the
     * Bidirectional Algorithm in The Unicode Standard version 6.3.
     * <p>
     * <b>Input:</b><br>
     * There are two levels of input to the algorithm, since clients may prefer to
     * supply some information from out-of-band sources rather than relying on the
     * default behavior.
     * <ol>
     * <li>Bidi class array
     * <li>Bidi class array, with externally supplied base line direction
     * </ol>
     * <p>
     * <b>Output:</b><br>
     * Output is separated into several stages as well, to better enable clients to
     * evaluate various aspects of implementation conformance.
     * <ol>
     * <li>levels array over entire paragraph
     * <li>reordering array over entire paragraph
     * <li>levels array over line
     * <li>reordering array over line
     * </ol>
     * Note that for conformance to the Unicode Bidirectional Algorithm,
     * implementations are only required to generate correct reordering and
     * character directionality (odd or even levels) over a line. Generating
     * identical level arrays over a line is not required. Bidi explicit format
     * codes (LRE, RLE, LRO, RLO, PDF) and BN can be assigned arbitrary levels and
     * positions as long as the rest of the input is properly reordered.
     * <p>
     * As the algorithm is defined to operate on a single paragraph at a time, this
     * implementation is written to handle single paragraphs. Thus rule P1 is
     * presumed by this implementation-- the data provided to the implementation is
     * assumed to be a single paragraph, and either contains no 'B' codes, or a
     * single 'B' code at the end of the input. 'B' is allowed as input to
     * illustrate how the algorithm assigns it a level.
     * <p>
     * Also note that rules L3 and L4 depend on the rendering engine that uses the
     * result of the bidi algorithm. This implementation assumes that the rendering
     * engine expects combining marks in visual order (e.g. to the left of their
     * base character in RTL runs) and that it adjusts the glyphs used to render
     * mirrored characters that are in RTL runs so that they render appropriately.
     *
     * @author Doug Felt
     * @author Roozbeh Pournader
     * @author Asmus Freytag
     */

    public class BidiReference
    {
        private readonly BidiDirection[] _initialTypes;
        private ParagraphDirection _paragraphEmbeddingLevel = ParagraphDirection.Default;

        private int _textLength; // for convenience
        private BidiDirection[] _resultTypes; // for paragraph, not lines
        private sbyte[] _resultLevels; // for paragraph, not lines

        public BidiDirection[] GetResultTypes() => _resultTypes.ToArray(); // for display in test mode

        /*
         * Index of matching PDI for isolate initiator characters. For other
         * characters, the value of _matchingPDI will be set to -1. For isolate
         * initiators with no matching PDI, _matchingPDI will be set to the length of
         * the input string.
         */
        private int[] _matchingPDI;

        /*
         * Index of matching isolate initiator for PDI characters. For other
         * characters, and for PDIs with no matching isolate initiator, the value of
         * _matchingIsolateInitiator will be set to -1.
         */
        private int[] _matchingIsolateInitiator;

        /*
         * Arrays of properties needed for paired bracket evaluation in N0
         */
        private readonly BracketType[] _pairTypes; // paired Bracket types for paragraph
        private readonly int[] _pairValues; // paired Bracket values for paragraph

        public BidiPBAReference PBA { get; set; }

        //
        // Input
        //

        /**
         * Initialize using several arrays, then run the algorithm
         * @param types
         *            Array of types ranging from TypeMin to TypeMax inclusive 
         *            and representing the direction codes of the characters in the text.
         * @param _pairTypes
         * 			  Array of paired bracket types ranging from 0 (none) to 2 (closing)
         * 			  of the characters
         * @param _pairValues
         * 			  Array identifying which set of matching bracket characters
         * 			  as defined in BidiPBAReference (note, both opening and closing
         * 			  bracket get the same value if they are part of the same canonical "set"
         * 			  or pair)
         */
        public BidiReference(BidiDirection[] types, BracketType[] pairTypes, int[] pairValues)
        {
            ValidateTypes(types);
            ValidatePbTypes(pairTypes);
            ValidatePbValues(pairValues, pairTypes);

            _initialTypes = types.ToArray(); // client type array remains unchanged
            _pairTypes = pairTypes;
            _pairValues = pairValues;

            RunAlgorithm();
        }

        /**
         * Initialize using several arrays of direction and other types and an externally supplied
         * paragraph embedding level. The embedding level may be  0, 1 or 2.
         * <p>
         * 2 means to apply the default algorithm (rules P2 and P3), 0 is for LTR
         * paragraphs, and 1 is for RTL paragraphs.
         *
         * @param types
         *            the types array
         * @param _pairTypes
         *           the paired bracket types array
         * @param _pairValues
         * 			 the paired bracket values array
         * @param _paragraphEmbeddingLevel
         *            the externally supplied paragraph embedding level.
         */
        public BidiReference(BidiDirection[] types, BracketType[] pairTypes, int[] pairValues, ParagraphDirection paragraphEmbeddingLevel)
        {
            ValidateTypes(types);
            ValidatePbTypes(pairTypes);
            ValidatePbValues(pairValues, pairTypes);
            ValidateParagraphEmbeddingLevel(paragraphEmbeddingLevel);

            _initialTypes = types.ToArray(); // client type array remains unchanged
            _paragraphEmbeddingLevel = paragraphEmbeddingLevel;
            _pairTypes = pairTypes;
            _pairValues = pairValues;

            RunAlgorithm();
        }

        

        /**
         * The algorithm. Does not include line-based processing (Rules L1, L2).
         * These are applied later in the line-based phase of the algorithm.
         */
        private void RunAlgorithm()
        {
            _textLength = _initialTypes.Length;

            // Initialize output types.
            // Result types initialized to input types.
            _resultTypes = _initialTypes.ToArray();

            // Preprocessing to find the matching isolates
            DetermineMatchingIsolates();

            // 1) determining the paragraph level
            // Rule P1 is the requirement for entering this algorithm.
            // Rules P2, P3.
            // If no externally supplied paragraph embedding level, use default.
            if (_paragraphEmbeddingLevel == ParagraphDirection.Default)
            {
                _paragraphEmbeddingLevel = DetermineParagraphEmbeddingLevel(0, _textLength);
            }

            // Initialize result levels to paragraph embedding level.
            _resultLevels = new sbyte[_textLength];
            SetLevels(_resultLevels, 0, _textLength, (sbyte)_paragraphEmbeddingLevel);

            // 2) Explicit levels and directions
            // Rules X1-X8.
            DetermineExplicitEmbeddingLevels();

            // Rule X9.
            // We do not remove the embeddings, the overrides, the PDFs, and the BNs
            // from the string explicitly. But they are not copied into isolating run
            // sequences when they are created, so they are removed for all
            // practical purposes.

            // Rule X10.
            // Run remainder of algorithm one isolating run sequence at a time
            var sequences = DetermineIsolatingRunSequences();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < sequences.Length; ++i)
            {
                var sequence = sequences[i];
                // 3) resolving weak types
                // Rules W1-W7.
                sequence.ResolveWeakTypes();

                // 4a) resolving paired brackets
                // Rule N0
                sequence.ResolvePairedBrackets();

                // 4b) resolving neutral types
                // Rules N1-N3.
                sequence.ResolveNeutralTypes();

                // 5) resolving implicit embedding levels
                // Rules I1, I2.
                sequence.ResolveImplicitLevels();

                // Apply the computed levels and types
                sequence.ApplyLevelsAndTypes();
            }

            // Assign appropriate levels to 'hide' LREs, RLEs, LROs, RLOs, PDFs, and
            // BNs. This is for convenience, so the resulting level array will have
            // a value for every character.
            AssignLevelsToCharactersRemovedByX9();
        }

        /**
         * Determine the matching PDI for each isolate initiator and vice versa.
         * <p>
         * Definition BD9.
         * <p>
         * At the end of this function:
         * <ul>
         * <li>The member variable _matchingPDI is set to point to the index of the
         * matching PDI character for each isolate initiator character. If there is
         * no matching PDI, it is set to the length of the input text. For other
         * characters, it is set to -1.
         * <li>The member variable _matchingIsolateInitiator is set to point to the
         * index of the matching isolate initiator character for each PDI character.
         * If there is no matching isolate initiator, or the character is not a PDI,
         * it is set to -1.
         * </ul>
         */
        private void DetermineMatchingIsolates()
        {
            _matchingPDI = new int[_textLength];
            _matchingIsolateInitiator = new int[_textLength];

            for (var i = 0; i < _textLength; ++i)
            {
                _matchingIsolateInitiator[i] = -1;
            }

            for (var i = 0; i < _textLength; ++i)
            {
                _matchingPDI[i] = -1;

                var t = _resultTypes[i];
                if (t == LRI || t == RLI || t == FSI)
                {
                    var depthCounter = 1;
                    for (var j = i + 1; j < _textLength; ++j)
                    {
                        var u = _resultTypes[j];
                        if (u == LRI || u == RLI || u == FSI)
                        {
                            ++depthCounter;
                        }
                        else if (u == PDI)
                        {
                            --depthCounter;
                            if (depthCounter == 0)
                            {
                                _matchingPDI[i] = j;
                                _matchingIsolateInitiator[j] = i;
                                break;
                            }
                        }
                    }
                    if (_matchingPDI[i] == -1)
                    {
                        _matchingPDI[i] = _textLength;
                    }
                }
            }
        }

        /**
         * Determines the paragraph level based on rules P2, P3. This is also used
         * in rule X5c to find if an FSI should resolve to LRI or RLI.
         *
         * @param startIndex
         *            the index of the beginning of the substring
         * @param endIndex
         *            the index of the character after the end of the string
         *
         * @return the resolved paragraph direction of the substring limited by
         *         startIndex and endIndex
         */
        private ParagraphDirection DetermineParagraphEmbeddingLevel(int startIndex, int endIndex)
        {
            var strongType = Unknown; // unknown

            // Rule P2.
            for (var i = startIndex; i < endIndex; ++i)
            {
                var t = _resultTypes[i];
                if (t == L || t == AL || t == R)
                {
                    strongType = t;
                    break;
                }
                if (t == FSI || t == LRI || t == RLI)
                {
                    i = _matchingPDI[i]; // skip over to the matching PDI
                    //assert(i <= endIndex);
                }
            }

            // Rule P3.
            if (strongType == Unknown)
            { // none found
              // default embedding level when no strong types found is 0.
                return ParagraphDirection.Left;
            }
            if (strongType == L)
            {
                return ParagraphDirection.Left;
            }
            // AL, R
            return ParagraphDirection.Right;
        }

        public const int MaxDepth = 125;

        // This stack will store the embedding levels and override and isolated
        // statuses
        private class DirectionalStatusStack
        {
            private int _stackCounter;
            private readonly sbyte[] _embeddingLevelStack = new sbyte[MaxDepth + 1];
            private readonly BidiDirection[] _overrideStatusStack = new BidiDirection[MaxDepth + 1];
            private readonly bool[] _isolateStatusStack = new bool[MaxDepth + 1];

            public void Empty()
            {
                _stackCounter = 0;
            }

            public void Push(sbyte level, BidiDirection overrideStatus, bool isolateStatus)
            {
                _embeddingLevelStack[_stackCounter] = level;
                _overrideStatusStack[_stackCounter] = overrideStatus;
                _isolateStatusStack[_stackCounter] = isolateStatus;
                ++_stackCounter;
            }

            public void Pop()
            {
                --_stackCounter;
            }

            public int Depth()
            {
                return _stackCounter;
            }

            public sbyte LastEmbeddingLevel()
            {
                return _embeddingLevelStack[_stackCounter - 1];
            }

            public BidiDirection LastDirectionalOverrideStatus()
            {
                return _overrideStatusStack[_stackCounter - 1];
            }

            public bool LastDirectionalIsolateStatus()
            {
                return _isolateStatusStack[_stackCounter - 1];
            }
        }

        /**
         * Determine explicit levels using rules X1 - X8
         */
        private void DetermineExplicitEmbeddingLevels()
        {
            var stack = new DirectionalStatusStack();

            // Rule X1.
            stack.Empty();
            stack.Push((sbyte)_paragraphEmbeddingLevel, ON, false);
            var overflowIsolateCount = 0;
            var overflowEmbeddingCount = 0;
            var validIsolateCount = 0;
            for (var i = 0; i < _textLength; ++i)
            {
                var t = _resultTypes[i];

                // Rules X2, X3, X4, X5, X5a, X5b, X5c
                switch (t)
                {
                    case RLE:
                    case LRE:
                    case RLO:
                    case LRO:
                    case RLI:
                    case LRI:
                    case FSI:
                        var isIsolate = t == RLI || t == LRI || t == FSI;
                        var isRtl = t == RLE || t == RLO || t == RLI;
                        // override if this is an FSI that resolves to RLI
                        if (t == FSI)
                        {
                            isRtl = DetermineParagraphEmbeddingLevel(i + 1, _matchingPDI[i]) == ParagraphDirection.Right;
                        }

                        if (isIsolate)
                        {
                            _resultLevels[i] = stack.LastEmbeddingLevel();
                        }

                        sbyte newLevel;
                        if (isRtl)
                        {
                            // least greater odd
                            newLevel = (sbyte)((stack.LastEmbeddingLevel() + 1) | 1);
                        }
                        else
                        {
                            // least greater even
                            newLevel = (sbyte)((stack.LastEmbeddingLevel() + 2) & ~1);
                        }

                        if (newLevel <= MaxDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                        {
                            if (isIsolate)
                            {
                                ++validIsolateCount;
                            }
                            // Push new embedding level, override status, and isolated
                            // status.
                            // No check for valid stack counter, since the level check
                            // suffices.
                            stack.Push(
                                    newLevel,
                                    t == LRO ? L : t == RLO ? R : ON,
                                    isIsolate);

                            // Not really part of the spec
                            if (!isIsolate)
                            {
                                _resultLevels[i] = newLevel;
                            }
                        }
                        else
                        {
                            // This is an invalid explicit formatting character,
                            // so apply the "Otherwise" part of rules X2-X5b.
                            if (isIsolate)
                            {
                                ++overflowIsolateCount;
                            }
                            else
                            { // !isIsolate
                                if (overflowIsolateCount == 0)
                                {
                                    ++overflowEmbeddingCount;
                                }
                            }
                        }
                        break;

                    // Rule X6a
                    case PDI:
                        if (overflowIsolateCount > 0)
                        {
                            --overflowIsolateCount;
                        }
                        else if (validIsolateCount == 0)
                        {
                            // do nothing
                        }
                        else
                        {
                            overflowEmbeddingCount = 0;
                            while (!stack.LastDirectionalIsolateStatus())
                            {
                                stack.Pop();
                            }
                            stack.Pop();
                            --validIsolateCount;
                        }
                        _resultLevels[i] = stack.LastEmbeddingLevel();
                        break;

                    // Rule X7
                    case PDF:
                        // Not really part of the spec
                        _resultLevels[i] = stack.LastEmbeddingLevel();

                        if (overflowIsolateCount > 0)
                        {
                            // do nothing
                        }
                        else if (overflowEmbeddingCount > 0)
                        {
                            --overflowEmbeddingCount;
                        }
                        else if (!stack.LastDirectionalIsolateStatus() && stack.Depth() >= 2)
                        {
                            stack.Pop();
                        }
                        break;

                    case B:
                        // Rule X8.

                        // These values are reset for clarity, in this implementation B
                        // can only occur as the last code in the array.
                        stack.Empty();
                        overflowIsolateCount = 0;
                        overflowEmbeddingCount = 0;
                        validIsolateCount = 0;
                        _resultLevels[i] = (sbyte)_paragraphEmbeddingLevel;
                        break;

                    default:
                        _resultLevels[i] = stack.LastEmbeddingLevel();
                        if (stack.LastDirectionalOverrideStatus() != ON)
                        {
                            _resultTypes[i] = stack.LastDirectionalOverrideStatus();
                        }
                        break;
                }
            }
        }

        private class IsolatingRunSequence
        {
            private readonly int[] _indexes; // indexes to the original string
            private readonly BidiReference _bidiReference;
            private readonly BidiDirection[] _types; // type of each character using the index
            private sbyte[] _resolvedLevels; // resolved levels after application of
                                            // rules
            private readonly int _length; // length of isolating run sequence in
                                         // characters
            private readonly sbyte _level;
            private readonly BidiDirection _sos, _eos;

            /**
             * Rule X10, second bullet: Determine the start-of-sequence (sos) and end-of-sequence (eos) types,
             * 			 either L or R, for each isolating run sequence.
             * @param inputIndexes
             */
            public IsolatingRunSequence(int[] inputIndexes, BidiReference bidiReference)
            {
                _indexes = inputIndexes;
                _bidiReference = bidiReference;
                _length = _indexes.Length;

                _types = new BidiDirection[_length];
                for (var i = 0; i < _length; ++i)
                {
                    _types[i] = _bidiReference._resultTypes[_indexes[i]];
                }

                // assign level, sos and eos
                _level = _bidiReference._resultLevels[_indexes[0]];

                var prevChar = _indexes[0] - 1;
                while (prevChar >= 0 && IsRemovedByX9(_bidiReference._initialTypes[prevChar]))
                {
                    --prevChar;
                }
                var prevLevel = prevChar >= 0 ? _bidiReference._resultLevels[prevChar] : (sbyte)_bidiReference._paragraphEmbeddingLevel;
                _sos = TypeForLevel(Math.Max(prevLevel, _level));

                var lastType = _types[_length - 1];
                sbyte succLevel;
                if (lastType == LRI || lastType == RLI || lastType == FSI)
                {
                    succLevel = (sbyte)_bidiReference._paragraphEmbeddingLevel;
                }
                else
                {
                    var limit = _indexes[_length - 1] + 1; // the first character
                                                         // after the end of
                                                         // run sequence
                    while (limit < _bidiReference._textLength && IsRemovedByX9(_bidiReference._initialTypes[limit]))
                    {
                        ++limit;
                    }
                    succLevel = limit < _bidiReference._textLength ? _bidiReference._resultLevels[limit] : (sbyte)_bidiReference._paragraphEmbeddingLevel;
                }
                _eos = TypeForLevel(Math.Max(succLevel, _level));
            }

            /**
             * Resolving bidi paired brackets  Rule N0
             */

            public void ResolvePairedBrackets()
            {
                _bidiReference.PBA = new BidiPBAReference();
                _bidiReference.PBA.ResolvePairedBrackets(_indexes, _types, _bidiReference._pairTypes, _bidiReference._pairValues, _sos, _level);
            }


            /**
             * Resolving weak types Rules W1-W7.
             *
             * Note that some weak types (EN, AN) remain after this processing is
             * complete.
             */
            public void ResolveWeakTypes()
            {

                // on entry, only these types remain
                AssertOnly(new[] { L, R, AL, EN, ES, ET, AN, CS, B, S, WS, ON, NSM, LRI, RLI, FSI, PDI });

                // Rule W1.
                // Changes all NSMs.
                var preceedingCharacterType = _sos;
                for (var i = 0; i < _length; ++i)
                {
                    var t = _types[i];
                    if (t == NSM)
                    {
                        _types[i] = preceedingCharacterType;
                    }
                    else
                    {
                        preceedingCharacterType = t;
                    }
                }

                // Rule W2.
                // EN does not change at the start of the run, because sos != AL.
                for (var i = 0; i < _length; ++i)
                {
                    if (_types[i] == EN)
                    {
                        for (var j = i - 1; j >= 0; --j)
                        {
                            var t = _types[j];
                            if (t == L || t == R || t == AL)
                            {
                                if (t == AL)
                                {
                                    _types[i] = AN;
                                }
                                break;
                            }
                        }
                    }
                }

                // Rule W3.
                for (var i = 0; i < _length; ++i)
                {
                    if (_types[i] == AL)
                    {
                        _types[i] = R;
                    }
                }

                // Rule W4.
                // Since there must be values on both sides for this rule to have an
                // effect, the scan skips the first and last value.
                //
                // Although the scan proceeds left to right, and changes the type
                // values in a way that would appear to affect the computations
                // later in the scan, there is actually no problem. A change in the
                // current value can only affect the value to its immediate right,
                // and only affect it if it is ES or CS. But the current value can
                // only change if the value to its right is not ES or CS. Thus
                // either the current value will not change, or its change will have
                // no effect on the remainder of the analysis.

                for (var i = 1; i < _length - 1; ++i)
                {
                    if (_types[i] == ES || _types[i] == CS)
                    {
                        var prevSepType = _types[i - 1];
                        var succSepType = _types[i + 1];
                        if (prevSepType == EN && succSepType == EN)
                        {
                            _types[i] = EN;
                        }
                        else if (_types[i] == CS && prevSepType == AN && succSepType == AN)
                        {
                            _types[i] = AN;
                        }
                    }
                }

                // Rule W5.
                for (var i = 0; i < _length; ++i)
                {
                    if (_types[i] == ET)
                    {
                        // locate end of sequence
                        var runstart = i;
                        var runlimit = FindRunLimit(runstart, _length, new[] { ET });

                        // check values at ends of sequence
                        var t = runstart == 0 ? _sos : _types[runstart - 1];

                        if (t != EN)
                        {
                            t = runlimit == _length ? _eos : _types[runlimit];
                        }

                        if (t == EN)
                        {
                            SetTypes(runstart, runlimit, EN);
                        }

                        // continue at end of sequence
                        i = runlimit;
                    }
                }

                // Rule W6.
                for (var i = 0; i < _length; ++i)
                {
                    var t = _types[i];
                    if (t == ES || t == ET || t == CS)
                    {
                        _types[i] = ON;
                    }
                }

                // Rule W7.
                for (var i = 0; i < _length; ++i)
                {
                    if (_types[i] == EN)
                    {
                        // set default if we reach start of run
                        var prevStrongType = _sos;
                        for (var j = i - 1; j >= 0; --j)
                        {
                            var t = _types[j];
                            if (t == L || t == R)
                            { // AL's have been changed to R
                                prevStrongType = t;
                                break;
                            }
                        }
                        if (prevStrongType == L)
                        {
                            _types[i] = L;
                        }
                    }
                }
            }

            /**
             * 6) resolving neutral types Rules N1-N2.
             */
            public void ResolveNeutralTypes()
            {

                // on entry, only these types can be in _resultTypes
                AssertOnly(new[] { L, R, EN, AN, B, S, WS, ON, RLI, LRI, FSI, PDI });

                for (var i = 0; i < _length; ++i)
                {
                    var t = _types[i];
                    if (t == WS || t == ON || t == B || t == S || t == RLI || t == LRI || t == FSI || t == PDI)
                    {
                        // find bounds of run of neutrals
                        var runstart = i;
                        var runlimit = FindRunLimit(runstart, _length, new[] { B, S, WS, ON, RLI, LRI, FSI, PDI });

                        // determine effective types at ends of run
                        BidiDirection leadingType;
                        BidiDirection trailingType;

                        // Note that the character found can only be L, R, AN, or
                        // EN.
                        if (runstart == 0)
                        {
                            leadingType = _sos;
                        }
                        else
                        {
                            leadingType = _types[runstart - 1];
                            if (leadingType == AN || leadingType == EN)
                            {
                                leadingType = R;
                            }
                        }

                        if (runlimit == _length)
                        {
                            trailingType = _eos;
                        }
                        else
                        {
                            trailingType = _types[runlimit];
                            if (trailingType == AN || trailingType == EN)
                            {
                                trailingType = R;
                            }
                        }

                        BidiDirection resolvedType;
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (leadingType == trailingType)
                        {
                            // Rule N1.
                            resolvedType = leadingType;
                        }
                        else
                        {
                            // Rule N2.
                            // Notice the embedding level of the run is used, not
                            // the paragraph embedding level.
                            resolvedType = TypeForLevel(_level);
                        }

                        SetTypes(runstart, runlimit, resolvedType);

                        // skip over run of (former) neutrals
                        i = runlimit;
                    }
                }
            }

            /**
             * 7) resolving implicit embedding levels Rules I1, I2.
             */
            public void ResolveImplicitLevels()
            {

                // on entry, only these types can be in _resultTypes
                AssertOnly(new[] { L, R, EN, AN });

                _resolvedLevels = new sbyte[_length];
                SetLevels(_resolvedLevels, 0, _length, _level);

                if ((_level & 1) == 0)
                { // even level
                    for (var i = 0; i < _length; ++i)
                    {
                        var t = _types[i];
                        // Rule I1.
                        if (t == L)
                        {
                            // no change
                        }
                        else if (t == R)
                        {
                            _resolvedLevels[i] += 1;
                        }
                        else
                        { // t == AN || t == EN
                            _resolvedLevels[i] += 2;
                        }
                    }
                }
                else
                { // odd level
                    for (var i = 0; i < _length; ++i)
                    {
                        var t = _types[i];
                        // Rule I2.
                        if (t == R)
                        {
                            // no change
                        }
                        else
                        { // t == L || t == AN || t == EN
                            _resolvedLevels[i] += 1;
                        }
                    }
                }
            }

            /*
             * Applies the levels and types resolved in rules W1-I2 to the
             * _resultLevels array.
             */
            public void ApplyLevelsAndTypes()
            {
                for (var i = 0; i < _length; ++i)
                {
                    var originalIndex = _indexes[i];
                    _bidiReference._resultTypes[originalIndex] = _types[i];
                    _bidiReference._resultLevels[originalIndex] = _resolvedLevels[i];
                }
            }

            /**
             * Return the limit of the run consisting only of the types in validSet
             * starting at index. This checks the value at index, and will return
             * index if that value is not in validSet.
             */
            // ReSharper disable once SuggestBaseTypeForParameter
            private int FindRunLimit(int index, int limit, BidiDirection[] validSet)
            {
                while (index < limit)
                {
                    var continueOuter = false;

                    var t = _types[index];
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < validSet.Length; ++i)
                    {
                        if (t == validSet[i])
                        {
                            ++index;
                            continueOuter = true;
                            break;
                        }
                    }
                    if (continueOuter)
                        continue;
                    // didn't find a match in validSet
                    return index;
                }
                return limit;
            }

            /**
             * Set types from start up to (but not including) limit to newType.
             */
            private void SetTypes(int start, int limit, BidiDirection newType)
            {
                for (var i = start; i < limit; ++i)
                {
                    _types[i] = newType;
                }
            }

            /**
             * Algorithm validation. Assert that all values in types are in the
             * provided set.
             */
            private void AssertOnly(BidiDirection[] codes)
            {
                for (var i = 0; i < _length; ++i)
                {
                    var continueOuter = false;
                    var t = _types[i];
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var j = 0; j < codes.Length; ++j)
                    {
                        // ReSharper disable once InvertIf
                        if (t == codes[j])
                        {
                            continueOuter = true;
                            break;
                        }
                    }
                    if (continueOuter)
                        continue;

                    throw new ArgumentException("invalid bidi code " + t + " present in assertOnly at position " + _indexes[i]);
                }
            }
        }

        /**
         * Determines the level runs. Rule X9 will be applied in determining the
         * runs, in the way that makes sure the characters that are supposed to be
         * removed are not included in the runs.
         *
         * @return an array of level runs. Each level run is described as an array
         *         of indexes into the input string.
         */
        private int[][] DetermineLevelRuns()
        {
            // temporary array to hold the run
            var temporaryRun = new int[_textLength];
            // temporary array to hold the list of runs
            var allRuns = new int[_textLength][];
            var numRuns = 0;

            var currentLevel = (sbyte)-1;
            var runLength = 0;
            for (var i = 0; i < _textLength; ++i)
            {
                if (!IsRemovedByX9(_initialTypes[i]))
                {
                    if (_resultLevels[i] != currentLevel)
                    { // we just encountered a
                      // new run
                      // Wrap up last run
                        if (currentLevel >= 0)
                        { // only wrap it up if there was a run
                            var run = temporaryRun.ArrayCopyOf(runLength);
                            allRuns[numRuns] = run;
                            ++numRuns;
                        }
                        // Start new run
                        currentLevel = _resultLevels[i];
                        runLength = 0;
                    }
                    temporaryRun[runLength] = i;
                    ++runLength;
                }
            }
            // Wrap up the final run, if any
            if (runLength != 0)
            {
                var run = temporaryRun.ArrayCopyOf(runLength);
                allRuns[numRuns] = run;
                ++numRuns;
            }

            return allRuns.ArrayCopyOf(numRuns);
        }

        /**
         * Definition BD13. Determine isolating run sequences.
         *
         * @return an array of isolating run sequences.
         */
        private IsolatingRunSequence[] DetermineIsolatingRunSequences()
        {
            var levelRuns = DetermineLevelRuns();
            var numRuns = levelRuns.Length;

            // Compute the run that each character belongs to
            var runForCharacter = new int[_textLength];
            for (var runNumber = 0; runNumber < numRuns; ++runNumber)
            {
                for (var i = 0; i < levelRuns[runNumber].Length; ++i)
                {
                    var characterIndex = levelRuns[runNumber][i];
                    runForCharacter[characterIndex] = runNumber;
                }
            }

            var sequences = new IsolatingRunSequence[numRuns];
            var numSequences = 0;
            var currentRunSequence = new int[_textLength];
            for (var i = 0; i < levelRuns.Length; ++i)
            {
                var firstCharacter = levelRuns[i][0];
                if (_initialTypes[firstCharacter] != PDI || _matchingIsolateInitiator[firstCharacter] == -1)
                {
                    var currentRunSequenceLength = 0;
                    var run = i;
                    do
                    {
                        // Copy this level run into currentRunSequence
                        Array.Copy(levelRuns[run], 0, currentRunSequence, currentRunSequenceLength, levelRuns[run].Length);
                        currentRunSequenceLength += levelRuns[run].Length;

                        var lastCharacter = currentRunSequence[currentRunSequenceLength - 1];
                        var lastType = _initialTypes[lastCharacter];
                        if ((lastType == LRI || lastType == RLI || lastType == FSI) &&
                                _matchingPDI[lastCharacter] != _textLength)
                        {
                            run = runForCharacter[_matchingPDI[lastCharacter]];
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    sequences[numSequences] = new IsolatingRunSequence(currentRunSequence.ArrayCopyOf(currentRunSequenceLength), this);
                    ++numSequences;
                }
            }
            return sequences.ArrayCopyOf(numSequences);
        }

        /**
         * Assign level information to characters removed by rule X9. This is for
         * ease of relating the level information to the original input data. Note
         * that the levels assigned to these codes are arbitrary, they're chosen so
         * as to avoid breaking level runs.
         *
         * @param _textLength
         *            the length of the data after compression
         * @return the length of the data (original length of types array supplied
         *         to constructor)
         */
        private void AssignLevelsToCharactersRemovedByX9()
        {
            for (var i = 0; i < _initialTypes.Length; ++i)
            {
                var t = _initialTypes[i];
                // ReSharper disable once InvertIf
                if (t == LRE || t == RLE || t == LRO || t == RLO || t == PDF || t == BN)
                {
                    _resultTypes[i] = t;
                    _resultLevels[i] = -1;
                }
            }

            // now propagate forward the levels information (could have
            // propagated backward, the main thing is not to introduce a level
            // break where one doesn't already exist).

            if (_resultLevels[0] == -1)
            {
                _resultLevels[0] = (sbyte)_paragraphEmbeddingLevel;
            }
            for (var i = 1; i < _initialTypes.Length; ++i)
            {
                if (_resultLevels[i] == -1)
                {
                    _resultLevels[i] = _resultLevels[i - 1];
                }
            }

            // Embedding information is for informational purposes only
            // so need not be adjusted.
        }

        //
        // Output
        //

        /**
         * Return levels array breaking lines at offsets in linebreaks. <br>
         * Rule L1.
         * <p>
         * The returned levels array contains the resolved level for each bidi code
         * passed to the constructor.
         * <p>
         * The linebreaks array must include at least one value. The values must be
         * in strictly increasing order (no duplicates) between 1 and the length of
         * the text, inclusive. The last value must be the length of the text.
         *
         * @param linebreaks
         *            the offsets at which to break the paragraph
         * @return the resolved levels of the text
         */
        public sbyte[] GetLevels(int[] linebreaks)
        {

            // Note that since the previous processing has removed all
            // P, S, and WS values from _resultTypes, the values referred to
            // in these rules are the initial types, before any processing
            // has been applied (including processing of overrides).
            //
            // This example implementation has reinserted explicit format codes
            // and BN, in order that the levels array correspond to the
            // initial text. Their final placement is not normative.
            // These codes are treated like WS in this implementation,
            // so they don't interrupt sequences of WS.

            ValidateLineBreaks(linebreaks, _textLength);

            var result = _resultLevels.ToArray(); // will be returned to
                                                     // caller

            // don't worry about linebreaks since if there is a break within
            // a series of WS values preceding S, the linebreak itself
            // causes the reset.
            for (var i = 0; i < result.Length; ++i)
            {
                var t = _initialTypes[i];
                if (t == B || t == S)
                {
                    // Rule L1, clauses one and two.
                    result[i] = (sbyte)_paragraphEmbeddingLevel;

                    // Rule L1, clause three.
                    for (var j = i - 1; j >= 0; --j)
                    {
                        if (IsWhitespace(_initialTypes[j]))
                        { // including format
                          // codes
                            result[j] = (sbyte)_paragraphEmbeddingLevel;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Rule L1, clause four.
            var start = 0;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < linebreaks.Length; ++i)
            {
                var limit = linebreaks[i];
                for (var j = limit - 1; j >= start; --j)
                {
                    if (IsWhitespace(_initialTypes[j]))
                    { // including format codes
                        result[j] = (sbyte)_paragraphEmbeddingLevel;
                    }
                    else
                    {
                        break;
                    }
                }

                start = limit;
            }

            return result;
        }

        /**
         * Return reordering array breaking lines at offsets in linebreaks.
         * <p>
         * The reordering array maps from a visual index to a logical index. Lines
         * are concatenated from left to right. So for example, the fifth character
         * from the left on the third line is
         *
         * <pre>
         * getReordering(linebreaks)[linebreaks[1] + 4]
         * </pre>
         *
         * (linebreaks[1] is the position after the last character of the second
         * line, which is also the index of the first character on the third line,
         * and adding four gets the fifth character from the left).
         * <p>
         * The linebreaks array must include at least one value. The values must be
         * in strictly increasing order (no duplicates) between 1 and the length of
         * the text, inclusive. The last value must be the length of the text.
         *
         * @param linebreaks
         *            the offsets at which to break the paragraph.
         */
        public int[] GetReordering(int[] linebreaks)
        {
            ValidateLineBreaks(linebreaks, _textLength);

            var levels = GetLevels(linebreaks);

            return ComputeMultilineReordering(levels, linebreaks);
        }

        /**
         * Return multiline reordering array for a given level array. Reordering
         * does not occur across a line break.
         */
        // ReSharper disable once SuggestBaseTypeForParameter
        private static int[] ComputeMultilineReordering(sbyte[] levels, int[] linebreaks)
        {
            var result = new int[levels.Length];

            var start = 0;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < linebreaks.Length; ++i)
            {
                var limit = linebreaks[i];

                var templevels = new sbyte[limit - start];
                Array.Copy(levels, start, templevels, 0, templevels.Length);

                var temporder = ComputeReordering(templevels);
                for (var j = 0; j < temporder.Length; ++j)
                {
                    result[start + j] = temporder[j] + start;
                }

                start = limit;
            }

            return result;
        }

        /**
         * Return reordering array for a given level array. This reorders a single
         * line. The reordering is a visual to logical map. For example, the
         * leftmost char is string.charAt(order[0]). Rule L2.
         */
        private static int[] ComputeReordering(sbyte[] levels)
        {
            var lineLength = levels.Length;

            var result = new int[lineLength];

            // initialize order
            for (var i = 0; i < lineLength; ++i)
            {
                result[i] = i;
            }

            // locate highest level found on line.
            // Note the rules say text, but no reordering across line bounds is
            // performed, so this is sufficient.
            sbyte highestLevel = 0;
            sbyte lowestOddLevel = MaxDepth + 2;
            for (var i = 0; i < lineLength; ++i)
            {
                var level = levels[i];
                if (level > highestLevel)
                {
                    highestLevel = level;
                }
                if (((level & 1) != 0) && level < lowestOddLevel)
                {
                    lowestOddLevel = level;
                }
            }

            for (int level = highestLevel; level >= lowestOddLevel; --level)
            {
                for (var i = 0; i < lineLength; ++i)
                {
                    if (levels[i] >= level)
                    {
                        // find range of text at or above this level
                        var start = i;
                        var limit = i + 1;
                        while (limit < lineLength && levels[limit] >= level)
                        {
                            ++limit;
                        }

                        // reverse run
                        for (int j = start, k = limit - 1; j < k; ++j, --k)
                        {
                            var temp = result[j];
                            result[j] = result[k];
                            result[k] = temp;
                        }

                        // skip to end of level run
                        i = limit;
                    }
                }
            }

            return result;
        }

        /**
         * Return the base level of the paragraph.
         */
        public sbyte GetBaseLevel()
        {
            return (sbyte)_paragraphEmbeddingLevel;
        }

        // --- internal utilities -------------------------------------------------

        /**
         * Return true if the type is considered a whitespace type for the line
         * break rules.
         */
        private static bool IsWhitespace(BidiDirection biditype)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (biditype)
            {
                case LRE:
                case RLE:
                case LRO:
                case RLO:
                case PDF:
                case LRI:
                case RLI:
                case FSI:
                case PDI:
                case BN:
                case WS:
                    return true;
                default:
                    return false;
            }
        }

        /**
         * Return true if the type is one of the types removed in X9.
         * Made public so callers can duplicate the effect.
         */
        public static bool IsRemovedByX9(BidiDirection biditype)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (biditype)
            {
                case LRE:
                case RLE:
                case LRO:
                case RLO:
                case PDF:
                case BN:
                    return true;
                default:
                    return false;
            }
        }

        /**
         * Return the strong type (L or R) corresponding to the level.
         */
        private static BidiDirection TypeForLevel(int level)
        {
            return (level & 0x1) == 0 ? L : R;
        }

        /**
         * Set levels from start up to (but not including) limit to newLevel.
         */
        private static void SetLevels(sbyte[] levels, int start, int limit, sbyte newLevel)
        {
            for (var i = start; i < limit; ++i)
            {
                levels[i] = newLevel;
            }
        }

        // --- input validation ---------------------------------------------------

        /**
         * Throw exception if type array is invalid.
         */
        private static void ValidateTypes(BidiDirection[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            for (var i = 0; i < types.Length; ++i)
            {
                if (types[i] < TypeMin || types[i] > TypeMax)
                {
                    throw new ArgumentException("illegal type value at " + i + ": " + types[i], nameof(types));
                }
            }
            for (var i = 0; i < types.Length - 1; ++i)
            {
                if (types[i] == B)
                {
                    throw new ArgumentException("B type before end of paragraph at index: " + i, nameof(types));
                }
            }
        }

        /**
         * Throw exception if paragraph embedding level is invalid. Special
         * allowance for implicitEmbeddinglevel so that default processing of the
         * paragraph embedding level as implicit can still be performed when
         * using this API.
         */
        private static void ValidateParagraphEmbeddingLevel(ParagraphDirection paragraphEmbeddingLevel)
        {
            if (paragraphEmbeddingLevel != ParagraphDirection.Default &&
                    paragraphEmbeddingLevel != ParagraphDirection.Left &&
                    paragraphEmbeddingLevel != ParagraphDirection.Right)
            {
                throw new ArgumentException("illegal paragraph embedding level: " + paragraphEmbeddingLevel, nameof(paragraphEmbeddingLevel));
            }
        }

        /**
         * Throw exception if line breaks array is invalid.
         */
        private static void ValidateLineBreaks(int[] linebreaks, int textLength)
        {
            var prev = 0;
            for (var i = 0; i < linebreaks.Length; ++i)
            {
                var next = linebreaks[i];
                if (next <= prev)
                {
                    throw new ArgumentException("bad linebreak: " + next + " at index: " + i, nameof(linebreaks));
                }
                prev = next;
            }
            if (prev != textLength)
            {
                throw new ArgumentException("last linebreak must be at " + textLength, nameof(linebreaks));
            }
        }

        /**
         * Throw exception if _pairTypes array is invalid
         */
        private static void ValidatePbTypes(BracketType[] pairTypes)
        {
            if (pairTypes == null)
            {
                throw new ArgumentNullException(nameof(pairTypes));
            }
        }

        /**
         * Throw exception if _pairValues array is invalid or doesn't match _pairTypes in length
         * Unfortunately there's little we can do in terms of validating the values themselves
         */
        private static void ValidatePbValues(int[] pairValues, BracketType[] pairTypes)
        {
            if (pairValues == null)
            {
                throw new ArgumentNullException(nameof(pairValues));
            }
            if (pairTypes.Length != pairValues.Length)
            {
                throw new ArgumentException("_pairTypes is different length from _pairValues", nameof(pairTypes));
            }
        }

        /**
         * static entry point for testing using several arrays of direction and other types and an externally supplied
         * paragraph embedding level. The embedding level may be 0, 1 or 2.
         * <p>
         * 2 means to apply the default algorithm (rules P2 and P3), 0 is for LTR
         * paragraphs, and 1 is for RTL paragraphs.
         *
         * @param types
         *            the directional types array
         * @param _pairTypes
         *           the paired bracket types array
         * @param _pairValues
         * 			 the paired bracket values array
         * @param _paragraphEmbeddingLevel
         *            the externally supplied paragraph embedding level.
         */
        public static BidiReference AnalyzeInput(BidiDirection[] types, BracketType[] pairTypes, int[] pairValues, ParagraphDirection paragraphEmbeddingLevel)
        {
            var bidi = new BidiReference(types, pairTypes, pairValues, paragraphEmbeddingLevel);
            return bidi;
        }
    }
}
