#region Usings

using System;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Return command
    /// </summary>
    public class Return : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="returnType">Return type</param>
        /// <param name="returnValue">Return value</param>
        public Return(Type returnType, object returnValue)
        {
            ReturnType = returnType;
            var tempReturnValue = returnValue as VariableBase;
            ReturnValue = returnValue == null || tempReturnValue == null
                              ? new ConstantBuilder(returnValue)
                              : tempReturnValue;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Return type
        /// </summary>
        protected virtual Type ReturnType { get; set; }

        /// <summary>
        ///     Return value
        /// </summary>
        protected virtual VariableBase ReturnValue { get; set; }

        #endregion

        #region Function

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (ReturnType == typeof (void) || ReturnType == null)
            {
                generator.Emit(OpCodes.Ret);
                return;
            }
            if (ReturnValue is FieldBuilder || ReturnValue is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            ReturnValue.Load(generator);
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            if (ReturnType != null && ReturnType != typeof (void))
            {
                output.Append("return ").Append(ReturnValue).Append(";\n");
            }
            return output.ToString();
        }

        #endregion
    }
}