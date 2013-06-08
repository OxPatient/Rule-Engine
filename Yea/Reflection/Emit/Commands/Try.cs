#region Usings

using System;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Starts a try block
    /// </summary>
    public class Try : CommandBase
    {
        #region Constructor

        #endregion

        #region Functions

        /// <summary>
        ///     Ends the try and starts a catch block
        /// </summary>
        /// <param name="exceptionType">Exception type</param>
        public virtual Catch StartCatchBlock(Type exceptionType)
        {
            return MethodBase.CurrentMethod.Catch(exceptionType);
        }

        /// <summary>
        ///     Ends the try/catch block
        /// </summary>
        public virtual void EndTryBlock()
        {
            MethodBase.CurrentMethod.EndTry();
        }

        /// <summary>
        ///     Sets up the try statement
        /// </summary>
        public override void Setup()
        {
            MethodBase.CurrentMethod.Generator.BeginExceptionBlock();
        }

        /// <summary>
        ///     The try statement as a string
        /// </summary>
        /// <returns>The try statement as a string</returns>
        public override string ToString()
        {
            return "try\n{\n";
        }

        #endregion
    }
}