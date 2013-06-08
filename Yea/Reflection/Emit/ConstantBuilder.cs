#region Usings

using System;
using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining a constant value
    /// </summary>
    public class ConstantBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value">Value of the constant</param>
        public ConstantBuilder(object value)
        {
            Value = value;
            if (value != null)
            {
                DataType = value.GetType();
                return;
            }
            DataType = null;
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Saves the constant
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
        }

        /// <summary>
        ///     Loads the constant
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            if (Value == null)
            {
                generator.Emit(OpCodes.Ldnull);
            }
            else if (DataType == typeof (Int32))
            {
                generator.Emit(OpCodes.Ldc_I4, (Int32) Value);
            }
            else if (DataType == typeof (Int64))
            {
                generator.Emit(OpCodes.Ldc_I8, (Int64) Value);
            }
            else if (DataType == typeof (float))
            {
                generator.Emit(OpCodes.Ldc_R4, (float) Value);
            }
            else if (DataType == typeof (double))
            {
                generator.Emit(OpCodes.Ldc_R8, (double) Value);
            }
            else if (DataType == typeof (string))
            {
                generator.Emit(OpCodes.Ldstr, (string) Value);
            }
            else if (DataType == typeof (bool))
            {
                if ((bool) Value)
                {
                    generator.Emit(OpCodes.Ldc_I4_1);
                }
                else
                {
                    generator.Emit(OpCodes.Ldc_I4_0);
                }
            }
        }

        /// <summary>
        ///     Get the definition of the constant
        /// </summary>
        /// <returns>The definition of the constant</returns>
        public override string GetDefinition()
        {
            return Value.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Value of the constant
        /// </summary>
        public virtual object Value { get; protected set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     The definition of the constant as a string
        /// </summary>
        /// <returns>The constant as a string</returns>
        public override string ToString()
        {
            if (DataType == typeof (string))
            {
                return "\"" + Value + "\"";
            }
            return Value.ToString();
        }

        #endregion
    }
}