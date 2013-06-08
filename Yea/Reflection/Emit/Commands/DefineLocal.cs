#region Usings

using System;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Defines a local variable
    /// </summary>
    public class DefineLocal : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Local object name</param>
        /// <param name="localType">Local type</param>
        public DefineLocal(string name, Type localType)
        {
            Result = new LocalBuilder(MethodBase.CurrentMethod, name, localType);
        }

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
            var output = new StringBuilder();
            output.Append(Result.GetDefinition()).Append(";\n");
            return output.ToString();
        }

        #endregion
    }
}