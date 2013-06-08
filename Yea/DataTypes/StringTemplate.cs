#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;
using Yea.DataTypes.ExtensionMethods;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Acts as a template for a string
    /// </summary>
    [Serializable]
    public class StringTemplate : Dictionary<string, string>
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="Template">Template</param>
        /// <param name="KeyEnd">Ending signifier of a key</param>
        /// <param name="KeyStart">Starting signifier of a key</param>
        public StringTemplate(string Template, string KeyStart = "{", string KeyEnd = "}")
        {
            this.KeyStart = KeyStart;
            this.KeyEnd = KeyEnd;
            this.Template = Template;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="Info">Serialization info</param>
        /// <param name="Context">Streaming context</param>
        protected StringTemplate(SerializationInfo Info, StreamingContext Context)
            : base(Info, Context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Template
        /// </summary>
        public string Template { get; protected set; }

        /// <summary>
        ///     Beginning signifier of a key
        /// </summary>
        public string KeyStart { get; protected set; }

        /// <summary>
        ///     Ending signifier of a key
        /// </summary>
        public string KeyEnd { get; protected set; }

        #endregion

        #region Functions

        /// <summary>
        ///     Applies the key/values to the template and returns the resulting string
        /// </summary>
        /// <returns>The resulting string</returns>
        public override string ToString()
        {
            return
                Template.FormatString(
                    this.ToArray(x => new KeyValuePair<string, string>(KeyStart + x.Key + KeyEnd, x.Value)));
        }

        /// <summary>
        ///     Implements the ISerializable interface and returns the data needed to serialize the dictionary instance
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        [SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion
    }
}