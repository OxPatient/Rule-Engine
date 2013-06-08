#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Commands;
using Yea.Reflection.Emit.Enums;

#endregion

namespace Yea.Reflection.Emit.Interfaces
{
    /// <summary>
    ///     Interface for methods
    /// </summary>
    public interface IMethodBuilder
    {
        #region Functions

        /// <summary>
        ///     Sets the method as the current method
        /// </summary>
        void SetCurrentMethod();

        /// <summary>
        ///     Defines a local variable
        /// </summary>
        /// <param name="name">name of the local variable</param>
        /// <param name="localType">The Type of the local variable</param>
        /// <returns>The LocalBuilder associated with the variable</returns>
        VariableBase CreateLocal(string name, Type localType);

        /// <summary>
        ///     Constant value
        /// </summary>
        /// <param name="value">Value of the constant</param>
        /// <returns>The ConstantBuilder associated with the variable</returns>
        VariableBase CreateConstant(object value);

        /// <summary>
        ///     Creates new object
        /// </summary>
        /// <param name="constructor">Constructor</param>
        /// <param name="variables">Variables to send to the constructor</param>
        VariableBase NewObj(ConstructorInfo constructor, object[] variables = null);

        /// <summary>
        ///     Creates new object
        /// </summary>
        /// <param name="objectType">object type</param>
        /// <param name="variables">Variables to send to the constructor</param>
        VariableBase NewObj(Type objectType, object[] variables = null);

        /// <summary>
        ///     Assigns the value to the left hand side variable
        /// </summary>
        /// <param name="leftHandSide">Left hand side variable</param>
        /// <param name="value">Value to store (may be constant or VariableBase object)</param>
        void Assign(VariableBase leftHandSide, object value);

        /// <summary>
        ///     Returns a specified value
        /// </summary>
        /// <param name="returnValue">Variable to return</param>
        void Return(object returnValue);

        /// <summary>
        ///     Returns from the method (used if void is the return type)
        /// </summary>
        void Return();

        /// <summary>
        ///     Calls a function on an object
        /// </summary>
        /// <param name="objectCallingOn">Object calling on</param>
        /// <param name="methodCalling">Method calling</param>
        /// <param name="parameters">parameters sending</param>
        /// <returns>The return value</returns>
        VariableBase Call(VariableBase objectCallingOn, MethodInfo methodCalling, object[] parameters);

        /// <summary>
        ///     Calls a function on an object
        /// </summary>
        /// <param name="objectCallingOn">Object calling on</param>
        /// <param name="methodCalling">Method calling</param>
        /// <param name="parameters">parameters sending</param>
        /// <returns>The return value</returns>
        void Call(VariableBase objectCallingOn, ConstructorInfo methodCalling, object[] parameters);

        /// <summary>
        ///     Defines an if statement
        /// </summary>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="leftHandSide">Left hand side of the if statement</param>
        /// <param name="rightHandSide">Right hand side of the if statement</param>
        /// <returns>The if command</returns>
        If If(VariableBase leftHandSide, Comparison comparisonType, VariableBase rightHandSide);

        /// <summary>
        ///     Defines the end of an if statement
        /// </summary>
        /// <param name="ifCommand">If command</param>
        void EndIf(If ifCommand);

        /// <summary>
        ///     Defines a while statement
        /// </summary>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="leftHandSide">Left hand side of the while statement</param>
        /// <param name="rightHandSide">Right hand side of the while statement</param>
        /// <returns>The while command</returns>
        While While(VariableBase leftHandSide, Comparison comparisonType, VariableBase rightHandSide);

        /// <summary>
        ///     Defines the end of a while statement
        /// </summary>
        /// <param name="whileCommand">While command</param>
        void EndWhile(While whileCommand);

        /// <summary>
        ///     Adds two variables and returns the result
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The result</returns>
        VariableBase Add(object leftHandSide, object rightHandSide);

        /// <summary>
        ///     Subtracts two variables and returns the result
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The result</returns>
        VariableBase Subtract(object leftHandSide, object rightHandSide);

        /// <summary>
        ///     Multiplies two variables and returns the result
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The result</returns>
        VariableBase Multiply(object leftHandSide, object rightHandSide);

        /// <summary>
        ///     Divides two variables and returns the result
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The result</returns>
        VariableBase Divide(object leftHandSide, object rightHandSide);

        /// <summary>
        ///     Mods (%) two variables and returns the result
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The result</returns>
        VariableBase Modulo(object leftHandSide, object rightHandSide);

        /// <summary>
        ///     Starts a try block
        /// </summary>
        Try Try();

        /// <summary>
        ///     Ends a try block and starts a catch block
        /// </summary>
        /// <param name="exceptionType">Exception type</param>
        Catch Catch(Type exceptionType);

        /// <summary>
        ///     Ends a try/catch block
        /// </summary>
        void EndTry();

        /// <summary>
        ///     Boxes a value
        /// </summary>
        /// <param name="value">Value to box</param>
        /// <returns>The resulting boxed variable</returns>
        VariableBase Box(object value);

        /// <summary>
        ///     Unboxes a value
        /// </summary>
        /// <param name="value">Value to unbox</param>
        /// <param name="valueType">Type to unbox to</param>
        /// <returns>The resulting unboxed variable</returns>
        VariableBase UnBox(VariableBase value, Type valueType);

        /// <summary>
        ///     Casts an object to another type
        /// </summary>
        /// <param name="value">Value to cast</param>
        /// <param name="valueType">Value type to cast to</param>
        /// <returns>The resulting casted value</returns>
        VariableBase Cast(VariableBase value, Type valueType);

        /// <summary>
        ///     Throws an exception
        /// </summary>
        /// <param name="exception">Exception to throw</param>
        void Throw(VariableBase exception);

        #endregion

        #region Properties

        /// <summary>
        ///     Method name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Return type
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        ///     parameters
        /// </summary>
        ICollection<ParameterBuilder> Parameters { get; }

        /// <summary>
        ///     Attributes for the method
        /// </summary>
        MethodAttributes Attributes { get; }

        /// <summary>
        ///     IL generator for this method
        /// </summary>
        ILGenerator Generator { get; }

        /// <summary>
        ///     Returns the this object for this object
        /// </summary>
        VariableBase This { get; }

        #endregion
    }
}