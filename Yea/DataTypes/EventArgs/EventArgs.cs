namespace Yea.DataTypes.EventArgs
{

    #region Event Args

    /// <summary>
    ///     Base event args for the events used in the system
    /// </summary>
    public class BaseEventArgs : System.EventArgs
    {
        /// <summary>
        ///     Should the event be stopped?
        /// </summary>
        public bool Stop { get; set; }

        /// <summary>
        ///     Content of the event
        /// </summary>
        public object Content { get; set; }
    }

    /// <summary>
    ///     Saved event args
    /// </summary>
    public class SavedEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Saving event args
    /// </summary>
    public class SavingEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Deleted event args
    /// </summary>
    public class DeletedEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Deleting event args
    /// </summary>
    public class DeletingEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Changed event args
    /// </summary>
    public class ChangedEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Loaded event args
    /// </summary>
    public class LoadedEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     Loading event args
    /// </summary>
    public class LoadingEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     On start event args
    /// </summary>
    public class OnStartEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     On end event args
    /// </summary>
    public class OnEndEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    ///     On error event args
    /// </summary>
    public class OnErrorEventArgs : BaseEventArgs
    {
    }

    #endregion
}