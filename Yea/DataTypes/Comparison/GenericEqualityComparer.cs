#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Yea.DataTypes.Comparison
{
    /// <summary>
    ///     Generic equality comparer
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Functions

        /// <summary>
        ///     Determines if the two items are equal
        /// </summary>
        /// <param name="x">Object 1</param>
        /// <param name="y">Object 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public bool Equals(T x, T y)
        {
            if (!typeof (T).IsValueType
                || (typeof (T).IsGenericType
                    && typeof (T).GetGenericTypeDefinition().IsAssignableFrom(typeof (Nullable<>))))
            {
                if (Object.Equals(x, default(T)))
                    return Object.Equals(y, default(T));
                if (Object.Equals(y, default(T)))
                    return false;
            }
            if (x.GetType() != y.GetType())
                return false;
            var enumerablex = x as IEnumerable;
            var enumerabley = y as IEnumerable;
            if (enumerablex != null && enumerabley != null)
            {
                var comparer = new GenericEqualityComparer<object>();
                IEnumerator xEnumerator = enumerablex.GetEnumerator();
                IEnumerator yEnumerator = enumerabley.GetEnumerator();
                while (true)
                {
                    bool xFinished = !xEnumerator.MoveNext();
                    bool yFinished = !yEnumerator.MoveNext();
                    if (xFinished || yFinished)
                        return xFinished & yFinished;
                    if (!comparer.Equals(xEnumerator.Current, yEnumerator.Current))
                        return false;
                }
            }
            var tempEquality = x as IEqualityComparer<T>;
            if (tempEquality != null)
                return tempEquality.Equals(y);
            var tempComparable = x as IComparable<T>;
            if (tempComparable != null)
                return tempComparable.CompareTo(y) == 0;
            var tempComparable2 = x as IComparable;
            if (tempComparable2 != null)
                return tempComparable2.CompareTo(y) == 0;
            return x.Equals(y);
        }

        /// <summary>
        ///     Get hash code
        /// </summary>
        /// <param name="obj">Object to get the hash code of</param>
        /// <returns>The object's hash code</returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}