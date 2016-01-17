// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt.DotnetGD
{
    /// <summary>
    /// Helper class for comparing content of two arrays
    /// </summary>
    /// <typeparam name="T">array element type</typeparam>
    internal class ArrayEqualityComparer<T> : IEqualityComparer<T[]> where T: IEquatable<T>
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private ArrayEqualityComparer()
        {
            
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static readonly ArrayEqualityComparer<T> Default = new ArrayEqualityComparer<T>();

        /// <summary>
        /// Compares two arrays for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>True if both arrays have the same length and the content is equal.</returns>
        public bool Equals(T[] x, T[] y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            var comparer = EqualityComparer<T>.Default;
            // ReSharper disable once LoopCanBeConvertedToQuery
            for(var i = 0; i < x.Length; i++)
            {
                if (!comparer.Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get hash code for the array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T[] obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                return obj.Aggregate(17, (current, item) => current * 31 + (item?.GetHashCode()).GetValueOrDefault());
            }
        }
    }
}
