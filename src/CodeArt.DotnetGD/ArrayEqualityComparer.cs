using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt.DotnetGD
{
    internal class ArrayEqualityComparer<T> : IEqualityComparer<T[]> where T: IEquatable<T>
    {
        internal static readonly ArrayEqualityComparer<T> Default = new ArrayEqualityComparer<T>();

        public bool Equals(T[] x, T[] y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (ReferenceEquals(x, y)) return true;
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
