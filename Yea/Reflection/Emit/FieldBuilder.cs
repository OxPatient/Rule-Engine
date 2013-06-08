#region Usings

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining a field within a type
    /// </summary>
    public class FieldBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="name">Name of the method</param>
        /// <param name="attributes">Attributes for the field (public, private, etc.)</param>
        /// <param name="fieldType">Type for the field</param>
        public FieldBuilder(TypeBuilder typeBuilder, string name, Type fieldType, FieldAttributes attributes)
        {
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            Type = typeBuilder;
            DataType = fieldType;
            Attributes = attributes;
            Builder = Type.Builder.DefineField(name, fieldType, attributes);
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Loads the field
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldfld, Builder);
        }

        /// <summary>
        ///     Saves the field
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
            generator.Emit(OpCodes.Stfld, Builder);
        }

        /// <summary>
        ///     Gets the definition of the field
        /// </summary>
        /// <returns>The field's definition</returns>
        public override string GetDefinition()
        {
            var output = new StringBuilder();

            output.Append("\n");
            if ((Attributes & FieldAttributes.Public) > 0)
                output.Append("public ");
            else if ((Attributes & FieldAttributes.Private) > 0)
                output.Append("private ");
            if ((Attributes & FieldAttributes.Static) > 0)
                output.Append("static ");
            output.Append(DataType.GetName());
            output.Append(" ").Append(Name).Append(";");

            return output.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Field builder
        /// </summary>
        public virtual System.Reflection.Emit.FieldBuilder Builder { get; protected set; }

        /// <summary>
        ///     Attributes for the field (private, public, etc.)
        /// </summary>
        public virtual FieldAttributes Attributes { get; protected set; }

        /// <summary>
        ///     Type builder
        /// </summary>
        protected virtual TypeBuilder Type { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     The field as a string
        /// </summary>
        /// <returns>The field as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        ///     Increments the field by one
        /// </summary>
        /// <param name="left">Field to increment</param>
        /// <returns>The field</returns>
        public static FieldBuilder operator ++(FieldBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        /// <summary>
        ///     Decrements the field by one
        /// </summary>
        /// <param name="left">Field to decrement</param>
        /// <returns>The field</returns>
        public static FieldBuilder operator --(FieldBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion
    }
}