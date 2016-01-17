// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.


// Bidi reference implementation from http://www.unicode.org/Public/PROGRAMS/BidiReferenceJava/
// Original Code ported from java to C# by Sherif Elmetainy
// The credits and copyright info for the original code listed below

/*
 * Last Revised: 2013-09-02
 * 
 * Credits:
 * Written by Asmus Freytag
 * 
 * (C) Copyright ASMUS, Inc. 2013, All Rights Reserved
 *
 * Distributed under the Terms of Use in http://www.unicode.org/copyright.html.
 */

using System;
using System.Collections.Generic;
using static CodeArt.Bidi.BidiDirection;

namespace CodeArt.Bidi
{
    /// <summary>
    /// Reference implementation of the BPA algorithm of the Unicode 6.3 Bidi algorithm.
    /// 
    /// Provides implemenation similar to that of java LinkedListIterator in order to have same behavior
    /// The java version has change detection but it is not needed in this case
    /// 
    /// This implementation is not optimized for performance.  It is intended
    /// as a reference implementation that closely follows the specification
    ///  of the paired bracket part of the Bidirectional Algorithm in 
    /// The Unicode Standard version 6.3
    /// 
    /// The implementation covers definitions BD14-BD16 and rule N0.
    /// 
    /// Like the BidiRefernece class which uses the BidiBPAReferenc class, the implementation is 
    /// designed to decouple the mapping of Unicode properties to characters from the handling
    /// of the Bidi Paired-bracket Algorithm.Such mappings are to be performed by the caller.
    /// 
    ///  One of the properties, the Bidi_Paired_Bracket requires some pre-processing to translate
    /// it into the format used here.Instead of being a code-point mapping from a bracket character
    /// to the other partner of the bracket pair, this implementation accepts any unique identifier
    /// common to BOTH parts of the pair, and 0 or some unique value for all non-bracket characters.
    /// The actual values of these unique identifiers are not defined.
    /// 
    /// The BPA algorithm requires that bracket characters that are canonical equivalents of each
    /// other must be able to be substituted for each other.Callers can accomplish this by re-using
    /// the same unique identifier for such equivalent characters.
    /// 
    /// The resultant values become the pairValues array used in calling the resolvPairedBrackets member.
    /// 
    ///  In implementing BD16, this implementation departs slightly from the "logical" algorithm defined
    /// in UAX#9. In particular, the stack referenced there supports operations that go beyond a "basic"
    /// stack.An equivalent implementation based on a linked list is used here.
    /// 
    /// Author of Java version: Asmus Freytag
    /// </summary>
    public class BidiPBAReference
    {


        private class LinkedListIterator<T>
        {
            private readonly LinkedList<T> _list;
            private int _nextIndex;
            private LinkedListNode<T> _nextNode;
            private LinkedListNode<T> _prevNode;
            private LinkedListNode<T> _currentNode;
            public LinkedListIterator(LinkedList<T> list, int index = 0)
            {
                if (list == null) throw new ArgumentNullException(nameof(list));
                if (index < 0 || index > list.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be from zero to list size.");

                _list = list;
                _nextIndex = 0;
                _nextNode = list.First;
                _prevNode = null;
                while (_nextIndex < index)
                {
                    Next();
                }
                _currentNode = null;

            }

            public int NextIndex()
            {
                return _nextIndex;
            }

            public bool HasNext()
            {
                return _nextNode != null;
            }

            public bool HasPrevious()
            {
                return _prevNode != null;
            }

            public T Next()
            {
                if (_nextNode == null)
                    throw new InvalidOperationException();
                _prevNode = _nextNode;
                _nextNode = _prevNode.Next;
                _nextIndex++;
                _currentNode = _prevNode;
                return _prevNode.Value;
            }

            // ReSharper disable once UnusedMethodReturnValue.Local
            public T Previous()
            {
                if (_prevNode == null)
                    throw new InvalidOperationException();
                _nextNode = _prevNode;
                _prevNode = _nextNode.Previous;
                _nextIndex--;
                _currentNode = _nextNode;
                return _nextNode.Value;
            }

            public void Remove()
            {
                if (_currentNode == null)
                    throw new InvalidOperationException();
                _nextNode = _currentNode.Next;
                _prevNode = _currentNode.Previous;
                _list.Remove(_currentNode);
                _currentNode = null;
            }
        }

        /// <summary>
        /// BD14. An opening paired bracket is a character whose
        /// Bidi_Paired_Bracket_Type property value is Open.
        /// 
        /// BD15. A closing paired bracket is a character whose
        /// Bidi_Paired_Bracket_Type property value is Close.
        /// </summary>
        private BidiDirection _sos; // direction corresponding to start of sequence

