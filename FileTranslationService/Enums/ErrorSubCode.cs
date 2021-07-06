using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Enums
{
    public enum ErrorSubCode
    {
        #region Task errors

        [Description("An unexpected error occured")]
        GatewayGeneric = 1,

        [Description("Task is not found")]
        GatewayTaskNotFound = 2,

        [Description("File not found")]
        GatewayFileNotFound = 3,

        [Description("File with same category is already uploaded")]
        GatewayTaskFileConflict = 4,

        [Description("Source file is forbidden to download")]
        GatewaySourceFileDownloadForbidden = 5,

        #endregion

        [Description("Failed to verify language direction")]
        GatewayLanguageDirectionGeneric = 6,

        [Description("Language direction is not found")]
        GatewayLanguageDirectionNotFound = 7,

        [Description("Request too large")]
        GatewayRequestTooLarge = 8,

        [Description("Request validation failed")]
        GatewayRequestValidation = 9,

        [Description("File extension unsupported")]
        GatewayMediaTypeNotValid = 10
    }
}
