#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Yea.DataTypes.Patterns
{
    /// <summary>
    ///     Helps in fluent interface design to hide
    ///     ToString, Equals, and GetHashCode
    /// </summary>
    public interface IFluentInterface
    {
        #region Functions

        /// <summary>
        ///     Hides equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        /// <summary>
        ///     Hides GetHashCode
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        ///     Hides ToString
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        ///     Hides GetType
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"),
         EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        #endregion
    }
}