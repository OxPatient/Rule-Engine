#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Yea.DataTypes.ExtensionMethods;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining types
    /// </summary>
    public class TypeBuilder : IType
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="assembly">Assembly to generate the type within</param>
        /// <param name="name">name of the type</param>
        /// <param name="interfaces">Interfaces that the type implements</param>
        /// <param name="attributes">attributes for the type (public, private, etc.)</param>
        /// <param name="baseClass">Base class for the type</param>
        public TypeBuilder(Assembly assembly, string name, IEnumerable<Type> interfaces,
                           Type baseClass, TypeAttributes attributes)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Assembly = assembly;
            Name = name;
            Interfaces = new List<Type>();
            if (interfaces != null)
                Interfaces.Add(interfaces);
            BaseClass = baseClass;
            Attributes = attributes;
            Methods = new List<IMethodBuilder>();
            Fields = new List<FieldBuilder>();
            Properties = new List<IPropertyBuilder>();
            Constructors = new List<IMethodBuilder>();
            Builder = assembly.Module.DefineType(assembly.Name + "." + name, attributes, baseClass,
                                                 Interfaces.ToArray(x => x));
        }

        #endregion

        #region Functions

        #region Create

        /// <summary>
        ///     Creates the type
        /// </summary>
        /// <returns>The type defined by this TypeBuilder</returns>
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

        #region CreateMethod

        /// <summary>
        ///     Creates a method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="attributes">attributes for the method (public, virtual, etc.)</param>
        /// <param name="returnType">Return type</param>
        /// <param name="parameterTypes">Parameter types</param>
        /// <returns>Method builder for the method</returns>
        public virtual IMethodBuilder CreateMethod(string name,
                                                   MethodAttributes attributes =
                                                       MethodAttributes.Public | MethodAttributes.Virtual,
                                                   Type returnType = null, IEnumerable<Type> parameterTypes = null)
        {
            var returnValue = new MethodBuilder(this, name, attributes, parameterTypes, returnType);
            Methods.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateField

        /// <summary>
        ///     Creates a field
        /// </summary>
        /// <param name="name">name of the field</param>
        /// <param name="fieldType">Type of the field</param>
        /// <param name="attributes">attributes for the field (public, private, etc.)</param>
        /// <returns>Field builder for the field</returns>
        public virtual FieldBuilder CreateField(string name, Type fieldType,
                                                FieldAttributes attributes = FieldAttributes.Public)
        {
            var returnValue = new FieldBuilder(this, name, fieldType, attributes);
            Fields.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateProperty

        /// <summary>
        ///     Creates a property
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="propertyType">Type of the property</param>
        /// <param name="attributes">attributes for the property (special name, etc.)</param>
        /// <param name="getMethodAttributes">Get method's attributes (public, private, etc.)</param>
        /// <param name="setMethodAttributes">Set method's attributes (public, private, etc.)</param>
        /// <param name="parameters">Parameter types</param>
        /// <returns>Property builder for the property</returns>
        public virtual IPropertyBuilder CreateProperty(string name, Type propertyType,
                                                       PropertyAttributes attributes = PropertyAttributes.SpecialName,
                                                       MethodAttributes getMethodAttributes =
                                                           MethodAttributes.Public | MethodAttributes.Virtual,
                                                       MethodAttributes setMethodAttributes =
                                                           MethodAttributes.Public | MethodAttributes.Virtual,
                                                       IEnumerable<Type> parameters = null)
        {
            var returnValue = new PropertyBuilder(this, name, attributes,
                                                  getMethodAttributes, setMethodAttributes, propertyType, parameters);
            Properties.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateDefaultProperty

        /// <summary>
        ///     Creates a default property (ex int Property{get;set;}
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="propertyType">Type of the property</param>
        /// <param name="attributes">attributes for the property (special name, etc.)</param>
        /// <param name="getMethodAttributes">Get method's attributes (public, private, etc.)</param>
        /// <param name="setMethodAttributes">Set method's attributes (public, private, etc.)</param>
        /// <param name="parameters">Parameter types</param>
        /// <returns>Property builder for the property</returns>
        public virtual IPropertyBuilder CreateDefaultProperty(string name, Type propertyType,
                                                              PropertyAttributes attributes =
                                                                  PropertyAttributes.SpecialName,
                                                              MethodAttributes getMethodAttributes =
                                                                  MethodAttributes.Public | MethodAttributes.Virtual,
                                                              MethodAttributes setMethodAttributes =
                                                                  MethodAttributes.Public | MethodAttributes.Virtual,
                                                              IEnumerable<Type> parameters = null)
        {
            var returnValue = new DefaultPropertyBuilder(this, name, attributes,
                                                         getMethodAttributes, setMethodAttributes, propertyType,
                                                         parameters);
            Properties.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateConstructor

        /// <summary>
        ///     Creates a constructor
        /// </summary>
        /// <param name="attributes">attributes for the constructor (public, private, etc.)</param>
        /// <param name="parameterTypes">The types for the parameters</param>
        /// <param name="callingConventions">The calling convention used</param>
        /// <returns>Constructor builder for the constructor</returns>
        public virtual IMethodBuilder CreateConstructor(MethodAttributes attributes = MethodAttributes.Public,
                                                        IEnumerable<Type> parameterTypes = null,
                                                        CallingConventions callingConventions =
                                                            CallingConventions.Standard)
        {
            var returnValue = new ConstructorBuilder(this, attributes, parameterTypes, callingConventions);
            Constructors.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateDefaultConstructor

        /// <summary>
        ///     Creates a default constructor
        /// </summary>
        /// <param name="attributes">attributes for the constructor (public, private, etc.)</param>
        /// <returns>Constructor builder for the constructor</returns>
        public virtual IMethodBuilder CreateDefaultConstructor(MethodAttributes attributes = MethodAttributes.Public)
        {
            var returnValue = new DefaultConstructorBuilder(this, attributes);
            Constructors.Add(returnValue);
            return returnValue;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     The type defined by this TypeBuilder (filled once Create is called)
        /// </summary>
        public Type DefinedType { get; protected set; }

        /// <summary>
        ///     List of methods defined by this TypeBuilder
        ///     (does not include methods defined in base classes unless overridden)
        /// </summary>
        public ICollection<IMethodBuilder> Methods { get; private set; }

        /// <summary>
        ///     List of fields defined by the TypeBuilder
        ///     (does not include fields defined in base classes)
        /// </summary>
        public ICollection<FieldBuilder> Fields { get; private set; }

        /// <summary>
        ///     List of properties defined by the TypeBuilder
        ///     (does not include properties defined in base classes)
        /// </summary>
        public ICollection<IPropertyBuilder> Properties { get; private set; }

        /// <summary>
        ///     Constructors defined by the TypeBuilder
        /// </summary>
        public ICollection<IMethodBuilder> Constructors { get; private set; }

        /// <summary>
        ///     List of interfaces used by this type
        /// </summary>
        public ICollection<Type> Interfaces { get; private set; }

        /// <summary>
        ///     Base class used by this type
        /// </summary>
        public Type BaseClass { get; protected set; }

        /// <summary>
        ///     Builder used by this type
        /// </summary>
        public System.Reflection.Emit.TypeBuilder Builder { get; protected set; }

        /// <summary>
        ///     TypeAttributes for this type
        /// </summary>
        public TypeAttributes Attributes { get; protected set; }

        /// <summary>
        ///     name of this type
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        ///     Assembly builder
        /// </summary>
        protected Assembly Assembly { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Writes out the type builder to a readable string
        /// </summary>
        /// <returns>Code version of the type builder</returns>
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
            output.Append("class ");
            output.Append(nameParts[nameParts.Length - 1]);
            string seperator = " : ";
            if (BaseClass != null)
            {
                output.Append(seperator).Append(BaseClass.Name);
                seperator = ", ";
            }
            foreach (var Interface in Interfaces)
            {
                output.Append(seperator).Append(Interface.Name);
                seperator = ", ";
            }
            output.Append("\n{");
            Constructors.ForEach(x => output.Append(x.ToString()));
            Methods.ForEach(x => output.Append(x.ToString()));
            Properties.ForEach(x => output.Append(x.GetDefinition()));
            Fields.ForEach(x => output.Append(x.GetDefinition()));
            output.Append("\n}\n}\n\n");
            return output.ToString();
        }

        #endregion
    }
}