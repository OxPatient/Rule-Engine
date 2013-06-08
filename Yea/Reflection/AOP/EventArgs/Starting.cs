#region Usings

using System.Collections.Generic;

#endregion

namespace Yea.Reflection.AOP.EventArgs
{
    /// <summary>
    ///     EventArgs used during the start event
    /// </summary>
    public class Starting : System.EventArgs
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        public Starting()
        {
            MethodName = "";
            Parameters = new List<object>();
            ReturnValue = null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Method Name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        ///     Parameter list
        /// </summary>
        public ICollection<object> Parameters { get; private set; }

        /// <summary>
        ///     Return value
        /// </summary>
        public object ReturnValue { get; set; }

        #endregion
    }
}