#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Yea.Reflection;

#endregion

namespace Yea.Configuration
{
    /// <summary>
    ///     Config manager
    /// </summary>
    public class ConfigurationManager
    {
        #region Constructor

        #endregion

        #region Public Functions

        #region RegisterConfigFile

        /// <summary>
        ///     Registers a config file
        /// </summary>
        /// <typeparam name="TConfigType">The config object type to register</typeparam>
        public static void RegisterConfigFile<TConfigType>() where TConfigType : Config<TConfigType>, new()
        {
            RegisterConfigFile(new TConfigType());
        }

        /// <summary>
        ///     Registers a config file
        /// </summary>
        /// <param name="configObject">Config object to register</param>
        /// <exception cref="ArgumentNullException">configObject</exception>
        public static void RegisterConfigFile(IConfig configObject)
        {
            if (configObject == null) throw new ArgumentNullException("configObject");
            if (ConfigFiles.ContainsKey(configObject.Name)) return;
            configObject.Load();
            ConfigFiles.Add(configObject.Name, configObject);
        }

        /// <summary>
        ///     Registers a set of config file
        /// </summary>
        /// <param name="configObjects">Config objects to register</param>
        /// <exception cref="ArgumentNullException">configObjects</exception>
        public static void RegisterConfigFile(IEnumerable<IConfig> configObjects)
        {
            if (configObjects == null) throw new ArgumentNullException("configObjects");
            foreach (var configObject in configObjects)
                RegisterConfigFile(configObject);
        }

        /// <summary>
        ///     Registers all config files in an assembly
        /// </summary>
        /// <param name="assemblyContainingConfig">Assembly to search</param>
        /// <exception cref="ArgumentNullException">assemblyContainingConfig</exception>
        public static void RegisterConfigFile(Assembly assemblyContainingConfig)
        {
            if (assemblyContainingConfig == null) throw new ArgumentNullException("assemblyContainingConfig");
            RegisterConfigFile(assemblyContainingConfig.GetObjects<IConfig>());
        }

        #endregion

        #region GetConfigFile

        /// <summary>
        ///     Gets a specified config file
        /// </summary>
        /// <typeparam name="T">Type of the config object</typeparam>
        /// <param name="name">Name of the config object</param>
        /// <returns>The config object specified</returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetConfigFile<T>(string name)
        {
            if (!ConfigFiles.ContainsKey(name))
                throw new ArgumentException("The config object " + name + " was not found.");
            if (!(ConfigFiles[name] is T))
                throw new ArgumentException("The config object " + name + " is not the specified type.");
            return (T) ConfigFiles[name];
        }

        #endregion

        #region ContainsConfigFile

        /// <summary>
        ///     Determines if a specified config file is registered
        /// </summary>
        /// <typeparam name="T">Type of the config object</typeparam>
        /// <param name="name">Name of the config object</param>
        /// <returns>The config object specified</returns>
        public static bool ContainsConfigFile<T>(string name)
        {
            return ConfigFiles.ContainsKey(name) && ConfigFiles[name] is T;
        }

        #endregion

        #region ToString

        /// <summary>
        ///     Outputs all of the configuration items as an HTML list
        /// </summary>
        /// <returns>All configs as a string list</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<ul>").Append("<li>").Append(ConfigFiles.Count).Append("</li>");
            foreach (var name in ConfigFiles.Keys)
                builder.Append("<li>")
                       .Append(name)
                       .Append(":")
                       .Append(ConfigFiles[name].GetType().FullName)
                       .Append("</li>");
            builder.Append("</ul>");
            return builder.ToString();
        }

        #endregion

        #endregion

        #region Private fields

        private static readonly Dictionary<string, IConfig> ConfigFiles = new Dictionary<string, IConfig>();

        #endregion
    }
}