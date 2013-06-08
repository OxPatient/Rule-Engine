#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Yea.Reflection.Emit.Interfaces;

#endregion

namespace Yea.Reflection.Emit.BaseClasses
{
    /// <summary>
    ///     Variable base class
    /// </summary>
    public abstract class VariableBase
    {
        #region Constructor

        #endregion

        #region Properties

        /// <summary>
        ///     Variable name
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        ///     Variable data type
        /// </summary>
        public virtual Type DataType { get; protected set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Assigns the value to the variable
        /// </summary>
        /// <param name="value">Value to assign</param>
        public virtual void Assign(object value)
        {
            MethodBase.CurrentMethod.Assign(this, value);
        }

        /// <summary>
        ///     Loads the variable onto the stack
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public abstract void Load(ILGenerator generator);

        /// <summary>
        ///     Saves the top item from the stack onto the variable
        /// </summary>
        /// <param name="generator">IL Generator</param>
        public abstract void Save(ILGenerator generator);

        /// <summary>
        ///     Gets the definition of the variable
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetDefinition()
        {
            return DataType.GetName() + " " + Name;
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual VariableBase Call(string methodName, object[] parameters = null)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException("methodName");
            var parameterTypes = new List<Type>();
            if (parameters != null)
            {
                parameterTypes.AddRange(from parameter in parameters
                                        let tempParameter = parameter as VariableBase
                                        select tempParameter != null ? tempParameter.DataType : parameter.GetType());
            }
            return Call(DataType.GetMethod(methodName, parameterTypes.ToArray()), parameters);
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual VariableBase Call(MethodBuilder method, object[] parameters = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            return Call(method.Builder, parameters);
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual VariableBase Call(System.Reflection.Emit.MethodBuilder method, object[] parameters = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            return MethodBase.CurrentMethod.Call(this, method, parameters);
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual VariableBase Call(MethodInfo method, object[] parameters = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            return MethodBase.CurrentMethod.Call(this, method, parameters);
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual void Call(ConstructorInfo method, object[] parameters = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            MethodBase.CurrentMethod.Call(this, method, parameters);
        }

        /// <summary>
        ///     Calls a method on this variable
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="parameters">Parameters sent in</param>
        /// <returns>Variable returned by the function (if one exists, null otherwise)</returns>
        public virtual VariableBase Call(IMethodBuilder method, object[] parameters = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            return Call((MethodBuilder) method, parameters);
        }

        #endregion

        #region Operator Functions

        /// <summary>
        ///     Addition operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        [SuppressMessage("Microsoft.Design", "CA1013:OverloadOperatorEqualsOnOverloadingAddAndSubtract")]
        public static VariableBase operator +(VariableBase left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Add(left, right);
        }

        /// <summary>
        ///     Subtraction operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        [SuppressMessage("Microsoft.Design", "CA1013:OverloadOperatorEqualsOnOverloadingAddAndSubtract")]
        public static VariableBase operator -(VariableBase left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Subtract(left, right);
        }

        /// <summary>
        ///     Multiplication operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator *(VariableBase left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Multiply(left, right);
        }

        /// <summary>
        ///     Division operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator /(VariableBase left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Divide(left, right);
        }

        /// <summary>
        ///     Modulo operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator %(VariableBase left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Modulo(left, right);
        }

        /// <summary>
        ///     Addition operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator +(VariableBase left, object right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Add(left, right);
        }

        /// <summary>
        ///     Subtraction operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator -(VariableBase left, object right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Subtract(left, right);
        }

        /// <summary>
        ///     Multiplication operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator *(VariableBase left, object right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Multiply(left, right);
        }

        /// <summary>
        ///     Division operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator /(VariableBase left, object right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Divide(left, right);
        }

        /// <summary>
        ///     Modulo operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator %(VariableBase left, object right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Modulo(left, right);
        }

        /// <summary>
        ///     Addition operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator +(object left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Add(left, right);
        }

        /// <summary>
        ///     Subtraction operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator -(object left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Subtract(left, right);
        }

        /// <summary>
        ///     Multiplication operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator *(object left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Multiply(left, right);
        }

        /// <summary>
        ///     Division operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator /(object left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Divide(left, right);
        }

        /// <summary>
        ///     Modulo operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <param name="right">Right side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator %(object left, VariableBase right)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            return MethodBase.CurrentMethod.Modulo(left, right);
        }

        /// <summary>
        ///     Plus one operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator ++(VariableBase left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Add(left, 1));
            return left;
        }

        /// <summary>
        ///     Subtract one operator
        /// </summary>
        /// <param name="left">Left side</param>
        /// <returns>The resulting object</returns>
        public static VariableBase operator --(VariableBase left)
        {
            if (MethodBase.CurrentMethod == null)
                throw new InvalidOperationException("Unsure which method is the current method");
            left.Assign(MethodBase.CurrentMethod.Subtract(left, 1));
            return left;
        }

        #endregion
    }
}