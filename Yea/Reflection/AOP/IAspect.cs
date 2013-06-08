#region Usings

using System;
using System.Collections.Generic;
using Yea.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.AOP
{
    /// <summary>
    ///     Aspect interface
    /// </summary>
    public interface IAspect
    {
        #region Functions

        /// <summary>
        ///     Used to insert IL code at the beginning of the method
        /// </summary>
        /// <param name="method">Overridding Method</param>
        /// <param name="baseType">Base type</param>
        void SetupStartMethod(IMethodBuilder method, Type baseType);

        /// <summary>
        ///     Used to insert IL code at the end of the method
        /// </summary>
        /// <param name="method">Overridding Method</param>
        /// <param name="baseType">Base type</param>
        /// <param name="returnValue">Local holder for the value returned by the function</param>
        void SetupEndMethod(IMethodBuilder method, Type baseType, VariableBase returnValue);

        /// <summary>
        ///     Used to insert IL code within the catch portion of the try/catch portion of the method
        /// </summary>
        /// <param name="method">Overridding Method</param>
        /// <param name="baseType">Base type</param>
        void SetupExceptionMethod(IMethodBuilder method, Type baseType);

        /// <summary>
        ///     Used to hook into the object once it has been created
        /// </summary>
        /// <param name="Object">Object created by the system</param>
        void Setup(object Object);

        /// <summary>
        ///     Used to set up any interfaces, extra fields, methods, etc. prior to overridding any methods.
        /// </summary>
        /// <param name="typeBuilder">Type builder object</param>
        void SetupInterfaces(TypeBuilder typeBuilder);

        #endregion

        #region Properties

        /// <summary>
        ///     List of interfaces that need to be injected by this aspect
        /// </summary>
        ICollection<Type> InterfacesUsing { get; }

        #endregion
    }
}