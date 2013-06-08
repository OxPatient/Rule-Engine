#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.Reflection
{
    /// <summary>
    ///     Reflection oriented extensions
    /// </summary>
    public static class ReflectionExtensions
    {
        #region CallMethod

        /// <summary>
        ///     Calls a method on an object
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="Object">Object to call the method on</param>
        /// <param name="inputVariables">(Optional)input variables for the method</param>
        /// <typeparam name="TReturnType">Return type expected</typeparam>
        /// <returns>The returned value of the method</returns>
        public static TReturnType CallMethod<TReturnType>(this object Object, string methodName,
                                                          params object[] inputVariables)
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException("methodName");
            if (inputVariables == null)
                inputVariables = new object[0];
            Type objectType = Object.GetType();
            var methodInputTypes = new Type[inputVariables.Length];
            for (int x = 0; x < inputVariables.Length; ++x)
                methodInputTypes[x] = inputVariables[x].GetType();
            MethodInfo method = objectType.GetMethod(methodName, methodInputTypes);
            if (method == null)
                throw new InvalidOperationException("Could not find method " + methodName +
                                                    " with the appropriate input variables.");
            return (TReturnType) method.Invoke(Object, inputVariables);
        }

        #endregion

        #region CreateInstance

        /// <summary>
        ///     Creates an instance of the type and casts it to the specified type
        /// </summary>
        /// <typeparam name="TClassType">Class type to return</typeparam>
        /// <param name="type">Type to create an instance of</param>
        /// <param name="args">Arguments sent into the constructor</param>
        /// <returns>The newly created instance of the type</returns>
        public static TClassType CreateInstance<TClassType>(this Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return (TClassType) type.CreateInstance(args);
        }

        /// <summary>
        ///     Creates an instance of the type
        /// </summary>
        /// <param name="type">Type to create an instance of</param>
        /// <param name="args">Arguments sent into the constructor</param>
        /// <returns>The newly created instance of the type</returns>
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return Activator.CreateInstance(type, args);
        }

        #endregion

        #region DumpProperties

        /// <summary>
        ///     Dumps the property names and current values from an object
        /// </summary>
        /// <param name="Object">Object to dunp</param>
        /// <param name="htmlOutput">Determines if the output should be HTML or not</param>
        /// <returns>An HTML formatted table containing the information about the object</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string DumpProperties(this object Object, bool htmlOutput = true)
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            var tempValue = new StringBuilder();
            tempValue.Append(htmlOutput
                                 ? "<table><thead><tr><th>Property Name</th><th>Property Value</th></tr></thead><tbody>"
                                 : "Property Name\t\t\t\tProperty Value");
            Type objectType = Object.GetType();
            foreach (var property in objectType.GetProperties())
            {
                tempValue.Append(htmlOutput ? "<tr><td>" : "")
                         .Append(property.Name)
                         .Append(htmlOutput ? "</td><td>" : "\t\t\t\t");
                ParameterInfo[] parameters = property.GetIndexParameters();
                if (property.CanRead && parameters.Length == 0)
                {
                    try
                    {
                        object value = property.GetValue(Object, null);
                        tempValue.Append(value == null ? "null" : value.ToString());
                    }
                    catch
                    {
                    }
                }
                tempValue.Append(htmlOutput ? "</td></tr>" : "");
            }
            tempValue.Append(htmlOutput ? "</tbody></table>" : "");
            return tempValue.ToString();
        }

        /// <summary>
        ///     Dumps the properties names and current values
        ///     from an object type (used for static classes)
        /// </summary>
        /// <param name="objectType">Object type to dunp</param>
        /// <param name="htmlOutput">Should this be output as an HTML string</param>
        /// <returns>An HTML formatted table containing the information about the object type</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string DumpProperties(this Type objectType, bool htmlOutput = true)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            var tempValue = new StringBuilder();
            tempValue.Append(htmlOutput
                                 ? "<table><thead><tr><th>Property Name</th><th>Property Value</th></tr></thead><tbody>"
                                 : "Property Name\t\t\t\tProperty Value");
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (var property in properties)
            {
                tempValue.Append(htmlOutput ? "<tr><td>" : "")
                         .Append(property.Name)
                         .Append(htmlOutput ? "</td><td>" : "\t\t\t\t");
                if (property.GetIndexParameters().Length == 0)
                {
                    try
                    {
                        tempValue.Append(property.GetValue(null, null) == null
                                             ? "null"
                                             : property.GetValue(null, null).ToString());
                    }
                    catch
                    {
                    }
                }
                tempValue.Append(htmlOutput ? "</td></tr>" : "");
            }
            tempValue.Append(htmlOutput ? "</tbody></table>" : "");
            return tempValue.ToString();
        }

        #endregion

        #region GetAssemblyInformation

        /// <summary>
        ///     Gets assembly information for all currently loaded assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to dump information from</param>
        /// <param name="htmlOutput">Should HTML output be used</param>
        /// <returns>An HTML formatted string containing the assembly information</returns>
        public static string GetAssemblyInformation(this IEnumerable<Assembly> assemblies, bool htmlOutput = false)
        {
            var builder = new StringBuilder();
            builder.Append(htmlOutput ? "<strong>Assembly Information</strong><br />" : "Assembly Information\r\n");
            assemblies.ForEach<Assembly>(x => builder.Append(x.DumpProperties(htmlOutput)));
            return builder.ToString();
        }

        #endregion

        #region GetAttribute

        /// <summary>
        ///     Gets the attribute from the item
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="provider">Attribute provider</param>
        /// <param name="inherit">When true, it looks up the heirarchy chain for the inherited custom attributes</param>
        /// <returns>Attribute specified if it exists</returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = true) where T : Attribute
        {
            return provider.IsDefined(typeof (T), inherit) ? provider.GetAttributes<T>(inherit)[0] : default(T);
        }

        #endregion

        #region GetAttributes

        /// <summary>
        ///     Gets the attributes from the item
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="provider">Attribute provider</param>
        /// <param name="inherit">When true, it looks up the heirarchy chain for the inherited custom attributes</param>
        /// <returns>Array of attributes</returns>
        public static T[] GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T : Attribute
        {
            return provider.IsDefined(typeof (T), inherit)
                       ? provider.GetCustomAttributes(typeof (T), inherit).ToArray(x => (T) x)
                       : new T[0];
        }

        #endregion

        #region GetName

        /// <summary>
        ///     Returns the type's name (Actual C# name, not the funky version from
        ///     the Name property)
        /// </summary>
        /// <param name="objectType">Type to get the name of</param>
        /// <returns>string name of the type</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison",
            MessageId = "System.String.IndexOf(System.String,System.StringComparison)")]
        public static string GetName(this Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            var output = new StringBuilder();
            if (String.Equals(objectType.Name, "Void", StringComparison.CurrentCulture))
            {
                output.Append("void");
            }
            else
            {
                if (objectType.Name.Contains("`"))
                {
                    Type[] genericTypes = objectType.GetGenericArguments();
                    output.Append(objectType.Name.Remove(objectType.Name.IndexOf("`", StringComparison.InvariantCulture)))
                          .Append("<");
                    string seperator = "";
                    foreach (var genericType in genericTypes)
                    {
                        output.Append(seperator).Append(genericType.GetName());
                        seperator = ",";
                    }
                    output.Append(">");
                }
                else
                {
                    output.Append(objectType.Name);
                }
            }
            return output.ToString();
        }

        #endregion

        #region GetObjects

        /// <summary>
        ///     Returns an instance of all classes that it finds within an assembly
        ///     that are of the specified base type/interface.
        /// </summary>
        /// <typeparam name="TClassType">Base type/interface searching for</typeparam>
        /// <param name="assembly">Assembly to search within</param>
        /// <returns>A list of objects that are of the type specified</returns>
        public static IEnumerable<TClassType> GetObjects<TClassType>(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            return
                assembly.GetTypes<TClassType>()
                        .Where(x => !x.ContainsGenericParameters)
                        .Select(type => type.CreateInstance<TClassType>())
                        .ToList();
        }

        /// <summary>
        ///     Returns an instance of all classes that it finds within a group of assemblies
        ///     that are of the specified base type/interface.
        /// </summary>
        /// <typeparam name="TClassType">Base type/interface searching for</typeparam>
        /// <param name="assemblies">Assemblies to search within</param>
        /// <returns>A list of objects that are of the type specified</returns>
        public static IEnumerable<TClassType> GetObjects<TClassType>(this IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");
            var returnValues = new List<TClassType>();
            foreach (var assembly in assemblies)
                returnValues.AddRange(assembly.GetObjects<TClassType>());
            return returnValues;
        }

        /// <summary>
        ///     Returns an instance of all classes that it finds within a directory
        ///     that are of the specified base type/interface.
        /// </summary>
        /// <typeparam name="TClassType">Base type/interface searching for</typeparam>
        /// <param name="directory">Directory to search within</param>
        /// <param name="recursive">Should this be recursive</param>
        /// <returns>A list of objects that are of the type specified</returns>
        public static IEnumerable<TClassType> GetObjects<TClassType>(this DirectoryInfo directory,
                                                                     bool recursive = false)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");
            return directory.LoadAssemblies(recursive).GetObjects<TClassType>();
        }

        #endregion

        #region GetProperty

        /// <summary>
        ///     Gets the value of property
        /// </summary>
        /// <param name="Object">The object to get the property of</param>
        /// <param name="property">The property to get</param>
        /// <returns>Returns the property's value</returns>
        public static object GetProperty(this object Object, PropertyInfo property)
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (property == null)
                throw new ArgumentNullException("property");
            return property.GetValue(Object, null);
        }

        /// <summary>
        ///     Gets the value of property
        /// </summary>
        /// <param name="Object">The object to get the property of</param>
        /// <param name="property">The property to get</param>
        /// <returns>Returns the property's value</returns>
        public static object GetProperty(this object Object, string property)
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            string[] properties = property.Split(new[] {"."}, StringSplitOptions.None);
            object tempObject = Object;
            Type tempObjectType = tempObject.GetType();
            PropertyInfo destinationProperty;
            for (int x = 0; x < properties.Length - 1; ++x)
            {
                destinationProperty = tempObjectType.GetProperty(properties[x]);
                tempObjectType = destinationProperty.PropertyType;
                tempObject = destinationProperty.GetValue(tempObject, null);
                if (tempObject == null)
                    return null;
            }
            destinationProperty = tempObjectType.GetProperty(properties[properties.Length - 1]);
            return tempObject.GetProperty(destinationProperty);
        }

        #endregion

        #region GetPropertyGetter

        /// <summary>
        ///     Gets a lambda expression that calls a specific property's getter function
        /// </summary>
        /// <typeparam name="TClassType">Class type</typeparam>
        /// <typeparam name="TDataType">Data type expecting</typeparam>
        /// <param name="property">Property</param>
        /// <returns>A lambda expression that calls a specific property's getter function</returns>
        public static Expression<Func<TClassType, TDataType>> GetPropertyGetter<TClassType, TDataType>(
            this PropertyInfo property)
        {
            if (!property.PropertyType.IsOfType(typeof (TDataType)))
                throw new ArgumentException("Property is not of the type specified");
            if (!property.DeclaringType.IsOfType(typeof (TClassType)) &&
                !typeof (TClassType).IsOfType(property.DeclaringType))
                throw new ArgumentException("Property is not from the declaring class type specified");
            ParameterExpression objectInstance = Expression.Parameter(property.DeclaringType, "x");
            MemberExpression PropertyGet = Expression.Property(objectInstance, property);
            if (property.PropertyType != typeof (TDataType))
            {
                UnaryExpression convert = Expression.Convert(PropertyGet, typeof (TDataType));
                return Expression.Lambda<Func<TClassType, TDataType>>(convert, objectInstance);
            }
            return Expression.Lambda<Func<TClassType, TDataType>>(PropertyGet, objectInstance);
        }

        /// <summary>
        ///     Gets a lambda expression that calls a specific property's getter function
        /// </summary>
        /// <typeparam name="TClassType">Class type</typeparam>
        /// <param name="property">Property</param>
        /// <returns>A lambda expression that calls a specific property's getter function</returns>
        public static Expression<Func<TClassType, object>> GetPropertyGetter<TClassType>(this PropertyInfo property)
        {
            return property.GetPropertyGetter<TClassType, object>();
        }

        #endregion

        #region GetPropertyName

        /// <summary>
        ///     Gets a property name
        /// </summary>
        /// <param name="expression">LINQ expression</param>
        /// <returns>The name of the property</returns>
        public static string GetPropertyName(this LambdaExpression expression)
        {
            if (expression.Body is UnaryExpression && expression.Body.NodeType == ExpressionType.Convert)
            {
                var temp = (MemberExpression) ((UnaryExpression) expression.Body).Operand;
                return GetPropertyName(temp.Expression) + temp.Member.Name;
            }
            if (!(expression.Body is MemberExpression))
                throw new ArgumentException("Expression.Body is not a MemberExpression");
            return ((MemberExpression) expression.Body).Expression.GetPropertyName() +
                   ((MemberExpression) expression.Body).Member.Name;
        }

        /// <summary>
        ///     Gets a property name
        /// </summary>
        /// <param name="expression">LINQ expression</param>
        /// <returns>The name of the property</returns>
        public static string GetPropertyName(this Expression expression)
        {
            var tempExpression = expression as MemberExpression;
            if (tempExpression == null)
                return "";
            return tempExpression.Expression.GetPropertyName() + tempExpression.Member.Name + ".";
        }

        #endregion

        #region GetPropertyType

        /// <summary>
        ///     Gets a property's type
        /// </summary>
        /// <param name="Object">object who contains the property</param>
        /// <param name="propertyPath">
        ///     Path of the property (ex: Prop1.Prop2.Prop3 would be
        ///     the Prop1 of the source object, which then has a Prop2 on it, which in turn
        ///     has a Prop3 on it.)
        /// </param>
        /// <returns>The type of the property specified or null if it can not be reached.</returns>
        public static Type GetPropertyType(this object Object, string propertyPath)
        {
            if (Object == null || string.IsNullOrEmpty(propertyPath))
                return null;
            return Object.GetType().GetPropertyType(propertyPath);
        }

        /// <summary>
        ///     Gets a property's type
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="propertyPath">
        ///     Path of the property (ex: Prop1.Prop2.Prop3 would be
        ///     the Prop1 of the source object, which then has a Prop2 on it, which in turn
        ///     has a Prop3 on it.)
        /// </param>
        /// <returns>The type of the property specified or null if it can not be reached.</returns>
        public static Type GetPropertyType(this Type objectType, string propertyPath)
        {
            if (objectType == null || string.IsNullOrEmpty(propertyPath))
                return null;
            string[] sourceProperties = propertyPath.Split(new[] {"."}, StringSplitOptions.None);
            for (int x = 0; x < sourceProperties.Length; ++x)
            {
                PropertyInfo propertyInfo = objectType.GetProperty(sourceProperties[x]);
                objectType = propertyInfo.PropertyType;
            }
            return objectType;
        }

        #endregion

        #region GetPropertySetter

        /// <summary>
        ///     Gets a lambda expression that calls a specific property's setter function
        /// </summary>
        /// <typeparam name="TClassType">Class type</typeparam>
        /// <typeparam name="TDataType">Data type expecting</typeparam>
        /// <param name="property">Property</param>
        /// <returns>A lambda expression that calls a specific property's setter function</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static Expression<Action<TClassType, TDataType>> GetPropertySetter<TClassType, TDataType>(
            this Expression<Func<TClassType, TDataType>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            string propertyName = property.GetPropertyName();
            string[] splitName = propertyName.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            PropertyInfo propertyInfo = typeof (TClassType).GetProperty(splitName[0]);
            ParameterExpression objectInstance = Expression.Parameter(propertyInfo.DeclaringType, "x");
            ParameterExpression propertySet = Expression.Parameter(typeof (TDataType), "y");
            MethodCallExpression setterCall;
            MemberExpression propertyGet = null;
            if (splitName.Length > 1)
            {
                propertyGet = Expression.Property(objectInstance, propertyInfo);
                for (int x = 1; x < splitName.Length - 1; ++x)
                {
                    propertyInfo = propertyInfo.PropertyType.GetProperty(splitName[x]);
                    propertyGet = Expression.Property(propertyGet, propertyInfo);
                }
                propertyInfo = propertyInfo.PropertyType.GetProperty(splitName[splitName.Length - 1]);
            }
            if (propertyInfo.PropertyType != typeof (TDataType))
            {
                UnaryExpression convert = Expression.Convert(propertySet, propertyInfo.PropertyType);
                if (propertyGet == null)
                    setterCall = Expression.Call(objectInstance, propertyInfo.GetSetMethod(), convert);
                else
                    setterCall = Expression.Call(propertyGet, propertyInfo.GetSetMethod(), convert);
                return Expression.Lambda<Action<TClassType, TDataType>>(setterCall, objectInstance, propertySet);
            }
            if (propertyGet == null)
                setterCall = Expression.Call(objectInstance, propertyInfo.GetSetMethod(), propertySet);
            else
                setterCall = Expression.Call(propertyGet, propertyInfo.GetSetMethod(), propertySet);
            return Expression.Lambda<Action<TClassType, TDataType>>(setterCall, objectInstance, propertySet);
        }

        /// <summary>
        ///     Gets a lambda expression that calls a specific property's setter function
        /// </summary>
        /// <typeparam name="TClassType">Class type</typeparam>
        /// <param name="property">Property</param>
        /// <returns>A lambda expression that calls a specific property's setter function</returns>
        public static Expression<Action<TClassType, object>> GetPropertySetter<TClassType>(
            this Expression<Func<TClassType, object>> property)
        {
            return property.GetPropertySetter<TClassType, object>();
        }

        #endregion

        #region GetTypes

        /// <summary>
        ///     Gets a list of types based on an interface
        /// </summary>
        /// <param name="assembly">Assembly to check</param>
        /// <typeparam name="TBaseType">Class type to search for</typeparam>
        /// <returns>List of types that use the interface</returns>
        public static IEnumerable<Type> GetTypes<TBaseType>(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            return assembly.GetTypes(typeof (TBaseType));
        }

        /// <summary>
        ///     Gets a list of types based on an interface
        /// </summary>
        /// <param name="assembly">Assembly to check</param>
        /// <param name="baseType">Base type to look for</param>
        /// <returns>List of types that use the interface</returns>
        public static IEnumerable<Type> GetTypes(this Assembly assembly, Type baseType)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (baseType == null)
                throw new ArgumentNullException("baseType");
            return assembly.GetTypes().Where(x => x.IsOfType(baseType) && x.IsClass && !x.IsAbstract);
        }

        /// <summary>
        ///     Gets a list of types based on an interface
        /// </summary>
        /// <param name="assemblies">Assemblies to check</param>
        /// <typeparam name="TBaseType">Class type to search for</typeparam>
        /// <returns>List of types that use the interface</returns>
        public static IEnumerable<Type> GetTypes<TBaseType>(this IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");
            return assemblies.GetTypes(typeof (TBaseType));
        }

        /// <summary>
        ///     Gets a list of types based on an interface
        /// </summary>
        /// <param name="assemblies">Assemblies to check</param>
        /// <param name="baseType">Base type to look for</param>
        /// <returns>List of types that use the interface</returns>
        public static IEnumerable<Type> GetTypes(this IEnumerable<Assembly> assemblies, Type baseType)
        {
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");
            if (baseType == null)
                throw new ArgumentNullException("baseType");
            var returnValues = new List<Type>();
            assemblies.ForEach(y => returnValues.AddRange(y.GetTypes(baseType)));
            return returnValues;
        }

        #endregion

        #region HasDefaultConstructor

        /// <summary>
        ///     Determines if the type has a default constructor
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>True if it does, false otherwise</returns>
        public static bool HasDefaultConstructor(this Type type)
        {
            Guard.NotNull(type, "type");
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                       .Any(x => x.GetParameters().Length == 0);
        }

        #endregion

        #region IsIEnumerable

        /// <summary>
        ///     Simple function to determine if an item is an IEnumerable
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsIEnumerable(this Type objectType)
        {
            return objectType.IsOfType(typeof (IEnumerable));
        }

        #endregion

        #region IsOfType

        /// <summary>
        ///     Determines if an object is of a specific type
        /// </summary>
        /// <param name="Object">Object</param>
        /// <param name="type">Type</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsOfType(this object Object, Type type)
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (type == null)
                throw new ArgumentNullException("type");
            return Object.GetType().IsOfType(type);
        }

        /// <summary>
        ///     Determines if an object is of a specific type
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="type">Type</param>
        /// <returns>True if it is, false otherwise</returns>
        public static bool IsOfType(this Type objectType, Type type)
        {
            if (objectType == null)
                return false;
            if (type == null)
                throw new ArgumentNullException("type");
            if (type == objectType || objectType.GetInterfaces().Any(x => x == type))
                return true;
            if (objectType.BaseType == null)
                return false;
            return objectType.BaseType.IsOfType(type);
        }

        #endregion

        #region Load

        /// <summary>
        ///     Loads an assembly by its name
        /// </summary>
        /// <param name="name">Name of the assembly to return</param>
        /// <returns>The assembly specified if it exists</returns>
        public static Assembly Load(this AssemblyName name)
        {
            Guard.NotNull(name, "name");
            return AppDomain.CurrentDomain.Load(name);
        }

        #endregion

        #region LoadAssemblies

        /// <summary>
        ///     Loads assemblies within a directory and returns them in an array.
        /// </summary>
        /// <param name="directory">The directory to search in</param>
        /// <param name="recursive">Determines whether to search recursively or not</param>
        /// <returns>Array of assemblies in the directory</returns>
        public static IEnumerable<Assembly> LoadAssemblies(this DirectoryInfo directory, bool recursive = false)
        {
            return
                directory.GetFiles("*.dll", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                         .Select(file => AssemblyName.GetAssemblyName(file.FullName).Load());
        }

        #endregion

        #region MarkedWith

        /// <summary>
        ///     Goes through a list of types and determines if they're marked with a specific attribute
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="types">Types to check</param>
        /// <param name="inherit">When true, it looks up the heirarchy chain for the inherited custom attributes</param>
        /// <returns>The list of types that are marked with an attribute</returns>
        public static IEnumerable<Type> MarkedWith<T>(this IEnumerable<Type> types, bool inherit = true)
            where T : Attribute
        {
            if (types == null)
                return null;
            return types.Where(x => x.IsDefined(typeof (T), inherit) && !x.IsAbstract);
        }

        #endregion

        #region MakeShallowCopy

        /// <summary>
        ///     Makes a shallow copy of the object
        /// </summary>
        /// <param name="Object">Object to copy</param>
        /// <param name="simpleTypesOnly">If true, it only copies simple types (no classes, only items like int, string, etc.), false copies everything.</param>
        /// <returns>A copy of the object</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static T MakeShallowCopy<T>(this T Object, bool simpleTypesOnly = false)
        {
            if (Object == null)
                return default(T);
            Type objectType = Object.GetType();
            var classInstance = objectType.CreateInstance<T>();
            foreach (var property in objectType.GetProperties())
            {
                try
                {
                    if (property.CanRead
                        && property.CanWrite
                        && simpleTypesOnly
                        && property.PropertyType.IsValueType)
                        property.SetValue(classInstance, property.GetValue(Object, null), null);
                    else if (!simpleTypesOnly
                             && property.CanRead
                             && property.CanWrite)
                        property.SetValue(classInstance, property.GetValue(Object, null), null);
                }
                catch
                {
                }
            }

            foreach (var field in objectType.GetFields())
            {
                try
                {
                    if (simpleTypesOnly && field.IsPublic)
                        field.SetValue(classInstance, field.GetValue(Object));
                    else if (!simpleTypesOnly && field.IsPublic)
                        field.SetValue(classInstance, field.GetValue(Object));
                }
                catch
                {
                }
            }

            return classInstance;
        }

        #endregion

        #region SetProperty

        /// <summary>
        ///     Sets the value of destination property
        /// </summary>
        /// <param name="Object">The object to set the property of</param>
        /// <param name="property">The property to set</param>
        /// <param name="value">Value to set the property to</param>
        /// <param name="format">Allows for formatting if the destination is a string</param>
        public static object SetProperty(this object Object, PropertyInfo property, object value, string format = "")
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (property == null)
                throw new ArgumentNullException("property");
            if (value == null)
                throw new ArgumentNullException("value");
            if (property.PropertyType == typeof (string))
                value = value.FormatToString(format);
            //if(!Value.GetType().IsOfType(Property.PropertyType))
            //    Value=Convert.ChangeType(Value,Property.PropertyType);
            property.SetValue(Object, value.TryTo(property.PropertyType, null), null);
            return Object;
        }

        /// <summary>
        ///     Sets the value of destination property
        /// </summary>
        /// <param name="Object">The object to set the property of</param>
        /// <param name="property">The property to set</param>
        /// <param name="value">Value to set the property to</param>
        /// <param name="format">Allows for formatting if the destination is a string</param>
        public static object SetProperty(this object Object, string property, object value, string format = "")
        {
            if (Object == null)
                throw new ArgumentNullException("Object");
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            if (value == null)
                throw new ArgumentNullException("value");
            string[] properties = property.Split(new[] {"."}, StringSplitOptions.None);
            object tempObject = Object;
            Type tempObjectType = tempObject.GetType();
            PropertyInfo destinationProperty;
            for (int x = 0; x < properties.Length - 1; ++x)
            {
                destinationProperty = tempObjectType.GetProperty(properties[x]);
                tempObjectType = destinationProperty.PropertyType;
                tempObject = destinationProperty.GetValue(tempObject, null);
                if (tempObject == null)
                    return Object;
            }
            destinationProperty = tempObjectType.GetProperty(properties[properties.Length - 1]);
            tempObject.SetProperty(destinationProperty, value, format);
            return Object;
        }

        #endregion

        #region ToLongVersionString

        /// <summary>
        ///     Gets the long version of the version information
        /// </summary>
        /// <param name="assembly">Assembly to get version information from</param>
        /// <returns>The long version of the version information</returns>
        public static string ToLongVersionString(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            return assembly.GetName().Version.ToString();
        }

        #endregion

        #region ToShortVersionString

        /// <summary>
        ///     Gets the short version of the version information
        /// </summary>
        /// <param name="assembly">Assembly to get version information from</param>
        /// <returns>The short version of the version information</returns>
        public static string ToShortVersionString(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            Version versionInfo = assembly.GetName().Version;
            return versionInfo.Major + "." + versionInfo.Minor;
        }

        #endregion

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetTypesPublicProperties();

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetTypesPublicProperties()
                       .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                       .ToArray();
        }


        internal static PropertyInfo[] GetTypesPublicProperties(this Type subType)
        {
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance);
        }
    }
}