#region Usings

using System;
using System.ComponentModel;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     Gets the textual description of the enum if it has one. e.g.
        ///     <code>
        /// enum UserColors
        /// {
        ///     [Description("Bright Red")]
        ///     BrightRed
        /// }
        /// UserColors.BrightRed.Description();
        /// </code>
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string Description(this Enum @enum)
        {
            var type = @enum.GetType();

            var memInfo = type.GetMember(@enum.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(
                    typeof (DescriptionAttribute),
                    false);

                if (attrs.Length > 0)
                    return ((DescriptionAttribute) attrs[0]).Description;
            }

            return @enum.ToString();
        }

/*
        public static List<string> ToList(this Enum @enum)
        {
            return new List<string>(Enum.GetNames(@enum.GetType()));
        }

        public static bool Has<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (((int)(object)@enum & (int)(object)value) == (int)(object)value);
            if (enumType == typeof(long))
                return (((long)(object)@enum & (long)(object)value) == (long)(object)value);
            if (enumType == typeof(byte))
                return (((byte)(object)@enum & (byte)(object)value) == (byte)(object)value);

            throw new NotSupportedException(string.Format("Enums of type {0}", enumType.Name));
        }

        public static bool Is<T>(this Enum @enum, T value)
        {
            var enumType = Enum.GetUnderlyingType(@enum.GetType());
            if (enumType == typeof(int))
                return (int)(object)@enum == (int)(object)value;
            if (enumType == typeof(long))
                return (long)(object)@enum == (long)(object)value;
            if (enumType == typeof(byte))
                return (byte)(object)@enum == (byte)(object)value;

            throw new NotSupportedException(string.Format("Enums of type {0}", enumType.Name));
        }*/
    }
}