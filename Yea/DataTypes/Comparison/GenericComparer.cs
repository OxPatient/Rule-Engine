#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.DataTypes.Comparison
{
    /// <summary>
    ///     Generic IComparable class
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class GenericComparer<T> : IComparer<T> where T : IComparable
    {
        #region Functions

        /// <summary>
        ///     Compares the two objects
        /// </summary>
        /// <param name="x">Object 1</param>
        /// <param name="y">Object 2</param>
        /// <returns>0 if they're equal, any other value they are not</returns>
        public int Compare(T x, T y)
        {
            if (!typeof (T).IsValueType
                || (typeof (T).IsGenericType
                    && typeof (T).GetGenericTypeDefinition().IsAssignableFrom(typeof (Nullable<>))))
            {
                if (Equals(x, default(T)))
                    return Equals(y, default(T)) ? 0 : -1;
                if (Equals(y, default(T)))
                    return -1;
            }
            if (x.GetType() != y.GetType())
                return -1;
            var tempComparable = x as IComparable<T>;
            if (tempComparable != null)
                return tempComparable.CompareTo(y);
            return x.CompareTo(y);
        }

        #endregion
    }
}