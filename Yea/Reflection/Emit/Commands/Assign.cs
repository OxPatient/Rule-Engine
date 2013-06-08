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
    ///     Assignment command
    /// </summary>
    public class Assign : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="value">Value to store</param>
        public Assign(VariableBase leftHandSide, object value)
        {
            if (leftHandSide == null)
                throw new ArgumentNullException("leftHandSide");
            LeftHandSide = leftHandSide;
            var tempValue = value as VariableBase;
            RightHandSide = tempValue == null ? MethodBase.CurrentMethod.CreateConstant(value) : tempValue;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Left hand side of the assignment
        /// </summary>
        protected virtual VariableBase LeftHandSide { get; set; }

        /// <summary>
        ///     Value to assign
        /// </summary>
        protected virtual VariableBase RightHandSide { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (RightHandSide.DataType.IsValueType
                && !LeftHandSide.DataType.IsValueType)
            {
                RightHandSide = MethodBase.CurrentMethod.Box(RightHandSide);
            }
            else if (!RightHandSide.DataType.IsValueType
                     && LeftHandSide.DataType.IsValueType)
            {
                RightHandSide = MethodBase.CurrentMethod.UnBox(RightHandSide, LeftHandSide.DataType);
            }
            else if (!RightHandSide.DataType.IsValueType
                     && !LeftHandSide.DataType.IsValueType
                     && RightHandSide.DataType != LeftHandSide.DataType)
            {
                RightHandSide = MethodBase.CurrentMethod.Cast(RightHandSide, LeftHandSide.DataType);
            }
            if (LeftHandSide is FieldBuilder || LeftHandSide is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            if (RightHandSide is FieldBuilder || RightHandSide is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            RightHandSide.Load(generator);
            if (RightHandSide.DataType != LeftHandSide.DataType)
            {
                if (ConversionOpCodes.ContainsKey(LeftHandSide.DataType))
                {
                    generator.Emit(ConversionOpCodes[LeftHandSide.DataType]);
                }
            }
            LeftHandSide.Save(generator);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(LeftHandSide).Append("=").Append(RightHandSide).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}