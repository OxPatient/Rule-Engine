#region Usings

using System;
using System.Collections.Generic;
using Yea.DataTypes.Comparison;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     IComparable extensions
    /// </summary>
    public static class ComparableExtensions
    {
        #region Functions

        #region Between

        /// <summary>
        ///     Checks if an item is between two values
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="comparer">Comparer used to compare the values (defaults to GenericComparer)"</param>
        /// <returns>True if it is between the values, false otherwise</returns>
        public static bool Between<T>(this T value, T min, T max, IComparer<T> comparer = null)
            where T : IComparable
        {
            comparer = comparer.NullCheck(() => new GenericComparer<T>());
            return comparer.Compare(max, value) >= 0 && comparer.Compare(value, min) >= 0;
        }

        #endregion

        #region Clamp

        /// <summary>
        ///     Clamps a value between two values
        /// </summary>
        /// <param name="value">Value sent in</param>
        /// <param name="max">Max value it can be (inclusive)</param>
        /// <param name="min">Min value it can be (inclusive)</param>
        /// <param name="comparer">Comparer to use (defaults to GenericComparer)</param>
        /// <returns>The value set between Min and Max</returns>
        public static T Clamp<T>(this T value, T max, T min, IComparer<T> comparer = null)
            where T : IComparable
        {
            comparer = comparer.NullCheck(() => new GenericComparer<T>());
            if (comparer.Compare(max, value) < 0)
                return max;
            if (comparer.Compare(value, min) < 0)
                return min;
            return value;
        }

        #endregion

        #region Max

        /// <summary>
        ///     Returns the maximum value between the two
        /// </summary>
        /// <param name="inputB">Input B</param>
        /// <param name="comparer">Comparer to use (defaults to GenericComparer)</param>
        /// <returns>The maximum value</returns>
        public static T Max<T>(T inputB, IComparer<T> comparer = null)
            where T : IComparable
        {
            return Max(default(T), inputB, comparer);
        }

        /// <summary>
        ///     Returns the maximum value between the two
        /// </summary>
        /// <param name="inputA">Input A</param>
        /// <param name="inputB">Input B</param>
        /// <param name="comparer">Comparer to use (defaults to GenericComparer)</param>
        /// <returns>The maximum value</returns>
        public static T Max<T>(this T inputA, T inputB, IComparer<T> comparer = null)
            where T : IComparable
        {
            comparer = comparer.NullCheck(() => new GenericComparer<T>());
            return comparer.Compare(inputA, inputB) < 0 ? inputB : inputA;
        }

        #endregion

        #region Min

        /// <summary>
        ///     Returns the minimum value between the two
        /// </summary>
        /// <param name="inputA">Input A</param>
        /// <param name="inputB">Input B</param>
        /// <param name="comparer">Comparer to use (defaults to GenericComparer)</param>
        /// <returns>The minimum value</returns>
        public static T Min<T>(this T inputA, T inputB, IComparer<T> comparer = null)
            where T : IComparable
        {
            comparer = comparer.NullCheck(() => new GenericComparer<T>());
            return comparer.Compare(inputA, inputB) > 0 ? inputB : inputA;
        }

        #endregion

        #endregion
    }
}