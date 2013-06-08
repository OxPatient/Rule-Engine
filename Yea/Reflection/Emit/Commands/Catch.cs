#region Usings

using System;
using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Catch block
    /// </summary>
    public class Catch : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exceptionType">Exception type</param>
        public Catch(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Exception caught in exception block
        /// </summary>
        public virtual VariableBase Exception { get; set; }

        /// <summary>
        ///     Exception type
        /// </summary>
        protected virtual Type ExceptionType { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Rethrows the error
        /// </summary>
        public virtual void Rethrow()
        {
            Exception.Load(MethodBase.CurrentMethod.Generator);
            MethodBase.CurrentMethod.Generator.Emit(OpCodes.Rethrow);
        }

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            MethodBase.CurrentMethod.Generator.BeginCatchBlock(ExceptionType);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            return "}\ncatch\n{\n";
        }

        #endregion
    }
}