#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Yea.DataTypes.ExtensionMethods;
using Yea.Reflection.AOP.EventArgs;
using Yea.Reflection.Emit.BaseClasses;
using Yea.Reflection.Emit.Commands;
using Yea.Reflection.Emit.Enums;
using Yea.Reflection.Emit.Interfaces;
using Assembly = Yea.Reflection.Emit.Assembly;
using Exception = Yea.Reflection.AOP.EventArgs.Exception;
using ParameterBuilder = Yea.Reflection.Emit.ParameterBuilder;
using TypeBuilder = Yea.Reflection.Emit.TypeBuilder;

#endregion

namespace Yea.Reflection.AOP
{
    /// <summary>
    ///     AOP interface manager
    /// </summary>
    public class AOPManager
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="aspectLocation">Aspect DLL location (optional)</param>
        /// <param name="assemblyDirectory">Directory to save the generated types (optional)</param>
        /// <param name="assemblyName">Assembly name to save the generated types as (optional)</param>
        /// <param name="regenerateAssembly">Should this assembly be regenerated if found? (optional)</param>
        public AOPManager(string aspectLocation = "", string assemblyDirectory = "", string assemblyName = "Aspects",
                          bool regenerateAssembly = false)
        {
            AssemblyDirectory = assemblyDirectory;
            AssemblyName = assemblyName;
            RegenerateAssembly = regenerateAssembly;
            if (!string.IsNullOrEmpty(aspectLocation))
            {
                if (aspectLocation.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase))
                    _aspects.AddRange(new AssemblyName(aspectLocation).Load().GetObjects<IAspect>());
                else if (new DirectoryInfo(aspectLocation).Exists)
                    _aspects.AddRange(new DirectoryInfo(aspectLocation).GetObjects<IAspect>());
                else
                    _aspects.AddRange(new AssemblyName(aspectLocation).Load().GetObjects<IAspect>());
            }
            if (AssemblyBuilder != null)
                return;
            if (string.IsNullOrEmpty(assemblyDirectory)
                || !new FileInfo(assemblyDirectory + assemblyName + ".dll").Exists
                || regenerateAssembly)
            {
                AssemblyBuilder = new Assembly(assemblyName, assemblyDirectory);
            }
            else
            {
                new AssemblyName(assemblyDirectory + assemblyName + ".dll").Load()
                                                                           .GetTypes()
                                                                           .ForEach(x => _classes.Add(x.BaseType, x));
            }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Clears out the AOP data (really only used in testing)
        /// </summary>
        public static void Destroy()
        {
            AssemblyBuilder = null;
            _classes = new Dictionary<Type, Type>();
            _aspects = new List<IAspect>();
        }

        /// <summary>
        ///     Adds an aspect to the manager (only needed if loading aspects manually)
        /// </summary>
        /// <param name="aspect">Aspect to load</param>
        public virtual void AddAspect(IAspect aspect)
        {
            _aspects.AddIfUnique(aspect);
        }

