#region Usings

using System;

#endregion

namespace Yea.RuleEngine.Utils
{
    public static class GenericObjectExtensions
    {
        /// <summary>
        ///     �ж϶����Ƿ�Ϊnull
        /// </summary>
        /// <param name="Object">��������</param>
        public static bool IsNull(this object Object)
        {
            return Object == null || Convert.IsDBNull(Object);
        }
    }
}