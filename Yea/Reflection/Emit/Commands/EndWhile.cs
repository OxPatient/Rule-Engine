#region Usings

using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     End While command
    /// </summary>
    public class EndWhile : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="whileCommand">While command</param>
        public EndWhile(While whileCommand)
        {
            WhileCommand = whileCommand;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     While command
        /// </summary>
        protected virtual While WhileCommand { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            MethodBase.CurrentMethod.Generator.Emit(OpCodes.Br, WhileCommand.StartWhileLabel);
            MethodBase.CurrentMethod.Generator.MarkLabel(WhileCommand.EndWhileLabel);
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            return "}\n";
        }

        #endregion
    }
}