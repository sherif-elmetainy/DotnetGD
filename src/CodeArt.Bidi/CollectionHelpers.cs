using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.Bidi
{
    /// <summary>
    ///  Helper classes to emulate behaviors of java collections in dotnet
    /// </summary>
    internal static class CollectionHelpers
    {
        public static T[] ArrayCopyOf<T>(this T[] array, int newLength)
        {
            var newArray = new T[newLength];
            if (newLength > 0 && array.Length > 0)
            {
                Array.Copy(array, 0, newArray, 0, Math.Min(newLength, array.Length));
            }
            return newArray;
        }

        public static string CollectionToString<T>(this IEnumerable<T> collection)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            var first = true;
            foreach (var item in collection)
            {
                if (!first)
                    sb.Append(", ");
                sb.Append(ReferenceEquals(item, collection) ? "(this collection)" : item?.ToString() ?? "");
                first = false;
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
