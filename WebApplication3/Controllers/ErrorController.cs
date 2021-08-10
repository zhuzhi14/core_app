using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Helper;

namespace WebApplication3.Controllers
{
    [ApiController]
   
    public class ErrorController : Controller
    {
        [Route("/error-local-development")]
        [ApiExplorerSettings(IgnoreApi =true)]
        public ReturnData<int> ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return new ReturnData<int>(500, context.Error.Message,new List<int>());
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi =true)]
        public IActionResult Error() => Problem();
    }
}