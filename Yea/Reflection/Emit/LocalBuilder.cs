#region Usings

using System;
using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit
{
    /// <summary>
    ///     Helper class for defining a local variable
    /// </summary>
    public class LocalBuilder : VariableBase
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="methodBuilder">Method builder</param>
        /// <param name="name">Name of the local</param>
        /// <param name="localType">Type of the local</param>
        public LocalBuilder(IMethodBuilder methodBuilder, string name, Type localType)
        {
            if (methodBuilder == null)
                throw new ArgumentNullException("methodBuilder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
            MethodBuilder = methodBuilder;
            DataType = localType;
            Builder = methodBuilder.Generator.DeclareLocal(localType);
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Loads the local object
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Load(ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldloc, Builder);
        }

        /// <summary>
        ///     Saves the local object
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public override void Save(ILGenerator generator)
        {
            generator.Emit(OpCodes.Stloc, Builder);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Local builder
        /// </summary>
        public virtual System.Reflection.Emit.LocalBuilder Builder { get; protected set; }

        /// <summary>
        ///     Method builder
        /// </summary>
        protected virtual IMethodBuilder MethodBuilder { get; set; }

        #endregion

        #region Overridden Functions

        /// <summary>
        ///     The local item as a string
        /// </summary>
        /// <returns>The local item as a string</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Operator Functions

        /// <summary>
        ///     Increments the local object by one
        /// </summary>
        /// <param name="left">Local object to increment</param>
        /// <returns>The local object</returns>
        public static LocalBuilder operator ++(LocalBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        /// <summary>
        ///     Decrements the local object by one
        /// </summary>
        /// <param name="left">Local object to decrement</param>
        /// <returns>The local object</returns>
        public static LocalBuilder operator --(LocalBuilder left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion
    }
}