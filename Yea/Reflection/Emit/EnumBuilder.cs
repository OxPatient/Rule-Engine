#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining enums
    /// </summary>
    public class EnumBuilder : IType
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="assembly">Assembly builder</param>
        /// <param name="name">name of the enum</param>
        /// <param name="attributes">Attributes for the enum (public, private, etc.)</param>
        /// <param name="enumType">Type for the enum</param>
        public EnumBuilder(Assembly assembly, string name, Type enumType, TypeAttributes attributes)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            Assembly = assembly;
            EnumType = enumType;
            Attributes = attributes;
            Literals = new List<System.Reflection.Emit.FieldBuilder>();
            Builder = assembly.Module.DefineEnum(assembly.Name + "." + name, TypeAttributes.Public, enumType);
        }

        #endregion

        #region Functions

        #region AddLiteral

        /// <summary>
        ///     Adds a literal to the enum (an entry)
        /// </summary>
        /// <param name="name">name of the entry</param>
        /// <param name="value">Value associated with it</param>
        public virtual void AddLiteral(string name, object value)
        {
            Literals.Add(Builder.DefineLiteral(name, value));
        }

        #endregion

        #region Create

        /// <summary>
        ///     Creates the enum
        /// </summary>
        /// <returns>The type defined by this EnumBuilder</returns>
        public virtual Type Create()
        {
            if (Builder == null)
                throw new InvalidOperationException(
                    "The builder object has not been defined. Ensure that Setup is called prior to Create");
            if (DefinedType != null)
                return DefinedType;
            DefinedType = Builder.CreateType();
            return DefinedType;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Field name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Literals defined within the enum
        /// </summary>
        public ICollection<System.Reflection.Emit.FieldBuilder> Literals { get; private set; }

        /// <summary>
        ///     Field builder
        /// </summary>
        public System.Reflection.Emit.EnumBuilder Builder { get; protected set; }

        /// <summary>
        ///     Base enum type (int32, etc.)
        /// </summary>
        public Type EnumType { get; protected set; }

        /// <summary>
        ///     Type defined by this enum
        /// </summary>
        public Type DefinedType { get; protected set; }

        /// <summary>
        ///     Attributes for the enum (private, public, etc.)
        /// </summary>
        public TypeAttributes Attributes { get; protected set; }

        /// <summary>
        ///     Assembly builder
        /// </summary>
        protected Assembly Assembly { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Enum definition as a string
        /// </summary>
        /// <returns>The enum as a string</returns>
        public override string ToString()
        {
            string[] splitter = {"."};
            string[] nameParts = Name.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            var output = new StringBuilder();
            output.Append("namespace ").Append(Assembly.Name);
            for (int x = 0; x < nameParts.Length - 1; ++x)
                output.Append(".").Append(nameParts[x]);
            output.Append("\n{\n");
            output.Append((Attributes & TypeAttributes.Public) > 0 ? "public " : "private ");
            output.Append("enum ").Append(nameParts[nameParts.Length - 1]).Append("\n{");
            string seperator = "";
            foreach (var literal in Literals)
            {
                output.Append(seperator).Append("\n\t").Append(literal.Name);
                seperator = ",";
            }
            output.Append("\n}\n}\n\n");
            return output.ToString();
        }

        #endregion
    }
}