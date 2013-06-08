namespace Yea.Reflection.AOP.EventArgs
{
    /// <summary>
    ///     EventArgs used during the exception event
    /// </summary>
    public class Exception : System.EventArgs
    {
        #region Properties

        /// <summary>
        ///     Exception that was thrown
        /// </summary>
        public System.Exception InternalException { get; set; }

        #endregion
    }
}