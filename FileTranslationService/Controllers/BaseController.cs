using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Extensions;
using Tilde.MT.FileTranslationService.Models.Errors;

namespace Tilde.MT.FileTranslationService.Controllers
{
    public class BaseController: Controller
    {
        protected ObjectResult FormatAPIError(HttpStatusCode status, ErrorSubCode subcode, HttpStatusCode? messageStatusCode = null)
        {
            return StatusCode(
                (int)status,
                new APIError()
                {
                    Error = new Error()
                    {
                        Code = (int)(messageStatusCode ?? status) * 1000 + (int)subcode,
                        Message = subcode.Description()
                    }
                }
            );
        }
    }
}
