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
    ///     Casts a class object to another class
    /// </summary>
    public class Cast : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value">Value to cast</param>
        /// <param name="valueType">Desired type to cast to</param>
        public Cast(VariableBase value, Type valueType)
        {
            Value = value;
            ValueType = valueType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Value to cast
        /// </summary>
        public virtual VariableBase Value { get; set; }

        /// <summary>
        ///     Desired type to cast to
        /// </summary>
        public virtual Type ValueType { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            if (ValueType.IsValueType)
                throw new ArgumentException(
                    "ValueType is a value type, cast operations convert reference types to other reference types");
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "CastResult" + Value.Name + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    ValueType);
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (Value is FieldBuilder || Value is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            Value.Load(generator);
            generator.Emit(OpCodes.Castclass, ValueType);
            Result.Save(generator);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
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