        /// <summary>
        /// Holds a pair of index values for opening and closing bracket location of
        /// a bracket pair
        /// Contains additional methods to allow pairs to be sorted by the location
        /// of the opening bracket
        /// </summary>
        public sealed class BracketPair :
                IComparable<BracketPair>,
                IEquatable<BracketPair>
        {
            private readonly int _ichOpener;
            private readonly int _ichCloser;

            public BracketPair(int ichOpener, int ichCloser)
            {
                _ichOpener = ichOpener;
                _ichCloser = ichCloser;
            }

            public override bool Equals(object other)
            {
                return Equals(other as BracketPair);
            }

            public bool Equals(BracketPair otherPair)
            {
                if (otherPair == null)
                    return false;
                return _ichOpener == otherPair._ichOpener
                        && _ichCloser == otherPair._ichCloser;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;
                    hash = hash * 31 + _ichOpener;
                    hash = hash * 31 + _ichCloser;
                    return hash;
                }
            }

            public int CompareTo(BracketPair otherPair)
            {
                if (_ichOpener == otherPair._ichOpener)
                    return 0;
                if (_ichOpener < otherPair._ichOpener)
                    return -1;
                return 1;
            }

            public override string ToString()
            {
                return "(" + _ichOpener + ", " + _ichCloser + ")";
            }

            public int GetOpener()
            {
                return _ichOpener;
            }

            public int GetCloser()
            {
                return _ichCloser;
            }
        }

        // The following is a restatement of BD 16 using non-algorithmic language. 
        //
        // A bracket pair is a pair of characters consisting of an opening
        // paired bracket and a closing paired bracket such that the
        // Bidi_Paired_Bracket property value of the former equals the latter,
        // subject to the following constraints.
        // - both characters of a pair occur in the same isolating run sequence
        // - the closing character of a pair follows the opening character
        // - any bracket character can belong at most to one pair, the earliest possible one
        // - any bracket character not part of a pair is treated like an ordinary character
        // - pairs may nest properly, but their spans may not overlap otherwise

        // Bracket characters with canonical decompositions are supposed to be treated
        // as if they had been normalized, to allow normalized and non-normalized text
        // to give the same result. In this implementation that step is pushed out to
        // the caller - see definition of the pairValues array.

        /// <summary>
        /// list of positions for opening brackets
        /// </summary>
        private LinkedList<int> _openers;

        // bracket pair positions sorted by location of opening bracket
        private SortedSet<BracketPair> _pairPositions;

        public string GetPairPositionsString()
        {
            var tempPositions = new SortedSet<BracketPair>();
            foreach (var pair in _pairPositions)
            {
                tempPositions.Add(new BracketPair(_indexes[pair.GetOpener()], _indexes[pair.GetCloser()]));
            }
            return tempPositions.CollectionToString();
        }


        /// <summary>
        /// directional bidi codes for an isolated run
        /// </summary>
        public BidiDirection[] CodesIsolatedRun;
        /// <summary>
        /// array of index values into the original string
        /// </summary>
        private int[] _indexes;

        /// <summary>
        /// check whether characters at putative positions could form a bracket pair
        /// based on the paired bracket character properties
        /// </summary>
        /// <param name="pairValues">unique ID for the pair (or set) of canonically matched brackets</param>
        /// <param name="ichOpener">position of the opening bracket</param>
        /// <param name="ichCloser">position of the closing bracket</param>
        /// <returns>true if match</returns>
        private bool MatchOpener(int[] pairValues, int ichOpener, int ichCloser)
        {
            return pairValues[_indexes[ichOpener]] == pairValues[_indexes[ichCloser]];
        }

        /// <summary>
        /// removes any list elements from First to index
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void RemoveHead(LinkedList<int> list, int index)
        {

            var iter = new LinkedListIterator<int>(list, index);

            while (iter.HasPrevious())
            {
                iter.Previous();
                iter.Remove();
            }
        }



