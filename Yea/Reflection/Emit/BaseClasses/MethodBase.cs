#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Yea.Reflection.Emit.Commands;
using Yea.Reflection.Emit.Enums;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.BaseClasses
{
    /// <summary>
    ///     Method base class
    /// </summary>
    public class MethodBase : IMethodBuilder
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public MethodBase()
        {
            Commands = new List<ICommand>();
            Parameters = new List<ParameterBuilder>();
            SetCurrentMethod();
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Creates a local variable
        /// </summary>
        /// <param name="name">name of the local</param>
        /// <param name="localType">Object type</param>
        /// <returns>The variable</returns>
        public virtual VariableBase CreateLocal(string name, Type localType)
        {
            SetCurrentMethod();
            var tempCommand = new DefineLocal(name, localType);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            return tempCommand.Result;
        }

        /// <summary>
        ///     Creates a constant
        /// </summary>
        /// <param name="value">Constant value</param>
        /// <returns>The constant</returns>
        public virtual VariableBase CreateConstant(object value)
        {
            SetCurrentMethod();
            return new ConstantBuilder(value);
        }

        /// <summary>
        ///     Creates a new object
        /// </summary>
        /// <param name="constructor">Constructor to use</param>
        /// <param name="variables">Variables to use</param>
        /// <returns>The new object</returns>
        public virtual VariableBase NewObj(ConstructorInfo constructor, object[] variables = null)
        {
            SetCurrentMethod();
            var tempCommand = new NewObj(constructor, variables);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Creates a new object
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="variables">Variables to use</param>
        /// <returns>The new object</returns>
        public virtual VariableBase NewObj(Type objectType, object[] variables = null)
        {
            SetCurrentMethod();
            var variableTypes = new List<Type>();
            if (variables != null)
            {
                variableTypes.AddRange(from variable in variables
                                       let tempVariable = variable as VariableBase
                                       select tempVariable != null ? tempVariable.DataType : variable.GetType());
            }
            ConstructorInfo constructor = objectType.GetConstructor(variableTypes.ToArray());
            return NewObj(constructor, variables);
        }

        /// <summary>
        ///     Calls a method
        /// </summary>
        /// <param name="objectCallingOn">Object to call the method on</param>
        /// <param name="methodCalling">Method to call</param>
        /// <param name="parameters">parameters to use</param>
        /// <returns>The result of the method call</returns>
        public virtual VariableBase Call(VariableBase objectCallingOn, MethodInfo methodCalling, object[] parameters)
        {
            SetCurrentMethod();
            var tempCommand = new Call(this, objectCallingOn, methodCalling, parameters);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Calls a constructor
        /// </summary>
        /// <param name="objectCallingOn">Object to call the constructor on</param>
        /// <param name="methodCalling">Constructor to call</param>
        /// <param name="parameters">parameters to use</param>
        public virtual void Call(VariableBase objectCallingOn, ConstructorInfo methodCalling, object[] parameters)
        {
            SetCurrentMethod();
            var tempCommand = new Call(this, objectCallingOn, methodCalling, parameters);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
        }

        /// <summary>
        ///     Assigns a value to a variable
        /// </summary>
        /// <param name="leftHandSide">Variable to assign to</param>
        /// <param name="value">Value to assign</param>
        public virtual void Assign(VariableBase leftHandSide, object value)
        {
            SetCurrentMethod();
            var tempCommand = new Assign(leftHandSide, value);
            tempCommand.Setup();
            Commands.Add(tempCommand);
        }

        /// <summary>
        ///     Returns a value back from the method
        /// </summary>
        /// <param name="returnValue">Value to return</param>
        public virtual void Return(object returnValue)
        {
            var tempCommand = new Return(ReturnType, returnValue);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            CurrentMethod = null;
        }

        /// <summary>
        ///     Returns from the method
        /// </summary>
        public virtual void Return()
        {
            Return(null);
        }

        /// <summary>
        ///     Creates an if statement
        /// </summary>
        /// <param name="leftHandSide">Left hand side variable</param>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="rightHandSide">Right hand side variable</param>
        /// <returns>The if object</returns>
        public virtual If If(VariableBase leftHandSide, Comparison comparisonType, VariableBase rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new If(comparisonType, leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            return tempCommand;
        }

        /// <summary>
        ///     Ends an if statement
        /// </summary>
        /// <param name="ifCommand">If command to end</param>
        public virtual void EndIf(If ifCommand)
        {
            SetCurrentMethod();
            var tempCommand = new EndIf(ifCommand);
            tempCommand.Setup();
            Commands.Add(tempCommand);
        }

        /// <summary>
        ///     Creates a while statement
        /// </summary>
        /// <param name="leftHandSide">Left hand side variable</param>
        /// <param name="comparisonType">Comparison type</param>
        /// <param name="rightHandSide">Right hand side variable</param>
        /// <returns>The while object</returns>
        public virtual While While(VariableBase leftHandSide, Comparison comparisonType, VariableBase rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new While(comparisonType, leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            return tempCommand;
        }

        /// <summary>
        ///     Ends a while statement
        /// </summary>
        /// <param name="whileCommand">While statement to end</param>
        public virtual void EndWhile(While whileCommand)
        {
            SetCurrentMethod();
            var tempCommand = new EndWhile(whileCommand);
            tempCommand.Setup();
            Commands.Add(tempCommand);
        }

        /// <summary>
        ///     Adds two objects
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Add(object leftHandSide, object rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new Add(leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Subtracts two objects
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Subtract(object leftHandSide, object rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new Subtract(leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Multiply two objects
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Multiply(object leftHandSide, object rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new Multiply(leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Divides two objects
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Divide(object leftHandSide, object rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new Divide(leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Modulo operator
        /// </summary>
        /// <param name="leftHandSide">Left hand side</param>
        /// <param name="rightHandSide">Right hand side</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Modulo(object leftHandSide, object rightHandSide)
        {
            SetCurrentMethod();
            var tempCommand = new Modulo(leftHandSide, rightHandSide);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Starts a try block
        /// </summary>
        /// <returns>The resulting try block</returns>
        public virtual Try Try()
        {
            var tempCommand = new Try();
            tempCommand.Setup();
            Commands.Add(tempCommand);
            return tempCommand;
        }

        /// <summary>
        ///     Starts a catch block
        /// </summary>
        /// <param name="exceptionType">Exception type to catch</param>
        /// <returns>The resulting catch block</returns>
        public virtual Catch Catch(Type exceptionType)
        {
            var tempCommand = new Catch(exceptionType);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            tempCommand.Exception = CreateLocal(
                "ExceptionLocal" + ObjectCounter.ToString(CultureInfo.InvariantCulture), exceptionType);
            tempCommand.Exception.Save(Generator);
            ++ObjectCounter;
            return tempCommand;
        }

        /// <summary>
        ///     Ends a try block
        /// </summary>
        public virtual void EndTry()
        {
            var tempCommand = new EndTry();
            tempCommand.Setup();
            Commands.Add(tempCommand);
        }

        /// <summary>
        ///     Boxes an object
        /// </summary>
        /// <param name="value">Variable to box</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Box(object value)
        {
            var tempCommand = new Box(value);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Unboxes an object
        /// </summary>
        /// <param name="value">Value to unbox</param>
        /// <param name="valueType">Type to unbox to</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase UnBox(VariableBase value, Type valueType)
        {
            var tempCommand = new UnBox(value, valueType);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Casts an object as a specific type
        /// </summary>
        /// <param name="value">Value to cast</param>
        /// <param name="valueType">Type to cast to</param>
        /// <returns>The resulting variable</returns>
        public virtual VariableBase Cast(VariableBase value, Type valueType)
        {
            var tempCommand = new Cast(value, valueType);
            tempCommand.Setup();
            Commands.Add(tempCommand);
            ++ObjectCounter;
            return tempCommand.Result;
        }

        /// <summary>
        ///     Throws an exception
        /// </summary>
        /// <param name="exception">Exception to throw</param>
        public virtual void Throw(VariableBase exception)
        {
            var tempCommand = new Throw(exception);
            tempCommand.Setup();
            Commands.Add(tempCommand);
        }

        /// <summary>
        ///     Sets the current method to this
        /// </summary>
        public virtual void SetCurrentMethod()
        {
            CurrentMethod = this;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Current method
        /// </summary>
        public static MethodBase CurrentMethod { get; protected set; }

        /// <summary>
        ///     Commands used in the method
        /// </summary>
        public ICollection<ICommand> Commands { get; private set; }

        /// <summary>
        ///     Object counter
        /// </summary>
        public static int ObjectCounter { get; set; }

        /// <summary>
        ///     name of the method
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Return type
        /// </summary>
        public Type ReturnType { get; protected set; }

        /// <summary>
        ///     parameters
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<ParameterBuilder> Parameters { get; protected set; }

        /// <summary>
        ///     Attributes for the method
        /// </summary>
        public MethodAttributes Attributes { get; protected set; }

        /// <summary>
        ///     IL Generator for the method
        /// </summary>
        public ILGenerator Generator { get; protected set; }

        /// <summary>
        ///     The this object
        /// </summary>
        public VariableBase This
        {
            get { return Parameters.FirstOrDefault(); }
        }

        #endregion
    }
}