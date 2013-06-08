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
    ///     Adds two variables
    /// </summary>
    public class Add : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="leftHandSide">Left variable</param>
        /// <param name="rightHandSide">Right variable</param>
        public Add(object leftHandSide, object rightHandSide)
        {
            MethodBase method = MethodBase.CurrentMethod;
            var left = leftHandSide as VariableBase;
            var right = rightHandSide as VariableBase;
            if (left == null)
                LeftHandSide = method.CreateConstant(leftHandSide);
            else
                LeftHandSide = left;
            if (right == null)
                RightHandSide = method.CreateConstant(rightHandSide);
            else
                RightHandSide = right;
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "AddLocalResult" + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    LeftHandSide.DataType);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Left hand side of the addition
        /// </summary>
        protected virtual VariableBase LeftHandSide { get; set; }

        /// <summary>
        ///     Right hand side of the addition
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
            generator.Emit(OpCodes.Add);
            Result.Save(generator);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Result).Append("=").Append(LeftHandSide).Append("+").Append(RightHandSide).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}