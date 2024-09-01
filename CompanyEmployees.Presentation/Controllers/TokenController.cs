using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController:ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public TokenController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDTO)
        {
            var tokenDtoToReturn = await _serviceManager.AuthenticationService.RefreshToken(tokenDTO);
            return Ok(tokenDtoToReturn);
        }
    }
}
