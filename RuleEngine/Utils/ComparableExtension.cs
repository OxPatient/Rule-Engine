#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Yea.RuleEngine.Utils
{
    public static class ComparableExtension
    {
        #region Between

        /// <summary>
        ///     判断值是否在指定范围之内
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">待检测的值</param>
        /// <param name="minValue">最小边界</param>
        /// <param name="maxValue">最大边界</param>
        /// <param name="includeBound">是否包括上下边界，默认不包括</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        /// <example>
        ///     var value = 5;
        ///     if(value.IsBetween(1, 10, null)) {
        ///     // ...
        ///     }
        /// </example>
        public static bool IsBetween<T>(this T value, T minValue, T maxValue, bool includeBound = false,
                                        IComparer<T> comparer = null) where T : IComparable<T>
        {
            comparer = comparer ?? Comparer<T>.Default;
            int minMaxCompare = comparer.Compare(minValue, maxValue);

            if (minMaxCompare < 0)
            {
                if (includeBound)
                    return ((comparer.Compare(value, minValue) >= 0) && (comparer.Compare(value, maxValue) <= 0));
                return ((comparer.Compare(value, minValue) > 0) && (comparer.Compare(value, maxValue) < 0));
            }
            if (minMaxCompare == 0)
            {
                if (includeBound)
                    return (comparer.Compare(value, minValue) == 0);
                return false;
            }
            if (includeBound)
                return ((comparer.Compare(value, maxValue) >= 0) && (comparer.Compare(value, minValue) <= 0));
            return ((comparer.Compare(value, maxValue) > 0) && (comparer.Compare(value, minValue) < 0));
        }

        /// <summary>
        ///     确保值是否在指定范围之内
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">待检测的值</param>
        /// <param name="minValue">最小边界</param>
        /// <param name="maxValue">最大边界</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="includeBound">是否包括上下边界</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static T EnsureBetween<T>(this T value, T minValue, T maxValue, T defaultValue, bool includeBound = false,
                                         IComparer<T> comparer = null) where T : IComparable<T>
        {
            return value.IsBetween(minValue, maxValue, includeBound, comparer) ? value : defaultValue;
        }

        #endregion

        #region MaxWith

        /// <summary>
        ///     判断值是否大于最小值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="includeEqual">是否包括等于，默认不包括</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static bool IsMaxWith<T>(this T value, T minValue, bool includeEqual = false,
                                        IComparer<T> comparer = null)
            where T : IComparable<T>
        {
            comparer = comparer ?? Comparer<T>.Default;

            int compareValue = comparer.Compare(value, minValue);

            if (includeEqual)
                return compareValue >= 0;
            return compareValue > 0;
        }

        /// <summary>
        ///     确保值大于最小值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="includeEqual">是否包括等于，默认不包括</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static T EnsureMaxWith<T>(this T value, T minValue, T defaultValue, bool includeEqual = false,
                                         IComparer<T> comparer = null) where T : IComparable<T>
        {
            return IsMaxWith(value, minValue, includeEqual, comparer) ? value : defaultValue;
        }

        #endregion

        #region MinWith

        /// <summary>
        ///     判断值是否小于最大值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="includeEqual">是否包括等于</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static bool IsMinWith<T>(this T value, T maxValue, bool includeEqual = false,
                                        IComparer<T> comparer = null)
            where T : IComparable<T>
        {
            comparer = comparer ?? Comparer<T>.Default;

            int compareValue = comparer.Compare(value, maxValue);

            if (includeEqual)
                return compareValue <= 0;
            return compareValue < 0;
        }

        /// <summary>
        ///     确保值小于最大值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="includeEqual">是否包括等于，默认不包括</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static T EnsureMinWith<T>(this T value,
                                         T maxValue,
                                         T defaultValue,
                                         bool includeEqual = false,
                                         IComparer<T> comparer = null) where T : IComparable<T>
        {
            return IsMinWith(value, maxValue, includeEqual, comparer) ? value : defaultValue;
        }

        #endregion

        #region Compare

        /// <summary>
        ///     比较
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="x">值1</param>
        /// <param name="y">值2</param>
        /// <returns>-1表示小于，0表示等于，1表示大于。</returns>
        public static int CompareTo<T>(this T x, T y) where T : IComparable
        {
            if (!typeof (T).IsValueType ||
                (typeof (T).IsGenericType && typeof (T).GetGenericTypeDefinition().IsAssignableFrom(typeof (Nullable<>))))
            {
                if (Equals(x, default(T)))
                    return Equals(y, default(T)) ? 0 : -1;
                if (Equals(y, default(T)))
                    return -1;
            }
            if (x.GetType() != y.GetType())
                return -1;
            if (x is IComparable<T>)
                return ((IComparable<T>) x).CompareTo(y);
            return x.CompareTo(y);
        }

        #endregion
    }
}