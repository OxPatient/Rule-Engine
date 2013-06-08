#region Usings

using System.Collections.Generic;

#endregion

namespace Yea.Reflection.AOP.EventArgs
{
    /// <summary>
    ///     EventArgs used in ending event
    /// </summary>
    public class Ending : System.EventArgs
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public Ending()
        {
            MethodName = "";
            Parameters = new List<object>();
            ReturnValue = null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        ///     Parameters list
        /// </summary>
        public ICollection<object> Parameters { get; private set; }

        /// <summary>
        ///     Return value
        /// </summary>
        public object ReturnValue { get; set; }

        #endregion
    }
}