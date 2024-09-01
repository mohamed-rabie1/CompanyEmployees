using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ActionFilters
{
    public class ValidateMediaTypeAttribute : IActionFilter
    {
        

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // check if accept header else return bad request
            var acceptHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Accept");
            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult("Accept header is missing");
                return;
            }
            //we parse the media type — and if there is no valid media type present, we return BadRequest.
            var mediaType = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                context.Result= new BadRequestObjectResult($"Media type not present. Please add Accept header with the required media type."); 
                return;

            }
            //we pass the parsed media type to the HttpContext of the controller.
            context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
