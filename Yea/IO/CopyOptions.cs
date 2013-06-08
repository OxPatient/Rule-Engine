namespace Yea.IO
{
    /// <summary>
    ///     Options used in directory copying
    /// </summary>
    public enum CopyOptions
    {
        /// <summary>
        ///     Copy if newer than the DateTime specified
        /// </summary>
        CopyIfNewer,

        /// <summary>
        ///     Copy always
        /// </summary>
        CopyAlways,

        /// <summary>
        ///     Do not overwrite a file
        /// </summary>
        DoNotOverwrite
    }
}