#region Usings

using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Else command
    /// </summary>
    public class Else : CommandBase
    {
        #region Constructor

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            return "}\nelse\n{\n";
        }

        #endregion
    }
}