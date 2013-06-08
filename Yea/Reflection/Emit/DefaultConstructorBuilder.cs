#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining/creating default constructors
    /// </summary>
    public class DefaultConstructorBuilder : MethodBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="attributes">Attributes for the constructor (public, private, etc.)</param>
        public DefaultConstructorBuilder(TypeBuilder typeBuilder, MethodAttributes attributes)
        {
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");
            Type = typeBuilder;
            Attributes = attributes;
            Builder = Type.Builder.DefineDefaultConstructor(attributes);
            Generator = null;
            Parameters = new List<ParameterBuilder>();
            Parameters.Add(new ParameterBuilder(null, 0));
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
        ///     The constructor definition as a string
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
            output.Append(nameParts[nameParts.Length - 1]).Append("()");
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