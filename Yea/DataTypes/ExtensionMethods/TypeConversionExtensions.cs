#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     Extensions converting between types, checking if something is null, etc.
    /// </summary>
    public static class TypeConversionExtensions
    {
        #region Functions

        #region FormatToString

        /// <summary>
        ///     Calls the object's ToString function passing in the formatting
        /// </summary>
        /// <param name="input">Input object</param>
        /// <param name="format">Format of the output string</param>
        /// <returns>The formatted string</returns>
        public static string FormatToString(this object input, string format)
        {
            if (input.IsNull())
                return "";
            return !string.IsNullOrEmpty(format) ? (string) CallMethod("ToString", input, format) : input.ToString();
        }

        #endregion

        #region ToSQLDbType

        /// <summary>
        ///     Converts a .Net type to SQLDbType value
        /// </summary>
        /// <param name="type">.Net Type</param>
        /// <returns>The corresponding SQLDbType</returns>
        public static SqlDbType ToSqlDbType(this Type type)
        {
            return type.ToDbType().ToSqlDbType();
        }

        /// <summary>
        ///     Converts a DbType to a SqlDbType
        /// </summary>
        /// <param name="type">Type to convert</param>
        /// <returns>The corresponding SqlDbType (if it exists)</returns>
        public static SqlDbType ToSqlDbType(this DbType type)
        {
            var parameter = new SqlParameter {DbType = type};
            return parameter.SqlDbType;
        }

        #endregion

        #region ToDbType

        /// <summary>
        ///     Converts a .Net type to DbType value
        /// </summary>
        /// <param name="type">.Net Type</param>
        /// <returns>The corresponding DbType</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static DbType ToDbType(this Type type)
        {
            if (type.IsEnum) return Enum.GetUnderlyingType(type).ToDbType();
            if (type == typeof (byte)) return DbType.Byte;
            if (type == typeof (sbyte)) return DbType.SByte;
            if (type == typeof (short)) return DbType.Int16;
            if (type == typeof (ushort)) return DbType.UInt16;
            if (type == typeof (int)) return DbType.Int32;
            if (type == typeof (uint)) return DbType.UInt32;
            if (type == typeof (long)) return DbType.Int64;
            if (type == typeof (ulong)) return DbType.UInt64;
            if (type == typeof (float)) return DbType.Single;
            if (type == typeof (double)) return DbType.Double;
            if (type == typeof (decimal)) return DbType.Decimal;
            if (type == typeof (bool)) return DbType.Boolean;
            if (type == typeof (string)) return DbType.String;
            if (type == typeof (char)) return DbType.StringFixedLength;
            if (type == typeof (Guid)) return DbType.Guid;
            if (type == typeof (DateTime)) return DbType.DateTime2;
            if (type == typeof (DateTimeOffset)) return DbType.DateTimeOffset;
            if (type == typeof (byte[])) return DbType.Binary;
            if (type == typeof (byte?)) return DbType.Byte;
            if (type == typeof (sbyte?)) return DbType.SByte;
            if (type == typeof (short?)) return DbType.Int16;
            if (type == typeof (ushort?)) return DbType.UInt16;
            if (type == typeof (int?)) return DbType.Int32;
            if (type == typeof (uint?)) return DbType.UInt32;
            if (type == typeof (long?)) return DbType.Int64;
            if (type == typeof (ulong?)) return DbType.UInt64;
            if (type == typeof (float?)) return DbType.Single;
            if (type == typeof (double?)) return DbType.Double;
            if (type == typeof (decimal?)) return DbType.Decimal;
            if (type == typeof (bool?)) return DbType.Boolean;
            if (type == typeof (char?)) return DbType.StringFixedLength;
            if (type == typeof (Guid?)) return DbType.Guid;
            if (type == typeof (DateTime?)) return DbType.DateTime2;
            if (type == typeof (DateTimeOffset?)) return DbType.DateTimeOffset;
            return DbType.Int32;
        }

        /// <summary>
        ///     Converts SqlDbType to DbType
        /// </summary>
        /// <param name="type">Type to convert</param>
        /// <returns>The corresponding DbType (if it exists)</returns>
        public static DbType ToDbType(this SqlDbType type)
        {
            var parameter = new SqlParameter {SqlDbType = type};
            return parameter.DbType;
        }

        #endregion

        #region ToList

        /// <summary>
        ///     Attempts to convert the DataTable to a list of objects
        /// </summary>
        /// <typeparam name="T">Type the objects should be in the list</typeparam>
        /// <param name="data">DataTable to convert</param>
        /// <param name="creator">Function used to create each object</param>
        /// <returns>The DataTable converted to a list of objects</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static System.Collections.Generic.List<T> ToList<T>(this DataTable data, Func<T> creator = null)
            where T : new()
        {
            if (data.IsNull())
                return new List<T>();
            creator = creator.NullCheck(() => new T());
            Type type = typeof (T);
            PropertyInfo[] properties = type.GetProperties();
            var results = new System.Collections.Generic.List<T>();
            for (int x = 0; x < data.Rows.Count; ++x)
            {
                T rowObject = creator();

                for (int y = 0; y < data.Columns.Count; ++y)
                {
                    PropertyInfo property =
                        properties.FirstOrDefault(
                            z => String.Equals(z.Name, data.Columns[y].ColumnName, StringComparison.CurrentCulture));
                    if (!property.IsNull())
                        if (property != null)
                            property.SetValue(rowObject,
                                              data.Rows[x][data.Columns[y]].TryTo(property.PropertyType, null),
                                              new object[] {});
                }
                results.Add(rowObject);
            }
            return results;
        }

        #endregion

        #region ToType

        /// <summary>
        ///     Converts a SQLDbType value to .Net type
        /// </summary>
        /// <param name="type">SqlDbType Type</param>
        /// <returns>The corresponding .Net type</returns>
        public static Type ToType(this SqlDbType type)
        {
            return type.ToDbType().ToType();
        }

        /// <summary>
        ///     Converts a DbType value to .Net type
        /// </summary>
        /// <param name="type">DbType</param>
        /// <returns>The corresponding .Net type</returns>
        public static Type ToType(this DbType type)
        {
            if (type == DbType.Byte) return typeof (byte);
            if (type == DbType.SByte) return typeof (sbyte);
            if (type == DbType.Int16) return typeof (short);
            if (type == DbType.UInt16) return typeof (ushort);
            if (type == DbType.Int32) return typeof (int);
            if (type == DbType.UInt32) return typeof (uint);
            if (type == DbType.Int64) return typeof (long);
            if (type == DbType.UInt64) return typeof (ulong);
            if (type == DbType.Single) return typeof (float);
            if (type == DbType.Double) return typeof (double);
            if (type == DbType.Decimal) return typeof (decimal);
            if (type == DbType.Boolean) return typeof (bool);
            if (type == DbType.String) return typeof (string);
            if (type == DbType.StringFixedLength) return typeof (char);
            if (type == DbType.Guid) return typeof (Guid);
            if (type == DbType.DateTime2) return typeof (DateTime);
            if (type == DbType.DateTime) return typeof (DateTime);
            if (type == DbType.DateTimeOffset) return typeof (DateTimeOffset);
            if (type == DbType.Binary) return typeof (byte[]);
            return typeof (int);
        }

        #endregion

        #region ToExpando

        /// <summary>
        ///     Converts the object to a dynamic object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Object">The object to convert</param>
        /// <returns>The object as an expando object</returns>
        public static ExpandoObject ToExpando<T>(this T Object)
        {
            var returnValue = new ExpandoObject();
            Type tempType = typeof (T);
            foreach (var property in tempType.GetProperties())
            {
                ((ICollection<KeyValuePair<String, Object>>) returnValue).Add(
                    new KeyValuePair<string, object>(property.Name, property.GetValue(Object, null)));
            }
            return returnValue;
        }

        #endregion

        #region TryTo

        /// <summary>
        ///     Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <typeparam name="TR">Return type</typeparam>
        /// <param name="Object">Object to convert</param>
        /// <param name="defaultValue">Default value to return if there is an issue or it can't be converted</param>
        /// <returns>The object converted to the other type or the default value if there is an error or can't be converted</returns>
        public static TR TryTo<T, TR>(this T Object, TR defaultValue = default(TR))
        {
            return (TR) Object.TryTo(typeof (TR), defaultValue);
        }

        /// <summary>
        ///     Converts an expando object to the specified type
        /// </summary>
        /// <typeparam name="TR">Type to convert to</typeparam>
        /// <param name="Object">Object to convert</param>
        /// <param name="defaultValue">Default value in case it can't convert the expando object</param>
        /// <returns>The object as the specified type</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static TR TryTo<TR>(this ExpandoObject Object, TR defaultValue = default(TR))
            where TR : class, new()
        {
            try
            {
                var returnValue = new TR();
                Type tempType = typeof (TR);
                foreach (var property in tempType.GetProperties())
                {
                    if (((IDictionary<String, Object>) Object).ContainsKey(property.Name))
                    {
                        property.SetValue(returnValue, ((IDictionary<String, Object>) Object)[property.Name], null);
                    }
                }
                return returnValue;
            }
            catch
            {
            }
            return defaultValue;
        }

        /// <summary>
        ///     Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <param name="resultType">Result type</param>
        /// <param name="Object">Object to convert</param>
        /// <param name="defaultValue">Default value to return if there is an issue or it can't be converted</param>
        /// <returns>The object converted to the other type or the default value if there is an error or can't be converted</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static object TryTo<T>(this T Object, Type resultType, object defaultValue = null)
        {
            try
            {
                if (Object.IsNull())
                    return defaultValue;
                var objectValue = Object as string;
                if (objectValue != null && (objectValue.IsNotNull() && objectValue.Length == 0))
                    return defaultValue;
                Type objectType = Object.GetType();
                if (resultType.IsAssignableFrom(objectType))
                    return Object;
                if (resultType.IsEnum)
                    if (objectValue != null) return Enum.Parse(resultType, objectValue, true);
                if ((Object as IConvertible).IsNotNull())
                    return Convert.ChangeType(Object, resultType, CultureInfo.InvariantCulture);
                TypeConverter converter = TypeDescriptor.GetConverter(objectType);
                if (converter.CanConvertTo(resultType))
                    return converter.ConvertTo(Object, resultType);
                if (objectValue.IsNotNull())
                    return objectValue.TryTo(resultType, defaultValue);
            }
            catch
            {
            }
            return defaultValue;
        }

        #endregion

        #endregion

        #region Private Static Functions

        /// <summary>
        ///     Calls a method on an object
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="Object">Object to call the method on</param>
        /// <param name="inputVariables">(Optional)input variables for the method</param>
        /// <returns>The returned value of the method</returns>
        private static object CallMethod(string methodName, object Object, params object[] inputVariables)
        {
            if (string.IsNullOrEmpty(methodName) || Object.IsNull())
                return null;
            Type objectType = Object.GetType();
            MethodInfo method;
            if (inputVariables.IsNotNull())
            {
                var methodInputTypes = new Type[inputVariables.Length];
                for (int x = 0; x < inputVariables.Length; ++x)
                    methodInputTypes[x] = inputVariables[x].GetType();
                method = objectType.GetMethod(methodName, methodInputTypes);
                if (method != null)
                    return method.Invoke(Object, inputVariables);
            }
            method = objectType.GetMethod(methodName);
            return method.IsNull() ? null : method.Invoke(Object, null);
        }

        #endregion
    }
}