#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Yea.DataTypes.ExtensionMethods;
using Yea.Reflection.Emit.Enums;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Assembly class
    /// </summary>
    public class Assembly
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="directory">directory to save the assembly (if left blank, the assembly is run only and will not be saved)</param>
        /// <param name="type">Assembly type (exe or dll)</param>
        public Assembly(string name, string directory = "", AssemblyType type = AssemblyType.Dll)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Setup(name, directory, type);
        }

        #endregion

        #region Functions

        #region Setup

        /// <summary>
        ///     Sets up the assembly
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="directory">directory to save the assembly (if left blank, the assembly is run only and will not be saved)</param>
        /// <param name="type">Assembly type (dll or exe)</param>
        private void Setup(string name, string directory = "", AssemblyType type = AssemblyType.Dll)
        {
            Name = name;
            Directory = directory;
            Type = type;
            var assemblyName = new AssemblyName(name);
            AppDomain domain = Thread.GetDomain();
            if (!string.IsNullOrEmpty(directory))
            {
                Builder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, directory);
                Module = Builder.DefineDynamicModule(name, name + (type == AssemblyType.Dll ? ".dll" : ".exe"), true);
            }
            else
            {
                Builder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                Module = Builder.DefineDynamicModule(name);
            }
            Classes = new List<TypeBuilder>();
            Enums = new List<EnumBuilder>();
        }

        #endregion

        #region CreateType

        /// <summary>
        ///     Creates a type builder
        /// </summary>
        /// <param name="name">name of the type</param>
        /// <param name="attributes">Attributes associated with the type</param>
        /// <param name="baseClass">Base class for this type</param>
        /// <param name="interfaces">Interfaces used by this type</param>
        /// <returns>A TypeBuilder class</returns>
        public virtual TypeBuilder CreateType(string name, TypeAttributes attributes = TypeAttributes.Public,
                                              Type baseClass = null, IEnumerable<Type> interfaces = null)
        {
            var returnValue = new TypeBuilder(this, name, interfaces, baseClass, attributes);
            Classes.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region CreateEnum

        /// <summary>
        ///     Creates an enum builder
        /// </summary>
        /// <param name="name">name of the enum</param>
        /// <param name="enumBaseType">Base type of the enum (defaults to int)</param>
        /// <param name="attributes">Attributes associated with the type</param>
        /// <returns>An EnumBuilder class</returns>
        public virtual EnumBuilder CreateEnum(string name, Type enumBaseType = null,
                                              TypeAttributes attributes = TypeAttributes.Public)
        {
            if (enumBaseType == null)
                enumBaseType = typeof (int);
            var returnValue = new EnumBuilder(this, name, enumBaseType, attributes);
            Enums.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region Create

        /// <summary>
        ///     Creates all types associated with the assembly and saves the assembly to disk
        ///     if a directory is specified.
        /// </summary>
        public virtual void Create()
        {
            foreach (IType Class in Classes)
                Class.Create();
            foreach (IType Enum in Enums)
                Enum.Create();
            if (!string.IsNullOrEmpty(Directory))
                Builder.Save(Name + (Type == AssemblyType.Dll ? ".dll" : ".exe"));
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     ModuleBuilder object
        /// </summary>
        public ModuleBuilder Module { get; protected set; }

        /// <summary>
        ///     name of the assembly
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     directory of the assembly
        /// </summary>
        public string Directory { get; protected set; }

        /// <summary>
        ///     List of classes associated with this assembly
        /// </summary>
        public ICollection<TypeBuilder> Classes { get; private set; }

        /// <summary>
        ///     List of enums associated with this assembly
        /// </summary>
        public ICollection<EnumBuilder> Enums { get; private set; }

        /// <summary>
        ///     Assembly type (exe or dll)
        /// </summary>
        public AssemblyType Type { get; protected set; }

        /// <summary>
        ///     Assembly builder
        /// </summary>
        protected AssemblyBuilder Builder { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     Converts the assembly to a string
        /// </summary>
        /// <returns>The string version of the assembly</returns>
        public override string ToString()
        {
            var output = new StringBuilder();
            Enums.ForEach(x => output.Append(x.ToString()));
            Classes.ForEach(x => output.Append(x.ToString()));
            return output.ToString();
        }

        #endregion
    }
}