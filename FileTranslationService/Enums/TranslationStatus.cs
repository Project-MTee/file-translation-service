namespace Tilde.MT.FileTranslationService.Enums
{
    /// <summary>
    /// Translation status types
    /// </summary>
    public enum TranslationStatus
    {
        #region Initializing

        /// <summary>
        /// Translation is scheduled
        /// </summary>
        Queuing = 1,

        #endregion

        #region Processing status:

        /// <summary>
        /// Translation is initializing
        /// </summary>
        Initializing = 2,
        /// <summary>
        /// Converting file into translation compatable format
        /// </summary>
        Extracting = 3,
        /// <summary>
        /// Waiting for MT system to become available
        /// </summary>
        Waiting = 4,
        /// <summary>
        /// Translation of file
        /// </summary>
        Translating = 5,
        /// <summary>
        /// Saving translation from compatable formats to user desired
        /// </summary>
        Saving = 6,

        #endregion

        #region Finished status:

        /// <summary>
        /// File translation was sucessfull
        /// </summary>
        Completed = 7,
        /// <summary>
        /// Translation failed
        /// </summary>
        /// 
        Error = 8

        #endregion
    }
}
