#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea
{
    /// <summary>
    ///     参数验证类
    /// </summary>
    public static class Guard
    {
        private const string Msg = "断言失败";

        /// <summary>
        ///     确保参数值不为null
        /// </summary>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        public static void NotNull<T>(T param, string paramName)
        {
            Assert(param.IsNull(), Msg, new ArgumentNullException(paramName));
        }

        /// <summary>
        ///     确保参数值不为null，也不为参数类型默认值，对Guid非常有用
        /// </summary>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        public static void NotDefault<T>(T param, string paramName)
        {
            NotNull(param, paramName);
            Assert(param.Equals(default(T)), Msg, new ArgumentNullException(paramName));
        }

        /// <summary>
        ///     确保参数集合不为null，也不为空
        /// </summary>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        public static void NotEmpty<T>(T param, string paramName) where T : IEnumerable
        {
            NotNull(param, paramName);
            Assert(!param.GetEnumerator().MoveNext(), Msg, new ArgumentNullException(paramName));
        }

        /// <summary>
        ///     确保参数为有效的枚举值
        /// </summary>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        public static void NotEnum<TEnum>(object param, string paramName)
        {
            Assert(!typeof (TEnum).IsEnum, Msg, new NotSupportedException());
            Assert(!Enum.IsDefined(typeof (TEnum), param), Msg, new ArgumentOutOfRangeException(paramName));
        }

        /// <summary>
        ///     确保参数值在指定范围之内
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="includeBound">是否包括上下边界，默认包括</param>
        /// <param name="comparer"></param>
        public static void NotOutOfRange<T>(T param, string paramName, T minValue, T maxValue,
                                            bool includeBound = true, IComparer<T> comparer = null)
            where T : IComparable<T>
        {
            Assert(!param.IsBetween(minValue, maxValue, includeBound, comparer), Msg,
                   new ArgumentOutOfRangeException(paramName));
        }

        /// <summary>
        ///     确保参数值大于指定值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        /// <param name="minValue">最小值</param>
        /// <param name="includeEqual">是否包括等于，默认包括</param>
        /// <param name="comparer">比较器</param>
        public static void NotMaxWith<T>(T param, string paramName, T minValue, bool includeEqual = true,
                                         IComparer<T> comparer = null) where T : IComparable<T>
        {
            Assert(!param.IsMaxWith(minValue, includeEqual, comparer), Msg, new ArgumentOutOfRangeException(paramName));
        }

        /// <summary>
        ///     确保参数值小于指定值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="param">参数值</param>
        /// <param name="paramName">参数名</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="includeEqual">是否包括等于，默认包括</param>
        /// <param name="comparer">比较器</param>
        public static void NotMinWith<T>(T param, string paramName, T maxValue, bool includeEqual = true,
                                         IComparer<T> comparer = null) where T : IComparable<T>
        {
            Assert(!param.IsMinWith(maxValue, includeEqual, comparer), Msg, new ArgumentOutOfRangeException(paramName));
        }


        /// <exception cref="AssertionException">断言失败</exception>
        public static void Assert(bool assertion)
        {
            if (!assertion)
                throw new AssertionException(Msg);
        }

        /// <exception cref="AssertionException"></exception>
        public static void Assert(bool assertion, string message)
        {
            if (!assertion)
                throw new AssertionException(message);
        }

        /// <exception cref="AssertionException"></exception>
        public static void Assert(bool assertion, string message, Exception innerException)
        {
            if (!assertion)
                throw new AssertionException(message, innerException);
        }

        /// <exception cref="AssertionException">断言失败</exception>
        public static void Assert(bool assertion, Exception innerException)
        {
            if (!assertion)
                throw new AssertionException(Msg, innerException);
        }
    }


    public sealed class AssertionException : Exception
    {
        public AssertionException(string message)
            : base(message)
        {
        }

        public AssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}