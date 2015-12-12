using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt.DotnetGD
{
    internal class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> where T: IEquatable<T>
    {
        internal static readonly CollectionEqualityComparer<T> Default = new CollectionEqualityComparer<T>();

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (ReferenceEquals(x, y)) return true;
            if (x.GetType() != y.GetType()) return false;
            using (var enX = x.GetEnumerator())
            {
                using (var enY = y.GetEnumerator())
                {
                    do
                    {
                        var rX = enX.MoveNext();
                        var rY = enX.MoveNext();
                        if (rX != rY)
                            return false;
                        if (!rX)
                            return true;
                        if (!EqualityComparer<T>.Default.Equals(enX.Current, enY.Current))
                            return false;
                    } while (true);
                }    
            }
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                return obj.Aggregate(17, (current, item) => current * 31 + (item?.GetHashCode()).GetValueOrDefault());
            }
        }
    }
}
