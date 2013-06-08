#region Usings

using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     End If command
    /// </summary>
    public class EndIf : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ifCommand">If command</param>
        public EndIf(If ifCommand)
        {
            IfCommand = ifCommand;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     If command
        /// </summary>
        protected virtual If IfCommand { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            MethodBase.CurrentMethod.Generator.MarkLabel(IfCommand.EndIfLabel);
            MethodBase.CurrentMethod.Generator.MarkLabel(IfCommand.EndIfFinalLabel);
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