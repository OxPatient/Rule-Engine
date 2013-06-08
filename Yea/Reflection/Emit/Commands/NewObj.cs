#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit.Commands
{
    /// <summary>
    ///     Command for creating a new object
    /// </summary>
    public class NewObj : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="constructor">Constructor to use</param>
        /// <param name="parameters">Variables sent to the constructor</param>
        public NewObj(ConstructorInfo constructor, object[] parameters)
        {
            Constructor = constructor;
            if (parameters != null)
            {
                Parameters = new VariableBase[parameters.Length];
                for (int x = 0; x < parameters.Length; ++x)
                {
                    if (parameters[x] is VariableBase)
                        Parameters[x] = (VariableBase) parameters[x];
                    else
                        Parameters[x] = MethodBase.CurrentMethod.CreateConstant(parameters[x]);
                }
            }
            Result =
                MethodBase.CurrentMethod.CreateLocal(
                    "ObjLocal" + MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture),
                    constructor.DeclaringType);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Constructor used
        /// </summary>
        protected virtual ConstructorInfo Constructor { get; set; }

        /// <summary>
        ///     Variables sent to the Constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected virtual VariableBase[] Parameters { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            ILGenerator generator = MethodBase.CurrentMethod.Generator;
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    if (parameter is FieldBuilder || parameter is IPropertyBuilder)
                        generator.Emit(OpCodes.Ldarg_0);
                    parameter.Load(generator);
                }
            }
            generator.Emit(OpCodes.Newobj, Constructor);
            Result.Save(generator);
        }

        /// <summary>
        ///     Converts the command to the string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Result).Append(" = new ")
                  .Append(Constructor.DeclaringType.GetName())
                  .Append("(");
            string seperator = "";
            if (Parameters != null)
            {
                foreach (var variable in Parameters)
                {
                    output.Append(seperator).Append(variable);
                    seperator = ",";
                }
            }
            output.Append(");\n");
            return output.ToString();
        }

        #endregion
    }
}