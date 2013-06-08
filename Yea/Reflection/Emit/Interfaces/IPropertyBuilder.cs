#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#endregion

namespace Yea.Reflection.Emit.Interfaces
{
    /// <summary>
    ///     Interface for properties
    /// </summary>
    public interface IPropertyBuilder
    {
        #region Properties

        /// <summary>
        ///     Property name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Data type
        /// </summary>
        Type DataType { get; }

        /// <summary>
        ///     Property builder
        /// </summary>
        System.Reflection.Emit.PropertyBuilder Builder { get; }

        /// <summary>
        ///     Attributes for the property
        /// </summary>
        PropertyAttributes Attributes { get; }

        /// <summary>
        ///     Attributes for the get method
        /// </summary>
        MethodAttributes GetMethodAttributes { get; }

        /// <summary>
        ///     Attributes for the set method
        /// </summary>
        MethodAttributes SetMethodAttributes { get; }

        /// <summary>
        ///     Method builder for the get method
        /// </summary>
        MethodBuilder GetMethod { get; }

        /// <summary>
        ///     Method builder for the set method
        /// </summary>
        MethodBuilder SetMethod { get; }

        #endregion

        #region Functions

        /// <summary>
        ///     Gets the definition of the variable
        /// </summary>
        /// <returns>string representation of the variable definition</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetDefinition();

        #endregion
    }
}