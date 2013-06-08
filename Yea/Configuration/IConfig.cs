namespace Yea.Configuration
{
    /// <summary>
    ///     Config interface
    /// </summary>
    public interface IConfig
    {
        #region Properties

        /// <summary>
        ///     Name of the config file
        /// </summary>
        string Name { get; }

        #endregion

        #region Functions

        /// <summary>
        ///     Loads the config file
        /// </summary>
        void Load();

        /// <summary>
        ///     Saves the config file
        /// </summary>
        void Save();

        #endregion
    }
}