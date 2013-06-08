using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Yea.Configuration
{
    /// <summary>
    /// Config object
    /// </summary>
    [Serializable()]
    public abstract class Config<ConfigClassType> : IConfig
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StringToObject">String to object</param>
        /// <param name="ObjectToString">Object to string</param>
        protected Config(Func<string, ConfigClassType> StringToObject = null, Func<IConfig, string> ObjectToString = null)
        {
            this.ObjectToString = ObjectToString.NullCheck((x) => x.Serialize(new XMLSerializer(), FileLocation: ConfigFileLocation));
            this.StringToObject = StringToObject.NullCheck((x) => (ConfigClassType)x.Deserialize(this.GetType(), new XMLSerializer()));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Location to save/load the config file from.
        /// If blank, it does not save/load but uses any defaults specified.
        /// </summary>
        protected virtual string ConfigFileLocation { get { return ""; } }

        /// <summary>
        /// Encryption password for properties/fields. Used only if set.
        /// </summary>
        protected virtual string EncryptionPassword { get { return ""; } }

        /// <summary>
        /// Gets the object
        /// </summary>
        private Func<string, ConfigClassType> StringToObject { get; set; }

        /// <summary>
        /// Gets a string representation of the object
        /// </summary>
        private Func<IConfig, string> ObjectToString { get; set; }

        /// <summary>
        /// Name of the config object
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region IConfig Members

        /// <summary>
        /// Loads the config
        /// </summary>
        public void Load()
        {
            if (ConfigFileLocation.IsNullOrEmpty())
                return;
            string FileContent = new FileInfo(ConfigFileLocation).Read();
            if (string.IsNullOrEmpty(FileContent))
            {
                Save();
                return;
            }
            LoadProperties(StringToObject(FileContent));
            Decrypt();
        }

        /// <summary>
        /// Saves the config
        /// </summary>
        public void Save()
        {
            if (ConfigFileLocation.IsNullOrEmpty())
                return;
            Encrypt();
            new FileInfo(ConfigFileLocation).Save(ObjectToString(this));
            Decrypt();
        }

        #endregion

        #region Private Functions

        private void LoadProperties(ConfigClassType Temp)
        {
            if (Temp.IsNull())
                return;
            foreach (PropertyInfo Property in Temp.GetType().GetProperties().Where(x => x.CanWrite && x.CanRead))
                this.SetProperty(Property, Temp.GetProperty(Property));
        }

        private void Encrypt()
        {
            if (EncryptionPassword.IsNullOrEmpty())
                return;
            foreach (PropertyInfo Property in this.GetType().GetProperties().Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof(string)))
                this.SetProperty(Property, ((string)this.GetProperty(Property)).Encrypt(EncryptionPassword));
        }

        private void Decrypt()
        {
            if (EncryptionPassword.IsNullOrEmpty())
                return;
            foreach (PropertyInfo Property in this.GetType().GetProperties().Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof(string)))
                this.SetProperty(Property, ((string)this.GetProperty(Property)).Decrypt(EncryptionPassword));
        }

        #endregion
    }
}