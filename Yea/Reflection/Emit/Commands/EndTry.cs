#region Usings

using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Ends a try/catch block
    /// </summary>
    public class EndTry : CommandBase
    {
        #region Constructor

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            MethodBase.CurrentMethod.Generator.EndExceptionBlock();
        }

        /// <summary>
        ///     To string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            return "}\n";
        }

        #endregion
    }
}