        /// <summary>
        ///
        /// locate all Paired Bracket characters and determine whether they form
        /// pairs according to BD16.This implementation uses a linked list instead
        /// of a stack, because, while elements are added at the front (like a push)
        /// there are not generally removed in atomic 'pop' operations, reducing the
        /// benefit of the stack archetype.
        /// </summary>
        /// <param name="pairTypes">array of paired Bracket types</param>
        /// <param name="pairValues">array of characters codes such that for all bracket characters it contains the same unique value if their
        /// Bidi_Paired_Bracket properties map between them.
        /// For  brackets that have canonical decompositions (singleton  mappings) 
        /// it contains the same value as for the canonically decomposed character.For characters that have paired 
        /// bracket type of "n" the value is ignored.
        /// </param>
        private void LocateBrackets(BracketType[] pairTypes, int[] pairValues)
        {
            _openers = new LinkedList<int>();
            _pairPositions = new SortedSet<BracketPair>();

            // traverse the run
            // do that explicitly (not in a for-each) so we can record position
            for (var ich = 0; ich < _indexes.Length; ich++)
            {

                // look at the bracket type for each character
                switch (pairTypes[_indexes[ich]])
                {
                    case BracketType.None: // default - non paired
                        continue; // continue scanning

                    // opening bracket found, note location
                    case BracketType.Opening:
                        // remember opener location, most recent first
                        _openers.AddFirst(ich);
                        break;

                    // closing bracket found
                    case BracketType.Closing:
                        // see if there is a match
                        if (_openers.Count == 0)
                            continue; // no opening bracket defined

                        var iter = new LinkedListIterator<int>(_openers);

                        while (iter.HasNext())
                        {
                            var opener = iter.Next();
                            if (MatchOpener(pairValues, opener, ich))
                            {
                                // if the opener matches, add nested pair to the ordered list
                                _pairPositions
                                        .Add(new BracketPair(opener, ich));
                                // remove up to and including matched opener
                                RemoveHead(_openers, iter.NextIndex());
                                break;
                            }
                        }
                        // if we get here, the closing bracket matched no openers
                        // and gets ignored
                        continue;
                }
            }
        }

        /// <summary>
        /// Bracket pairs within an isolating run sequence are processed as units so
        /// that both the opening and the closing paired bracket in a pair resolve to
        /// the same direction.
        /// 
        /// N0. Process bracket pairs in an isolating run sequence sequentially in
        /// the logical order of the text positions of the opening paired brackets
        /// using the logic given below. Within this scope, bidirectional types EN
        /// and AN are treated as R.
        /// 
        /// Identify the bracket pairs in the current isolating run sequence
        /// according to BD16. For each bracket-pair element in the list of pairs of
        /// text positions:
        /// 
        /// a Inspect the bidirectional types of the characters enclosed within the
        /// bracket pair.
        /// 
        /// b If any strong type (either L or R) matching the embedding direction is
        /// found, set the type for both brackets in the pair to match the embedding
        /// direction.
        /// 
        /// o [ e ] o -> o e e e o
        /// 
        /// o [ o e ] -> o e o e e
        /// 
        /// o [ NI e ] -> o e NI e e
        /// 
        /// c Otherwise, if a strong type (opposite the embedding direction) is
        /// found, test for adjacent strong types as follows: 1 First, check
        /// backwards before the opening paired bracket until the first strong type
        /// (L, R, or sos) is found. If that first preceding strong type is opposite
        /// the embedding direction, then set the type for both brackets in the pair
        /// to that type. 2 Otherwise, set the type for both brackets in the pair to
        /// the embedding direction.
        /// 
        /// o [ o ] e -> o o o o e
        /// 
        /// o [ o NI ] o -> o o o NI o o
        /// 
        /// e [ o ] o -> e e o e o
        /// 
        /// e [ o ] e -> e e o e e
        /// 
        /// e ( o [ o ] NI ) e -> e e o o o o NI e e
        /// 
        /// d Otherwise, do not set the type for the current bracket pair. Note that
        /// if the enclosed text contains no strong types the paired brackets will
        /// both resolve to the same level when resolved individually using rules N1
        /// and N2.
        /// 
        /// e ( NI ) o -> e ( NI ) o
        ///  map character's directional code to strong type as required by rule N0
        /// </summary>
        /// <param name="ich">index into array of directional codes</param>
        /// <returns>R or L for strong directional codes, ON for anything else</returns>
        private BidiDirection GetStrongTypeN0(int ich)
        {

            switch (CodesIsolatedRun[ich])
            {
                default:
                    return ON;
                // in the scope of N0, number types are treated as R
                case EN:
                case AN:
                case AL:
                case R:
                    return R;
                case L:
                    return L;
            }
        }


        /// <summary>
        /// determine which strong types are contained inside a Bracket Pair
        /// </summary>
        /// <param name="pairedLocation">a bracket pair</param>
        /// <param name="dirEmbed"> the embedding direction</param>
        /// <returns>ON if no strong type found, otherwise return the embedding
        /// direction, unless the only strong type found is opposite the
        ///  embedding direction, in which case that is returned
        /// </returns>
        private BidiDirection ClassifyPairContent(BracketPair pairedLocation, BidiDirection dirEmbed)
        {
            var dirOpposite = ON;
            for (var ich = pairedLocation.GetOpener() + 1; ich < pairedLocation
                    .GetCloser(); ich++)
            {
                var dir = GetStrongTypeN0(ich);
                if (dir == ON)
                    continue;
                if (dir == dirEmbed)
                    return dir; // type matching embedding direction found
                dirOpposite = dir;
            }
            // return ON if no strong type found, or class opposite to dirEmbed
            return dirOpposite;
        }



