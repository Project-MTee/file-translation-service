namespace Tilde.MT.FileTranslationService.Enums
{
    /// <summary>
    /// File categories of File translation
    /// </summary>
    public enum FileCategory
    {
        /// <summary>
        /// Original source file
        /// </summary>
        Source = 1,
        /// <summary>
        /// Source converted to editable format.
        /// </summary>
        SourceConverted = 2,
        /// <summary>
        /// Translated file
        /// </summary>
        Translated = 3,
        /// <summary>
        /// Translated file in original format
        /// </summary>
        TranslatedConverted = 4,
    }
}