        /// <summary>
        ///     Saves the assembly to the directory
        /// </summary>
        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(AssemblyDirectory)
                && (!new FileInfo(AssemblyDirectory + AssemblyName + ".dll").Exists
                    || RegenerateAssembly))
            {
                AssemblyBuilder.Create();
            }
            AssemblyBuilder = null;
        }

        /// <summary>
        ///     Sets up a type so it can be used in the system later
        /// </summary>
        /// <param name="type">Type to set up</param>
        public virtual void Setup(Type type)
        {
            if (_classes.ContainsKey(type))
                return;
            if (new FileInfo(AssemblyDirectory + AssemblyName + ".dll").Exists
                && !RegenerateAssembly)
                throw new ArgumentException(
                    "Type specified not found and can't be generated due to being in 'GoDaddy' mode. Delete already generated DLL to add new types or set RegenerateAssembly to true.");
            var interfaces = new List<Type>();
            _aspects.ForEach(x => interfaces.AddRange(x.InterfacesUsing == null ? new List<Type>() : x.InterfacesUsing));
            interfaces.Add(typeof (IEvents));
            TypeBuilder builder = AssemblyBuilder.CreateType(AssemblyName + "." + type.Name + "Derived",
                                                             TypeAttributes.Class | TypeAttributes.Public,
                                                             type,
                                                             interfaces);
            {
                IPropertyBuilder aspectusStarting = builder.CreateDefaultProperty("Aspectus_Starting",
                                                                                  typeof (EventHandler<Starting>));
                IPropertyBuilder aspectusEnding = builder.CreateDefaultProperty("Aspectus_Ending",
                                                                                typeof (EventHandler<Ending>));
                IPropertyBuilder aspectusException = builder.CreateDefaultProperty("Aspectus_Exception",
                                                                                   typeof (EventHandler<Exception>));

                _aspects.ForEach(x => x.SetupInterfaces(builder));

                Type tempType = type;
                var methodsAlreadyDone = new List<string>();
                while (tempType != null)
                {
                    foreach (
                        var property in
                            tempType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                   BindingFlags.DeclaredOnly | BindingFlags.Instance))
                    {
                        MethodInfo getMethodInfo = property.GetGetMethod();
                        if (!methodsAlreadyDone.Contains("get_" + property.Name)
                            && !methodsAlreadyDone.Contains("set_" + property.Name)
                            && getMethodInfo != null
                            && getMethodInfo.IsVirtual
                            && !getMethodInfo.IsFinal)
                        {
                            IPropertyBuilder overrideProperty = builder.CreateProperty(property.Name,
                                                                                       property.PropertyType,
                                                                                       PropertyAttributes.SpecialName,
                                                                                       MethodAttributes.Public |
                                                                                       MethodAttributes.SpecialName |
                                                                                       MethodAttributes.HideBySig |
                                                                                       MethodAttributes.Virtual,
                                                                                       MethodAttributes.Public |
                                                                                       MethodAttributes.SpecialName |
                                                                                       MethodAttributes.HideBySig |
                                                                                       MethodAttributes.Virtual);
                            {
                                IMethodBuilder get = overrideProperty.GetMethod;
                                {
                                    SetupMethod(type, get, aspectusStarting, aspectusEnding, aspectusException, null);
                                    methodsAlreadyDone.Add(get.Name);
                                }
                                IMethodBuilder set = overrideProperty.SetMethod;
                                {
                                    SetupMethod(type, set, aspectusStarting, aspectusEnding, aspectusException, null);
                                    methodsAlreadyDone.Add(set.Name);
                                }
                            }
                        }
                    }
                    foreach (
                        var method in
                            tempType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
                                                BindingFlags.Instance))
                    {
                        if (!methodsAlreadyDone.Contains(method.Name) && method.IsVirtual && !method.IsFinal)
                        {
                            MethodAttributes methodAttribute = MethodAttributes.Virtual | MethodAttributes.HideBySig |
                                                               MethodAttributes.SpecialName | MethodAttributes.Public;
                            if (method.IsStatic)
                                methodAttribute |= MethodAttributes.Static;
                            var parameterTypes = new List<Type>();
                            method.GetParameters().ForEach(x => parameterTypes.Add(x.ParameterType));
                            IMethodBuilder overrideMethod = builder.CreateMethod(method.Name,
                                                                                 methodAttribute,
                                                                                 method.ReturnType,
                                                                                 parameterTypes);
                            SetupMethod(type, overrideMethod, aspectusStarting, aspectusEnding, aspectusException,
                                        method);
                            methodsAlreadyDone.Add(method.Name);
                        }
                    }

                    tempType = tempType.BaseType;
                    if (tempType == typeof (object))
                        break;
                }

                _classes.Add(type, builder.Create());
            }
        }

        /// <summary>
        ///     Creates an object of the specified base type, registering the type if necessary
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <returns>Returns an object of the specified base type</returns>
        public virtual T Create<T>()
        {
            return (T) Create(typeof (T));
        }

        /// <summary>
        ///     Creates an object of the specified base type, registering the type if necessary
        /// </summary>
        /// <param name="baseType">The base type</param>
        /// <returns>Returns an object of the specified base type</returns>
        public virtual object Create(Type baseType)
        {
            if (!_classes.ContainsKey(baseType))
                Setup(baseType);
            object returnObject = _classes[baseType].Assembly.CreateInstance(_classes[baseType].FullName);
            _aspects.ForEach(x => x.Setup(returnObject));
            return returnObject;
        }

        #endregion

        #region Private Functions

        private static void SetupMethod(Type baseType, IMethodBuilder method, IPropertyBuilder aspectusStarting,
                                        IPropertyBuilder aspectusEnding, IPropertyBuilder aspectusException,
                                        MethodInfo baseMethod)
        {
            if (baseMethod == null)
                baseMethod = baseType.GetMethod(method.Name);
            method.SetCurrentMethod();
            Label endLabel = method.Generator.DefineLabel();
            VariableBase returnValue = method.ReturnType != typeof (void)
                                           ? method.CreateLocal("FinalReturnValue", method.ReturnType)
                                           : null;
            Try Try = method.Try();
            {
                SetupStart(method, endLabel, returnValue, aspectusStarting);
                _aspects.ForEach(x => x.SetupStartMethod(method, baseType));
                var parameters = new List<ParameterBuilder>();
                method.Parameters.For(1, method.Parameters.Count - 1, x => parameters.Add(x));
                if (method.ReturnType != typeof (void) && baseMethod != null)
                    returnValue.Assign(method.This.Call(baseMethod, parameters.ToArray()));
                else if (baseMethod != null)
                    method.This.Call(baseMethod, parameters.ToArray());
                SetupEnd(method, returnValue, aspectusEnding);
                _aspects.ForEach(x => x.SetupEndMethod(method, baseType, returnValue));
                method.Generator.MarkLabel(endLabel);
            }
            Catch Catch = Try.StartCatchBlock(typeof (System.Exception));
            {
                SetupException(method, Catch, aspectusException);
                _aspects.ForEach(x => x.SetupExceptionMethod(method, baseType));
                Catch.Rethrow();
            }
            Try.EndTryBlock();

            if (method.ReturnType != typeof (void))
                method.Return(returnValue);
            else
                method.Return();
        }

        private static void SetupException(IMethodBuilder method, Catch Catch, IPropertyBuilder aspectusException)
        {
            VariableBase exceptionArgs = method.NewObj(typeof (Exception).GetConstructor(new Type[0]));
            exceptionArgs.Call(typeof (Exception).GetProperty("InternalException").GetSetMethod(),
                               new object[] {Catch.Exception});
            VariableBase eventsThis = method.Cast(method.This, typeof (IEvents));
            Type eventHelperType = typeof (DelegateExtensions);
            MethodInfo[] methods = eventHelperType.GetMethods()
                                                  .Where(x => x.GetParameters().Length == 3)
                                                  .ToArray();
            MethodInfo tempMethod = methods.Length > 0 ? methods[0] : null;
            tempMethod = tempMethod.MakeGenericMethod(new[] {typeof (Exception)});
            method.Call(null, tempMethod, new object[] {aspectusException, eventsThis, exceptionArgs});
        }

        private static void SetupEnd(IMethodBuilder method, VariableBase returnValue, IPropertyBuilder aspectusEnding)
        {
            VariableBase endingArgs = method.NewObj(typeof (Ending).GetConstructor(new Type[0]));
            endingArgs.Call(typeof (Ending).GetProperty("MethodName").GetSetMethod(), new object[] {method.Name});
            if (method.ReturnType != typeof (void) && returnValue.DataType != null && returnValue.DataType.IsValueType)
                endingArgs.Call(typeof (Ending).GetProperty("ReturnValue").GetSetMethod(),
                                new object[] {method.Box(returnValue)});
            else if (method.ReturnType != typeof (void))
                endingArgs.Call(typeof (Ending).GetProperty("ReturnValue").GetSetMethod(), new object[] {returnValue});
            VariableBase parameterList = endingArgs.Call(typeof (Ending).GetProperty("Parameters").GetGetMethod());
            for (int x = 1; x < method.Parameters.Count; ++x)
            {
                if (method.Parameters.ElementAt(x).DataType != null &&
                    method.Parameters.ElementAt(x).DataType.IsValueType)
                    parameterList.Call(typeof (List<object>).GetMethod("Add"),
                                       new object[] {method.Box(method.Parameters.ElementAt(x))});
                else
                    parameterList.Call(typeof (List<object>).GetMethod("Add"),
                                       new object[] {method.Parameters.ElementAt(x)});
            }

            VariableBase eventsThis = method.Cast(method.This, typeof (IEvents));
            Type eventHelperType = typeof (DelegateExtensions);
            MethodInfo[] methods = eventHelperType.GetMethods()
                                                  .Where(x => x.GetParameters().Length == 3)
                                                  .ToArray();
            MethodInfo tempMethod = methods.Length > 0 ? methods[0] : null;
            tempMethod = tempMethod.MakeGenericMethod(new[] {typeof (Ending)});
            method.Call(null, tempMethod, new object[] {aspectusEnding, eventsThis, endingArgs});
            if (method.ReturnType != typeof (void))
            {
                VariableBase tempReturnValue = endingArgs.Call(typeof (Ending).GetProperty("ReturnValue").GetGetMethod());
                VariableBase tempNull = method.CreateLocal("TempNull", typeof (object));
                If If = method.If(tempReturnValue, Comparison.NotEqual, tempNull);
                {
                    returnValue.Assign(tempReturnValue);
                }
                method.SetCurrentMethod();
                If.EndIf();
            }
        }

        private static void SetupStart(IMethodBuilder method, Label endLabel,
                                       VariableBase returnValue, IPropertyBuilder aspectusStarting)
        {
            if (method == null) throw new ArgumentNullException("method");
            VariableBase startingArgs = method.NewObj(typeof (Starting).GetConstructor(new Type[0]));
            startingArgs.Call(typeof (Starting).GetProperty("MethodName").GetSetMethod(), new object[] {method.Name});

            VariableBase parameterList = startingArgs.Call(typeof (Starting).GetProperty("Parameters").GetGetMethod());
            for (int x = 1; x < method.Parameters.Count; ++x)
            {
                if (method.Parameters.ElementAt(x).DataType != null &&
                    method.Parameters.ElementAt(x).DataType.IsValueType)
                    parameterList.Call(typeof (List<object>).GetMethod("Add"),
                                       new object[] {method.Box(method.Parameters.ElementAt(x))});
                else
                    parameterList.Call(typeof (List<object>).GetMethod("Add"),
                                       new object[] {method.Parameters.ElementAt(x)});
            }

            VariableBase eventsThis = method.Cast(method.This, typeof (IEvents));
            Type eventHelperType = typeof (DelegateExtensions);
            MethodInfo[] methods = eventHelperType.GetMethods()
                                                  .Where(x => x.GetParameters().Length == 3)
                                                  .ToArray();
            MethodInfo tempMethod = methods.Length > 0 ? methods[0] : null;
            tempMethod = tempMethod.MakeGenericMethod(new[] {typeof (Starting)});
            method.Call(null, tempMethod, new object[] {aspectusStarting, eventsThis, startingArgs});
            if (method.ReturnType != typeof (void))
            {
                VariableBase tempReturnValue =
                    startingArgs.Call(typeof (Starting).GetProperty("ReturnValue").GetGetMethod());
                VariableBase tempNull = method.CreateLocal("TempNull", typeof (object));
                If If = method.If(tempReturnValue, Comparison.NotEqual, tempNull);
                {
                    returnValue.Assign(tempReturnValue);
                    method.Generator.Emit(OpCodes.Br, endLabel);
                }
                method.SetCurrentMethod();
                If.EndIf();
            }
        }

        #endregion

        #region Properties/Fields

        /// <summary>
        ///     Dictionary containing generated types and associates it with original type
        /// </summary>
        private static Dictionary<Type, Type> _classes = new Dictionary<Type, Type>();

        /// <summary>
        ///     The list of aspects that are being used
        /// </summary>
        private static List<IAspect> _aspects = new List<IAspect>();

        /// <summary>
        ///     Assembly containing generated types
        /// </summary>
        protected static Assembly AssemblyBuilder { get; set; }

        /// <summary>
        ///     Assembly directory
        /// </summary>
        protected virtual string AssemblyDirectory { get; set; }

        /// <summary>
        ///     Assembly name
        /// </summary>
        protected virtual string AssemblyName { get; set; }

        /// <summary>
        ///     Determines if the assembly needs to be regenerated
        /// </summary>
        protected virtual bool RegenerateAssembly { get; set; }

        #endregion
    }
}