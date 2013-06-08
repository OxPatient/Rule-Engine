#region Usings

using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Throws an exception
    /// </summary>
    public class Throw : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exception">Exception to throw</param>
        public Throw(VariableBase exception)
        {
            Exception = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Exception to throw
        /// </summary>
        public virtual VariableBase Exception { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the throw statement
        /// </summary>
        public override void Setup()
        {
            if (Exception is FieldBuilder || Exception is IPropertyBuilder)
                MethodBase.CurrentMethod.Generator.Emit(OpCodes.Ldarg_0);
            Exception.Load(MethodBase.CurrentMethod.Generator);
            MethodBase.CurrentMethod.Generator.Emit(OpCodes.Throw);
        }

        /// <summary>
        ///     The throw statement as a string
        /// </summary>
        /// <returns>The throw statement as a string</returns>
        public override string ToString()
        {
            return "throw " + Exception.ToString() + ";\n";
        }

        #endregion
    }
}