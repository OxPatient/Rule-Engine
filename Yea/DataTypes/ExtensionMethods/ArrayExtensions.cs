#region Usings

using System;
using System.Linq;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Array extensions
    /// </summary>
    public static class ArrayExtensions
    {
        #region Clear

        /// <summary>
        ///     Clears the array completely
        /// </summary>
        /// <param name="array">Array to clear</param>
        /// <returns>The final array</returns>
        /// <example>
        ///     <code>
        ///  int[] TestObject = new int[] { 1, 2, 3, 4, 5, 6 };
        ///  TestObject.Clear();
        /// </code>
        /// </example>
        public static Array Clear(this Array array)
        {
            if (array.IsNull())
                return null;
            Array.Clear(array, 0, array.Length);
            return array;
        }

        /// <summary>
        ///     Clears the array completely
        /// </summary>
        /// <param name="array">Array to clear</param>
        /// <typeparam name="TArrayType">Array type</typeparam>
        /// <returns>The final array</returns>
        /// <example>
        ///     <code>
        ///  int[] TestObject = new int[] { 1, 2, 3, 4, 5, 6 };
        ///  TestObject.Clear();
        /// </code>
        /// </example>
        public static TArrayType[] Clear<TArrayType>(this TArrayType[] array)
        {
            return (TArrayType[]) ((Array) array).Clear();
        }

        #endregion

        #region Combine

        /// <summary>
        ///     Combines two arrays and returns a new array containing both values
        /// </summary>
        /// <typeparam name="TArrayType">Type of the data in the array</typeparam>
        /// <param name="array1">Array 1</param>
        /// <param name="array2">Array 2</param>
        /// <returns>A new array containing both arrays' values</returns>
        /// <example>
        ///     <code>
        ///  int[] TestObject1 = new int[] { 1, 2, 3 };
        ///  int[] TestObject2 = new int[] { 4, 5, 6 };
        ///  int[] TestObject3 = new int[] { 7, 8, 9 };
        ///  TestObject1 = TestObject1.Combine(TestObject2, TestObject3);
        /// </code>
        /// </example>
        public static TArrayType[] Combine<TArrayType>(this TArrayType[] array1, params TArrayType[][] array2)
        {
            if (array1.IsNull() && array2.IsNull())
                return null;
            int resultLength = (array1.IsNull() ? 0 : array1.Length);
            if (array2.IsNotNull())
                resultLength += array2.Sum(array => (array.IsNull() ? 0 : array.Length));
            var returnValue = new TArrayType[resultLength];
            int startPosition = 0;
            if (array1.IsNotNull())
            {
                Array.Copy(array1, returnValue, array1.Length);
                startPosition = array1.Length;
            }
            if (array2.IsNotNull())
            {
                foreach (var tempArray in array2)
                {
                    Array.Copy(tempArray, 0, returnValue, startPosition, tempArray.Length);
                    startPosition += tempArray.Length;
                }
            }
            return returnValue;
        }

        #endregion
    }
}