        private BidiDirection ClassBeforePair(BracketPair pairedLocation)
        {
            for (var ich = pairedLocation.GetOpener() - 1; ich >= 0; --ich)
            {
                var dir = GetStrongTypeN0(ich);
                if (dir != ON)
                    return dir;
            }
            // no strong types found, return sos
            return _sos;
        }

        /// <summary>
        /// Implement rule N0 for a single bracket pair
        /// </summary>
        /// <param name="pairedLocation">a bracket pair</param>
        /// <param name="dirEmbed">the embedding direction</param>
        private void AssignBracketType(BracketPair pairedLocation, BidiDirection dirEmbed)
        {
            // rule "N0, a", inspect contents of pair
            var dirPair = ClassifyPairContent(pairedLocation, dirEmbed);

            // dirPair is now L, R, or N (no strong type found)

            // the following logical tests are performed out of order compared to
            // the statement of the rules but yield the same results
            if (dirPair == ON)
                return; // case "d" - nothing to do

            if (dirPair != dirEmbed)
            {
                // case "c": strong type found, opposite - check before (c.1)
                dirPair = ClassBeforePair(pairedLocation);
                if (dirPair == dirEmbed || dirPair == ON)
                {
                    // no strong opposite type found before - use embedding (c.2)
                    dirPair = dirEmbed;
                }
            }
            // else: case "b", strong type found matching embedding,
            // no explicit action needed, as dirPair is already set to embedding
            // direction

            // set the bracket types to the type found
            SetBracketsToType(pairedLocation, dirPair);
        }

        private void SetBracketsToType(BracketPair pairedLocation, BidiDirection dirPair)
        {
            CodesIsolatedRun[pairedLocation.GetOpener()] = dirPair;
            CodesIsolatedRun[pairedLocation.GetCloser()] = dirPair;
        }

        // this implements rule N0 for a list of pairs
        public void ResolveBrackets(BidiDirection dirEmbed)
        {
            foreach (var pair in _pairPositions)
            {
                AssignBracketType(pair, dirEmbed);
            }
        }

        /// <summary>
        /// runAlgorithm - runs the paired bracket part of the UBA algorithm
        /// </summary>
        /// <param name="indexes">indexes into the original string</param>
        /// <param name="codes">bidi classes(directional codes) for each character in the original string</param>
        /// <param name="pairTypes">array of paired bracket types for each character in the original string</param>
        /// <param name="pairValues">array of unique integers identifying which pair of brackets (or canonically equivalent set) a bracket character
        ///  belongs to.For example in the string "[Test(s)>" the characters "(" and ")" would share one value and "[" and ">"
        /// share another(assuming that "]" and ">" are canonically equivalent).  Characters that have pairType = n might always get pairValue = 0.
        /// The actual values are no important as long as they are unique, so one way to assign them is to use the code position value for
        /// the closing element of a paired set for both opening and closing character - paying attention to first applying canonical decomposition.
        /// </param>
        /// <param name="sos">direction for sos</param>
        /// <param name="level">the embedding level</param>
        public void ResolvePairedBrackets(int[] indexes, BidiDirection[] codes, BracketType[] pairTypes,
                int[] pairValues, BidiDirection sos, sbyte level)
        {
            var dirEmbed = 1 == (level & 1) ? R
                    : L;
            _sos = sos;
            _indexes = indexes;
            CodesIsolatedRun = codes;
            LocateBrackets(pairTypes, pairValues);
            ResolveBrackets(dirEmbed);
        }

        /// <summary>
        /// Entry point for testing the BPA algorithm in isolation.Does not use an indexes
        /// array for indirection.Actual work is carried out by resolvePairedBrackets.
        /// </summary>
        /// <param name="codes">bidi classes (directional codes) for each character</param>
        /// <param name="pairTypes">array of paired bracket type values for each character</param>
        /// <param name="pairValues">array of unique integers identifying which bracket pair see resolvePairedBrackets for details.</param>
        /// <param name="sos">direction for sos</param>
        /// <param name="level">the embedding level</param>
        public void RunAlgorithm(BidiDirection[] codes, BracketType[] pairTypes,
                int[] pairValues, BidiDirection sos, sbyte level)
        {

            // dummy up an indexes array that represents an identity mapping
            _indexes = new int[codes.Length];
            for (var ich = 0; ich < _indexes.Length; ich++)
                _indexes[ich] = ich;
            ResolvePairedBrackets(_indexes, codes, pairTypes, pairValues, sos, level);
        }
    }
}
