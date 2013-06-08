#region Usings

using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Unboxes an object
    /// </summary>
    public class UnBox : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Unboxes a value to a specified type
        /// </summary>
        /// <param name="value">Value to unbox</param>
        /// <param name="valueType">Value type</param>
        public UnBox(VariableBase value, Type valueType)
        {
            Value = value;
            ValueType = valueType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Value to unbox
        /// </summary>
        public virtual VariableBase Value { get; set; }

        /// <summary>
        ///     Value type to unbox to
        /// </summary>
        public virtual Type ValueType { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the unbox statement
        /// </summary>
        public override void Setup()
        {
            if (!ValueType.IsValueType)
                throw new ArgumentException(
                    "ValueType is not a value type, unbox operations convert reference types to value types");
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "UnBoxResult" + Value.Name + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    ValueType);
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (Value is FieldBuilder || Value is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            Value.Load(generator);
            generator.Emit(OpCodes.Unbox_Any, ValueType);
            Result.Save(generator);
        }

        /// <summary>
        ///     Unbox statement as a string
        /// </summary>
        /// <returns>The unbox statement as a string</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Result).Append("=(").Append(ValueType.GetName())
                  .Append(")").Append(Value).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}