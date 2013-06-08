#region Usings

using System;
using System.IO;
using System.Linq;
using Yea.DataTypes.ExtensionMethods;
using Yea.Encryption;
using Yea.IO;
using Yea.IO.Serializers;
using Yea.Reflection;

#endregion

namespace Yea.Configuration
{
    /// <summary>
    ///     Config object
    /// </summary>
    [Serializable]
    public abstract class Config<TConfigClassType> : IConfig
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="stringToObject">String to object</param>
        /// <param name="objectToString">Object to string</param>
        protected Config(Func<string, TConfigClassType> stringToObject = null,
                         Func<IConfig, string> objectToString = null)
        {
            ObjectToString =
                objectToString.NullCheck(x => x.Serialize(new XMLSerializer(), fileLocation: ConfigFileLocation));
            StringToObject =
                stringToObject.NullCheck(x => (TConfigClassType) x.Deserialize(GetType(), new XMLSerializer()));
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Location to save/load the config file from.
        ///     If blank, it does not save/load but uses any defaults specified.
        /// </summary>
        protected virtual string ConfigFileLocation
        {
            get { return ""; }
        }

        /// <summary>
        ///     Encryption password for properties/fields. Used only if set.
        /// </summary>
        protected virtual string EncryptionPassword
        {
            get { return ""; }
        }

        /// <summary>
        ///     Gets the object
        /// </summary>
        private Func<string, TConfigClassType> StringToObject { get; set; }

        /// <summary>
        ///     Gets a string representation of the object
        /// </summary>
        private Func<IConfig, string> ObjectToString { get; set; }

        /// <summary>
        ///     Name of the config object
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region IConfig Members

        /// <summary>
        ///     Loads the config
        /// </summary>
        public void Load()
        {
            if (ConfigFileLocation.IsNullOrEmpty())
                return;
            string fileContent = new FileInfo(ConfigFileLocation).Read();
            if (string.IsNullOrEmpty(fileContent))
            {
                Save();
                return;
            }
            LoadProperties(StringToObject(fileContent));
            Decrypt();
        }

        /// <summary>
        ///     Saves the config
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

        private void LoadProperties(TConfigClassType temp)
        {
            if (temp.IsNull())
                return;
            foreach (var property in temp.GetType().GetProperties().Where(x => x.CanWrite && x.CanRead))
                this.SetProperty(property, temp.GetProperty(property));
        }

        private void Encrypt()
        {
            if (EncryptionPassword.IsNullOrEmpty())
                return;
            foreach (
                var property in
                    GetType().GetProperties().Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof (string)))
                this.SetProperty(property, ((string) this.GetProperty(property)).Encrypt(EncryptionPassword));
        }

        private void Decrypt()
        {
            if (EncryptionPassword.IsNullOrEmpty())
                return;
            foreach (
                var property in
                    GetType().GetProperties().Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof (string)))
                this.SetProperty(property, ((string) this.GetProperty(property)).Decrypt(EncryptionPassword));
        }

        #endregion
    }
}