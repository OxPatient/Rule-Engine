#region Usings

using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Subtracts two variables
    /// </summary>
    public class Subtract : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="leftHandSide">Left variable</param>
        /// <param name="rightHandSide">Right variable</param>
        public Subtract(object leftHandSide, object rightHandSide)
        {
            var tempLeftHandSide = leftHandSide as VariableBase;
            var tempRightHandSide = rightHandSide as VariableBase;
            if (tempLeftHandSide == null)
                LeftHandSide = MethodBase.CurrentMethod.CreateConstant(leftHandSide);
            else
                LeftHandSide = tempLeftHandSide;
            if (tempRightHandSide == null)
                RightHandSide = MethodBase.CurrentMethod.CreateConstant(rightHandSide);
            else
                RightHandSide = tempRightHandSide;
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "SubtractLocalResult" + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    LeftHandSide.DataType);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Left hand side of the subtraction
        /// </summary>
        protected virtual VariableBase LeftHandSide { get; set; }

        /// <summary>
        ///     Right hand side of the subtraction
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
            if (LeftHandSide is FieldBuilder || LeftHandSide is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            LeftHandSide.Load(generator);
            if (RightHandSide is FieldBuilder || RightHandSide is IPropertyBuilder)
                generator.Emit(OpCodes.Ldarg_0);
            RightHandSide.Load(generator);
            if (LeftHandSide.DataType != RightHandSide.DataType)
            {
                if (ConversionOpCodes.ContainsKey(LeftHandSide.DataType))
                {
                    generator.Emit(ConversionOpCodes[LeftHandSide.DataType]);
                }
            }
            generator.Emit(OpCodes.Sub);
            Result.Save(generator);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Result).Append("=").Append(LeftHandSide).Append("-").Append(RightHandSide).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}