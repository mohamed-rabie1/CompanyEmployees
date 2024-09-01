using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    //[Route("api/{v:apiversion}/companies")]
    [Route("api/[controller]")]
    [ApiController]
    //able to work with that API, but we will be notified that this version is deprecated
    //[ApiVersion("2.0",Deprecated =true)]
    //[ApiExplorerSettings(GroupName ="V2")]
    public class CompaniesV2Controller:ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesV2Controller(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies =await _serviceManager.CompanyService.GetAllCompaniesAsync(trakChanges: false);
            return Ok(companies);
        }
    }
}
