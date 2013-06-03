#region Usings

using System;

#endregion

namespace Yea.RuleEngine.Utils
{
    public static class GenericObjectExtensions
    {
        /// <summary>
        ///     判断对象是否为null
        /// </summary>
        /// <param name="Object">待检测对象</param>
        public static bool IsNull(this object Object)
        {
            return Object == null || Convert.IsDBNull(Object);
        }
    }
}