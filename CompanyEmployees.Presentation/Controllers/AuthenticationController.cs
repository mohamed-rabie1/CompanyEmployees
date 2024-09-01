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
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthenticationController:ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AuthenticationController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody]UserForRegistrationDTO userForRegistrationDTO)
        {
            var result=await _serviceManager.AuthenticationService.RegisterUser(userForRegistrationDTO);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);//created
        }
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody]UserForAuthenticationDTO user)
        {
            if (!await _serviceManager.AuthenticationService.ValidateUser(user))
            {
                return Unauthorized();
            }
            var tokenDTO = await _serviceManager.AuthenticationService.CreateToken(populateExp:true);

            return Ok(tokenDTO);
        }
        
    }
}
