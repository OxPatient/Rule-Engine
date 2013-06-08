#region Usings

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Yea.Reflection.Emit.Enums;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.BaseClasses
{
    /// <summary>
    ///     Command base class
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        protected CommandBase()
        {
            SetupOpCodes();
        }

        #endregion

        #region Functions

        private static void SetupOpCodes()
        {
            if (ConversionOpCodes == null)
            {
                ConversionOpCodes = new Dictionary<Type, OpCode>
                    {
                        {typeof (int), OpCodes.Conv_I4},
                        {typeof (Int64), OpCodes.Conv_I8},
                        {typeof (float), OpCodes.Conv_R4},
                        {typeof (double), OpCodes.Conv_R8},
                        {typeof (uint), OpCodes.Conv_U4},
                        {typeof (UInt64), OpCodes.Conv_U8}
                    };
            }
            if (ComparisonOpCodes == null)
            {
                ComparisonOpCodes = new Dictionary<Comparison, OpCode>
                    {
                        {Comparison.Equal, OpCodes.Beq},
                        {Comparison.GreaterThan, OpCodes.Ble},
                        {Comparison.GreaterThenOrEqual, OpCodes.Blt},
                        {Comparison.LessThan, OpCodes.Bge},
                        {Comparison.LessThanOrEqual, OpCodes.Bgt},
                        {Comparison.NotEqual, OpCodes.Beq}
                    };
            }
            if (ComparisonTextEquivalent == null)
            {
                ComparisonTextEquivalent = new Dictionary<Comparison, string>
                    {
                        {Comparison.Equal, "=="},
                        {Comparison.GreaterThan, ">"},
                        {Comparison.GreaterThenOrEqual, ">="},
                        {Comparison.LessThan, "<"},
                        {Comparison.LessThanOrEqual, "<="},
                        {Comparison.NotEqual, "!="}
                    };
            }
        }

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public abstract void Setup();

        #endregion

        #region Properties

        /// <summary>
        ///     Used to store conversion opcodes
        /// </summary>
        protected static Dictionary<Type, OpCode> ConversionOpCodes { get; private set; }

        /// <summary>
        ///     Used to store comparison opcodes
        /// </summary>
        protected static Dictionary<Comparison, OpCode> ComparisonOpCodes { get; private set; }

        /// <summary>
        ///     Used to store text equivalent of comparison types
        /// </summary>
        protected static Dictionary<Comparison, string> ComparisonTextEquivalent { get; private set; }

        /// <summary>
        ///     Return value (set to null if not used by the command)
        /// </summary>
        public VariableBase Result { get; set; }

        #endregion
    }
}