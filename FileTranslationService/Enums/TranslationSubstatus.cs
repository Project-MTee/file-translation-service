namespace Tilde.MT.FileTranslationService.Enums
{
    public enum TranslationSubstatus
    {
        /// <summary>
        /// Status sub code not specified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Bad file contents or format 
        /// </summary>
        BadFileError = 1,

        /// <summary>
        /// File type not implemented
        /// </summary>
        UnknownFileTypeError = 2,
        
        /// <summary>
        /// Track changes are enabled, disable to translate
        /// </summary>
        TrackChangesEnabledError = 3,

        /// <summary>
        /// No translatable text found
        /// </summary>
        NoTextExtractedError = 4
    }
}
