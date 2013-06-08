#region Usings

using System;
using Yea.Reflection.AOP.EventArgs;
using Exception = Yea.Reflection.AOP.EventArgs.Exception;

#endregion

namespace Yea.Reflection.AOP
{
    /// <summary>
    ///     Events interface (injected into all objects)
    /// </summary>
    public interface IEvents
    {
        #region Events

        /// <summary>
        ///     Called when property/function is ending
        /// </summary>
        EventHandler<Ending> AspectusEnding { get; set; }

        /// <summary>
        ///     Called when property/function is starting
        /// </summary>
        EventHandler<Starting> AspectusStarting { get; set; }

        /// <summary>
        ///     Called when an error is caught
        /// </summary>
        EventHandler<Exception> AspectusException { get; set; }

        #endregion
    }
}