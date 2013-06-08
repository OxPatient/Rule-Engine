#region Usings

using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Enums;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Else if command
    /// </summary>
    public class ElseIf : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="endIfLabel">End if label (for this else if)</param>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        public ElseIf(Label endIfLabel, Comparison comparisonType, VariableBase leftHandSide, VariableBase rightHandSide)
        {
            EndIfLabel = endIfLabel;
            if (leftHandSide != null)
                LeftHandSide = leftHandSide;
            else
                LeftHandSide = MethodBase.CurrentMethod.CreateConstant(null);
            if (rightHandSide != null)
                RightHandSide = rightHandSide;
            else
                RightHandSide = MethodBase.CurrentMethod.CreateConstant(null);
            ComparisonType = comparisonType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     End if label
        /// </summary>
        public virtual Label EndIfLabel { get; set; }

        /// <summary>
        ///     Left hand side of the comparison
        /// </summary>
        public virtual VariableBase LeftHandSide { get; set; }

        /// <summary>
        ///     Right hand side of the comparison
        /// </summary>
        public virtual VariableBase RightHandSide { get; set; }

        /// <summary>
        ///     Comparison type
        /// </summary>
        public virtual Comparison ComparisonType { get; set; }

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
            if (ComparisonType == Comparison.Equal)
            {
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Ldc_I4_0);
            }
            generator.Emit(ComparisonOpCodes[ComparisonType], EndIfLabel);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("}\nelse if(").Append(LeftHandSide)
                  .Append(ComparisonTextEquivalent[ComparisonType])
                  .Append(RightHandSide).Append(")\n{\n");
            return output.ToString();
        }

        #endregion
    }
}