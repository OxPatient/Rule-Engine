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
    ///     Boxes an object
    /// </summary>
    public class Box : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value">Value to box</param>
        public Box(object value)
        {
            var tempValue = value as VariableBase;
            Value = tempValue == null ? new ConstantBuilder(value) : tempValue;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Value to box
        /// </summary>
        public virtual VariableBase Value { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            if (!Value.DataType.IsValueType)
                throw new ArgumentException(
                    "Value is not a value type, box operations convert value types to reference types");
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "BoxResult" + Value.Name + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    typeof (object));
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (Value is FieldBuilder || Value is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            Value.Load(generator);
            generator.Emit(OpCodes.Box, Value.DataType);
            Result.Save(generator);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Result).Append("=(").Append(typeof (object).GetName())
                  .Append(")").Append(Value).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}