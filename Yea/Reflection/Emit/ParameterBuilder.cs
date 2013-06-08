#region Usings

using System;
using System.Globalization;
using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Used to define a parameter
    /// </summary>
    public class ParameterBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="number">Position in parameter order</param>
        public ParameterBuilder(Type parameterType, int number)
        {
            if (number == -1)
            {
                Name = "value";
                Number = 1;
                DataType = parameterType;
                return;
            }
            if (number == 0)
            {
                Name = "this";
                Number = 0;
                DataType = null;
                return;
            }
            Name = "Parameter" + number.ToString(CultureInfo.InvariantCulture);
            Number = number;
            DataType = parameterType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Order in the parameter list
        /// </summary>
        public virtual int Number { get; protected set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Loads from the parameter
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldarg, Number);
        }

        /// <summary>
        ///     Saves to the parameter
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
            generator.Emit(OpCodes.Starg, Number);
        }

        #endregion

        #region Overridden Function

        /// <summary>
        ///     Outputs the parameter as a string
        /// </summary>
        /// <returns>The parameter</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        ///     Increments by one
        /// </summary>
        /// <param name="left">Parameter to increment</param>
        /// <returns>The parameter</returns>
        public static ParameterBuilder operator ++(ParameterBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        /// <summary>
        ///     Decrements by one
        /// </summary>
        /// <param name="left">Parameter to decrement</param>
        /// <returns>The parameter</returns>
        public static ParameterBuilder operator --(ParameterBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion
    }
}