using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ActionFilters
{
    public class ValidationFilterAttribute:IActionFilter
    {
        public ValidationFilterAttribute()
        {
            
        }
        // excuted after action method
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
        // excuted before action method
        //context parameter to retrieve different values that we need inside this method
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //With the RouteData.Values dictionary,
            //we can get the values produced by routes on the current routing path
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];
            // the ActionArguments dictionary to extract the DTO parameter that we send to the POST and PUT actions
            var param = context.ActionArguments.SingleOrDefault(x => x.Value.ToString().Contains("DTO")).Value;
            if (param is null) 
            {
                //set the Result property of the context object to a new instance of the BadRequestObjectReturnResult
                context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, action: {action}");
                return;
            }
            if (!context.ModelState.IsValid) 
            {
                //create a new instance of the UnprocessableEntityObjectResult class and pass ModelState
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        } 
    }
}
