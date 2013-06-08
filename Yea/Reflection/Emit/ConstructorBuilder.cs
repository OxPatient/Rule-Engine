#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining/creating constructors
    /// </summary>
    public class ConstructorBuilder : MethodBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="attributes">Attributes for the constructor (public, private, etc.)</param>
        /// <param name="parameters">Parameter types for the constructor</param>
        /// <param name="callingConventions">Calling convention for the constructor</param>
        public ConstructorBuilder(TypeBuilder typeBuilder, MethodAttributes attributes,
                                  IEnumerable<Type> parameters, CallingConventions callingConventions)
        {
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");
            Type = typeBuilder;
            Attributes = attributes;
            Parameters = new List<ParameterBuilder>();
            Parameters.Add(new ParameterBuilder(null, 0));
            if (parameters != null)
            {
                int x = 1;
                foreach (var parameterType in parameters)
                {
                    Parameters.Add(new ParameterBuilder(parameterType, x));
                    ++x;
                }
            }
            CallingConventions = callingConventions;
            Builder = Type.Builder.DefineConstructor(attributes, callingConventions,
                                                     (parameters != null && parameters.Count() > 0)
                                                         ? parameters.ToArray()
                                                         : System.Type.EmptyTypes);
            Generator = Builder.GetILGenerator();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Calling conventions for the constructor
        /// </summary>
        public virtual CallingConventions CallingConventions { get; protected set; }

        /// <summary>
        ///     Constructor builder
        /// </summary>
        public virtual System.Reflection.Emit.ConstructorBuilder Builder { get; protected set; }

        /// <summary>
        ///     Type builder
        /// </summary>
        protected virtual TypeBuilder Type { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     The definition of the constructor as a string
        /// </summary>
        /// <returns>The constructor as a string</returns>
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
            if ((Attributes & MethodAttributes.Virtual) > 0)
                output.Append("virtual ");
            else if ((Attributes & MethodAttributes.Abstract) > 0)
                output.Append("abstract ");
            else if ((Attributes & MethodAttributes.HideBySig) > 0)
                output.Append("override ");

            string[] splitter = {"."};
            string[] nameParts = Type.Name.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            output.Append(nameParts[nameParts.Length - 1]).Append("(");

            string seperator = "";
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    if (parameter.Number != 0)
                    {
                        output.Append(seperator).Append(parameter.GetDefinition());
                        seperator = ",";
                    }
                }
            }
            output.Append(")");
            output.Append("\n{\n");
            foreach (var command in Commands)
            {
                output.Append(command);
            }
            output.Append("}\n\n");

            return output.ToString();
        }

        #endregion
    }
}