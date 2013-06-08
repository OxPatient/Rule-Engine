#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using Yea.DataTypes.ExtensionMethods;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining a method within a type
    /// </summary>
    public class MethodBuilder : MethodBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="name">Name of the method</param>
        /// <param name="attributes">Attributes for the method (public, private, etc.)</param>
        /// <param name="parameters">Parameter types for the method</param>
        /// <param name="returnType">Return type for the method</param>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.StartsWith(System.String,System.StringComparison)")]
        public MethodBuilder(TypeBuilder typeBuilder, string name,
                             MethodAttributes attributes, IEnumerable<Type> parameters, Type returnType)
        {
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            Type = typeBuilder;
            Attributes = attributes;
            ReturnType = (returnType == null) ? typeof (void) : returnType;
            Parameters = new List<ParameterBuilder>();
            Parameters.Add(new ParameterBuilder(null, 0));
            if (parameters != null)
            {
                int x = 1;
                if (name.StartsWith("set_", StringComparison.InvariantCulture))
                    x = -1;
                foreach (var parameterType in parameters)
                {
                    Parameters.Add(new ParameterBuilder(parameterType, x));
                    ++x;
                }
            }
            Builder = Type.Builder.DefineMethod(name, attributes, returnType,
                                                (parameters != null && parameters.Count() > 0)
                                                    ? parameters.ToArray()
                                                    : System.Type.EmptyTypes);
            Generator = Builder.GetILGenerator();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Method builder
        /// </summary>
        public virtual System.Reflection.Emit.MethodBuilder Builder { get; protected set; }

        /// <summary>
        ///     Type builder
        /// </summary>
        protected virtual TypeBuilder Type { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Outputs the method to a string
        /// </summary>
        /// <returns>The string representation of the method</returns>
        public override string ToString()
        {
            var output = new StringBuilder();

            output.Append("\n");
            if ((Attributes & MethodAttributes.Public) > 0)
                output.Append("public ");
            else if ((Attributes & MethodAttributes.Private) > 0)
                output.Append("private ");
            if ((Attributes & MethodAttributes.Static) > 0)
                output.Append("static ");
            if ((Attributes & MethodAttributes.Abstract) > 0)
                output.Append("abstract ");
            else if ((Attributes & MethodAttributes.HideBySig) > 0)
                output.Append("override ");
            else if ((Attributes & MethodAttributes.Virtual) > 0)
                output.Append("virtual ");
            output.Append(ReturnType.GetName());
            output.Append(" ").Append(Name).Append("(");

            string splitter = "";
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    if (parameter.Number != 0)
                    {
                        output.Append(splitter).Append(parameter.GetDefinition());
                        splitter = ",";
                    }
                }
            }
            output.Append(")");
            output.Append("\n{\n");
            Commands.ForEach(x => output.Append(x.ToString()));
            output.Append("}\n\n");

            return output.ToString();
        }

        #endregion
    }
}