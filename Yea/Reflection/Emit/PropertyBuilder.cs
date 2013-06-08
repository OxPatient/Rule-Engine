#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Yea.DataTypes.ExtensionMethods;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;
using MethodBase = Yea.Reflection.Emit.BaseClasses.MethodBase;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining a property
    /// </summary>
    public class PropertyBuilder : VariableBase, IPropertyBuilder
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
        public PropertyBuilder(TypeBuilder typeBuilder,
                               string name,
                               PropertyAttributes attributes,
                               MethodAttributes getMethodAttributes,
                               MethodAttributes setMethodAttributes,
                               Type propertyType,
                               IEnumerable<Type> parameters)
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
            Builder = Type.Builder.DefineProperty(name, attributes, propertyType,
                                                  (parameters != null && parameters.Count() > 0)
                                                      ? parameters.ToArray()
                                                      : System.Type.EmptyTypes);
            GetMethod = new MethodBuilder(Type, "get_" + name, getMethodAttributes, parameters, propertyType);
            var setParameters = new List<Type>();
            if (parameters != null)
                setParameters.AddRange(parameters);
            setParameters.Add(propertyType);
            SetMethod = new MethodBuilder(Type, "set_" + name, setMethodAttributes, setParameters, typeof (void));
            Builder.SetGetMethod(GetMethod.Builder);
            Builder.SetSetMethod(SetMethod.Builder);
        }

        #endregion

        #region Functions

        #region Load

        /// <summary>
        ///     Loads a property
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            generator.EmitCall(GetMethod.Builder.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, GetMethod.Builder, null);
        }

        #endregion

        #region Save

        /// <summary>
        ///     Saves the property
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
            generator.EmitCall(SetMethod.Builder.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, SetMethod.Builder, null);
        }

        #endregion

        #region GetDefinition

        /// <summary>
        ///     Gets the definition
        /// </summary>
        /// <returns>String version of what this object generates</returns>
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
            output.Append(" {\nget\n{\n");
            GetMethod.Commands.ForEach(x => output.Append(x.ToString()));
            output.Append("}\n\n");
            if ((SetMethodAttributes & GetMethodAttributes) != SetMethodAttributes)
            {
                if ((SetMethodAttributes & MethodAttributes.Public) > 0)
                    output.Append("public ");
                else if ((SetMethodAttributes & MethodAttributes.Private) > 0)
                    output.Append("private ");
            }
            output.Append("set\n{\n");
            SetMethod.Commands.ForEach(x => output.Append(x.ToString()));
            output.Append("}\n}\n");

            return output.ToString();
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Parameter list
        /// </summary>
        public ICollection<ParameterBuilder> Parameters { get; private set; }

        /// <summary>
        ///     Type builder
        /// </summary>
        protected TypeBuilder Type { get; set; }

        /// <summary>
        ///     Builder object
        /// </summary>
        public System.Reflection.Emit.PropertyBuilder Builder { get; protected set; }

        /// <summary>
        ///     Attribute object
        /// </summary>
        public PropertyAttributes Attributes { get; protected set; }

        /// <summary>
        ///     Get method attributes
        /// </summary>
        public MethodAttributes GetMethodAttributes { get; protected set; }

        /// <summary>
        ///     Set method attributes
        /// </summary>
        public MethodAttributes SetMethodAttributes { get; protected set; }

        /// <summary>
        ///     Get method
        /// </summary>
        public MethodBuilder GetMethod { get; protected set; }

        /// <summary>
        ///     Set method
        /// </summary>
        public MethodBuilder SetMethod { get; protected set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Returns the property name
        /// </summary>
        /// <returns>The property name</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        #region ++

        /// <summary>
        ///     Adds one to the property
        /// </summary>
        /// <param name="left">Property object</param>
        /// <returns>The property builder</returns>
        public static PropertyBuilder operator ++(PropertyBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        #endregion

        #region --

        /// <summary>
        ///     Subtracts one from the property builder
        /// </summary>
        /// <param name="left">Property builder</param>
        /// <returns>The property builder</returns>
        public static PropertyBuilder operator --(PropertyBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion

        #endregion
    }
}