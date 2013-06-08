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
    ///     Call command
    /// </summary>
    public class Call : CommandBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="objectCallingOn">Object calling on</param>
        /// <param name="method">Method builder</param>
        /// <param name="methodCalling">Method calling on the object</param>
        /// <param name="parameters">List of parameters to send in</param>
        public Call(IMethodBuilder method, VariableBase objectCallingOn, MethodInfo methodCalling, object[] parameters)
        {
            ObjectCallingOn = objectCallingOn;
            MethodCalling = methodCalling;
            MethodCallingFrom = method;
            if (methodCalling.ReturnType != null && methodCalling.ReturnType != typeof (void))
            {
                Result =
                    method.CreateLocal(
                        methodCalling.Name + "ReturnObject" +
                        MethodBase.ObjectCounter.ToString(CultureInfo.InvariantCulture), methodCalling.ReturnType);
            }
            if (parameters != null)
            {
                Parameters = new VariableBase[parameters.Length];
                for (int x = 0; x < parameters.Length; ++x)
                {
                    if (parameters[x] is VariableBase)
                        Parameters[x] = (VariableBase) parameters[x];
                    else
                        Parameters[x] = MethodCallingFrom.CreateConstant(parameters[x]);
                }
            }
            else
            {
                Parameters = null;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="objectCallingOn">Object calling on</param>
        /// <param name="method">Method builder</param>
        /// <param name="methodCalling">Method calling on the object</param>
        /// <param name="parameters">List of parameters to send in</param>
        public Call(IMethodBuilder method, VariableBase objectCallingOn, ConstructorInfo methodCalling,
                    object[] parameters)
        {
            ObjectCallingOn = objectCallingOn;
            ConstructorCalling = methodCalling;
            MethodCallingFrom = method;
            if (parameters != null)
            {
                Parameters = new VariableBase[parameters.Length];
                for (int x = 0; x < parameters.Length; ++x)
                {
                    if (parameters[x] is VariableBase)
                        Parameters[x] = (VariableBase) parameters[x];
                    else
                        Parameters[x] = MethodCallingFrom.CreateConstant(parameters[x]);
                }
            }
            else
            {
                Parameters = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Object calling on
        /// </summary>
        protected virtual VariableBase ObjectCallingOn { get; set; }

        /// <summary>
        ///     Method calling
        /// </summary>
        protected virtual MethodInfo MethodCalling { get; set; }

        /// <summary>
        ///     Method calling
        /// </summary>
        protected virtual ConstructorInfo ConstructorCalling { get; set; }

        /// <summary>
        ///     Parameters sent in
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected virtual VariableBase[] Parameters { get; set; }

        /// <summary>
        ///     Method calling from
        /// </summary>
        protected virtual IMethodBuilder MethodCallingFrom { get; set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Sets up the command
        /// </summary>
        public override void Setup()
        {
            if (ObjectCallingOn != null)
            {
                if (ObjectCallingOn is FieldBuilder || ObjectCallingOn is IPropertyBuilder)
                    MethodCallingFrom.Generator.Emit(OpCodes.Ldarg_0);
                ObjectCallingOn.Load(MethodCallingFrom.Generator);
            }
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    if (parameter is FieldBuilder || parameter is IPropertyBuilder)
                        MethodCallingFrom.Generator.Emit(OpCodes.Ldarg_0);
                    parameter.Load(MethodCallingFrom.Generator);
                }
            }
            OpCode opCodeUsing = OpCodes.Callvirt;
            if (MethodCalling != null)
            {
                if (ObjectCallingOn != null && (!MethodCalling.IsVirtual ||
                                                (ObjectCallingOn.Name == "this" &&
                                                 MethodCalling.Name == MethodCallingFrom.Name)))
                    opCodeUsing = OpCodes.Call;
                MethodCallingFrom.Generator.EmitCall(opCodeUsing, MethodCalling, null);
                if (MethodCalling.ReturnType != null && MethodCalling.ReturnType != typeof (void))
                {
                    Result.Save(MethodCallingFrom.Generator);
                }
            }
            else if (ConstructorCalling != null)
            {
                if (!ConstructorCalling.IsVirtual ||
                    (ObjectCallingOn.Name == "this" && MethodCalling.Name == MethodCallingFrom.Name))
                    opCodeUsing = OpCodes.Call;
                MethodCallingFrom.Generator.Emit(opCodeUsing, ConstructorCalling);
            }
        }

        /// <summary>
        ///     Converts the command to a string
        /// </summary>
        /// <returns>The string version of the command</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            if (Result != null)
            {
                output.Append(Result).Append(" = ");
            }
            if (ObjectCallingOn != null)
                output.Append(ObjectCallingOn).Append(".");
            if (ObjectCallingOn.Name == "this" && MethodCallingFrom.Name == MethodCalling.Name)
            {
                output.Append("base").Append("(");
            }
            else
            {
                output.Append(MethodCalling.Name).Append("(");
            }
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