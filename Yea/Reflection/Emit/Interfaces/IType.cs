#region Usings

using System;

#endregion

namespace Yea.Reflection.Emit.Interfaces
{
    /// <summary>
    ///     Type interface
    /// </summary>
    public interface IType
    {
        #region Functions

        /// <summary>
        ///     Creates the type
        /// </summary>
        /// <returns>The generated type defined by the object</returns>
        Type Create();

        #endregion
    }
}