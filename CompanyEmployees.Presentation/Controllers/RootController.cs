using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    //place where consumers can learn how to interact with the rest of the API.
    [Route("api/[controller]")]
    [ApiController]
    public class RootController:ControllerBase
    {
        //to generate links towards the API actions
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        [HttpGet("/1")]
        public IActionResult Get()
        {
            return Ok("Done");
        }
        [HttpGet(Name ="GetRoot")]
        public IActionResult GetRoot([FromHeader(Name ="Accept")]string mediaType)
        {
            if (mediaType.Contains("application/vnd.codemaze.apiroot"))
            {
                var list = new List<Link>()
                {
                    new Link()
                    {
                        Href=_linkGenerator.GetUriByName(httpContext:HttpContext,endpointName:nameof(GetRoot),values:new{}),
                        Rel="self",
                        Method="Get"
                    },
                    new Link()
                    {
                        Href=_linkGenerator.GetUriByName(httpContext:HttpContext,endpointName:"GetCompanies",values:new{}),
                        Rel="Companies",
                        Method="Get"
                    },
                    new Link()
                    {
                        Href=_linkGenerator.GetUriByName(httpContext:HttpContext,endpointName:"CreateCompany",values:new{}),
                        Rel="Create_Company",
                        Method="Post"
                    }
                };
                return Ok(list);
            }
            return NoContent();
        }
    }
}
