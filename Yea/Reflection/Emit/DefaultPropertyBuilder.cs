#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining default properties
    /// </summary>
    public class DefaultPropertyBuilder : VariableBase, IPropertyBuilder
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="typeBuilder">Type builder</param>
        /// <param name="name">Name of the property</param>
        /// <param name="attributes">Attributes for the property (public, private, etc.)</param>
        /// <param name="getMethodAttributes">Get method attributes</param>
        /// <param name="setMethodAttributes">Set method attributes</param>
        /// <param name="propertyType">Property type for the property</param>
        /// <param name="parameters">Parameter types for the property</param>
        public DefaultPropertyBuilder(TypeBuilder typeBuilder, string name,
                                      PropertyAttributes attributes, MethodAttributes getMethodAttributes,
                                      MethodAttributes setMethodAttributes,
                                      Type propertyType, IEnumerable<Type> parameters)
        {
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            Type = typeBuilder;
            Attributes = attributes;
            GetMethodAttributes = getMethodAttributes;
            SetMethodAttributes = setMethodAttributes;
            DataType = propertyType;
            Parameters = new List<ParameterBuilder>();
            if (parameters != null)
            {
                int x = 1;
                foreach (var parameter in parameters)
                {
                    Parameters.Add(new ParameterBuilder(parameter, x));
                    ++x;
                }
            }
            Field = new FieldBuilder(Type, "_" + name + "field", propertyType, FieldAttributes.Private);
            Builder = Type.Builder.DefineProperty(name, attributes, propertyType,
                                                  (parameters != null && parameters.Count() > 0)
                                                      ? parameters.ToArray()
                                                      : System.Type.EmptyTypes);
            GetMethod = new MethodBuilder(Type, "get_" + name, getMethodAttributes, parameters, propertyType);
            GetMethod.Generator.Emit(OpCodes.Ldarg_0);
            GetMethod.Generator.Emit(OpCodes.Ldfld, Field.Builder);
            GetMethod.Generator.Emit(OpCodes.Ret);
            var setParameters = new List<Type>();
            if (parameters != null)
            {
                setParameters.AddRange(parameters);
            }
            setParameters.Add(propertyType);
            SetMethod = new MethodBuilder(Type, "set_" + name, setMethodAttributes, setParameters, typeof (void));
            SetMethod.Generator.Emit(OpCodes.Ldarg_0);
            SetMethod.Generator.Emit(OpCodes.Ldarg_1);
            SetMethod.Generator.Emit(OpCodes.Stfld, Field.Builder);
            SetMethod.Generator.Emit(OpCodes.Ret);
            Builder.SetGetMethod(GetMethod.Builder);
            Builder.SetSetMethod(SetMethod.Builder);
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Gets the definition of the property
        /// </summary>
        /// <returns>The definition of the property</returns>
        public override string GetDefinition()
        {
            var output = new StringBuilder();

            output.Append("\n");
            if ((GetMethodAttributes & MethodAttributes.Public) > 0)
                output.Append("public ");
            else if ((GetMethodAttributes & MethodAttributes.Private) > 0)
                output.Append("private ");
            if ((GetMethodAttributes & MethodAttributes.Static) > 0)
                output.Append("static ");
            if ((GetMethodAttributes & MethodAttributes.Virtual) > 0)
                output.Append("virtual ");
            else if ((GetMethodAttributes & MethodAttributes.Abstract) > 0)
                output.Append("abstract ");
            else if ((GetMethodAttributes & MethodAttributes.HideBySig) > 0)
                output.Append("override ");
            output.Append(DataType.GetName());
            output.Append(" ").Append(Name);

            string splitter = "";
            if (Parameters != null && Parameters.Count > 0)
            {
                output.Append("[");
                foreach (var parameter in Parameters)
                {
                    output.Append(splitter).Append(parameter.GetDefinition());
                    splitter = ",";
                }
                output.Append("]");
            }
            output.Append(" { get; ");
            if ((SetMethodAttributes & GetMethodAttributes) != SetMethodAttributes)
            {
                if ((SetMethodAttributes & MethodAttributes.Public) > 0)
                    output.Append("public ");
                else if ((SetMethodAttributes & MethodAttributes.Private) > 0)
                    output.Append("private ");
            }
            output.Append("set; }\n");

            return output.ToString();
        }

        /// <summary>
        ///     Loads the property
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            if (GetMethod.Builder.IsVirtual)
                generator.EmitCall(OpCodes.Callvirt, GetMethod.Builder, null);
            else
                generator.EmitCall(OpCodes.Call, GetMethod.Builder, null);
        }

        /// <summary>
        ///     Saves the property
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
            if (SetMethod.Builder.IsVirtual)
                generator.EmitCall(OpCodes.Callvirt, SetMethod.Builder, null);
            else
                generator.EmitCall(OpCodes.Call, SetMethod.Builder, null);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Parameter types
        /// </summary>
        public ICollection<ParameterBuilder> Parameters { get; private set; }

        /// <summary>
        ///     Field builder
        /// </summary>
        public FieldBuilder Field { get; protected set; }

        /// <summary>
        ///     Type builder
        /// </summary>
        protected TypeBuilder Type { get; set; }

        /// <summary>
        ///     Method builder
        /// </summary>
        public System.Reflection.Emit.PropertyBuilder Builder { get; protected set; }

        /// <summary>
        ///     Attributes for the property
        /// </summary>
        public PropertyAttributes Attributes { get; protected set; }

        /// <summary>
        ///     Attributes for the get method
        /// </summary>
        public MethodAttributes GetMethodAttributes { get; protected set; }

        /// <summary>
        ///     Attributes for the set method
        /// </summary>
        public MethodAttributes SetMethodAttributes { get; protected set; }

        /// <summary>
        ///     Method builder for the get method
        /// </summary>
        public MethodBuilder GetMethod { get; protected set; }

        /// <summary>
        ///     Method builder for the set method
        /// </summary>
        public MethodBuilder SetMethod { get; protected set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     property as a string
        /// </summary>
        /// <returns>Property as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        ///     Increments the property by one
        /// </summary>
        /// <param name="left">The property to increment</param>
        /// <returns>The property</returns>
        public static DefaultPropertyBuilder operator ++(DefaultPropertyBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        /// <summary>
        ///     Decrements the property by one
        /// </summary>
        /// <param name="left">The property to decrement</param>
        /// <returns>The property</returns>
        public static DefaultPropertyBuilder operator --(DefaultPropertyBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion
    }
}