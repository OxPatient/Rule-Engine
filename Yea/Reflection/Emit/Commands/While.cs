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
    ///     While command
    /// </summary>
    public class While : CommandBase
    {
        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        public While(Comparison comparisonType, VariableBase leftHandSide, VariableBase rightHandSide)
        {
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            StartWhileLabel = generator.DefineLabel();
            EndWhileLabel = generator.DefineLabel();
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
        ///     Start while label
        /// </summary>
        public virtual Label StartWhileLabel { get; set; }

        /// <summary>
        ///     End while label
        /// </summary>
        public virtual Label EndWhileLabel { get; set; }

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
        ///     Ends the while statement
        /// </summary>
        public virtual void EndWhile()
        {
            MethodBase.CurrentMethod.EndWhile(this);
        }

        /// <summary>
        ///     Sets up the while statement
        /// </summary>
        public override void Setup()
        {
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            generator.MarkLabel(StartWhileLabel);
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
            generator.Emit(ComparisonOpCodes[ComparisonType], EndWhileLabel);
        }

        /// <summary>
        ///     The while statement as a string
        /// </summary>
        /// <returns>The string representation of the while statement</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append("while(").Append(LeftHandSide)
                  .Append(ComparisonTextEquivalent[ComparisonType]).Append(RightHandSide)
                  .Append(")\n{\n");
            return output.ToString();
        }

        #endregion
    }
}