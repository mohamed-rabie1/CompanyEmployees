using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.Exceptions;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
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
    //[ServiceFilter(typeof(ControllerFilterExample),Order =2)] action filter for specific controller
    //[Route("api/{v:apiversion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiVersion("1.0")]
    [Authorize]
    //[ApiExplorerSettings(GroupName ="V1")]
    public class CompaniesController:ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CompaniesController(IServiceManager serviceManager)
        {
            _serviceManager= serviceManager;
        }
        //[ServiceFilter(typeof(ControllerFilterExample))] action filter for specific action
        [HttpGet(Name = "GetCompanies")] // Attribute routing
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllCompanies()
        {
            
            var companies=await _serviceManager.CompanyService.GetAllCompaniesAsync(false);
            return Ok(companies);
            
            
        }

        [HttpGet("{id:Guid}",Name = "CompanyById")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [HttpCacheValidation(MustRevalidate = true)]
        [HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =120)]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var company= await _serviceManager.CompanyService.GetCompanyAsync(id,false);
           
            return Ok(company);
        }

        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDTO company)
        {
           
            /*if (company is null) //if it can’t be deserialized
            {
                return BadRequest("CompanyForCreationDto object is null");
            }
            
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }*/
            var createdCompany=await _serviceManager.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")] 
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(binderType:typeof(ArrayModelBinder))]IEnumerable<Guid> ids) 
        { 
            var companies = await _serviceManager.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies); 
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDTO> companyCollection)
        {
            var result=await _serviceManager.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _serviceManager.CompanyService.DeleteCompanyAsync(id, false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id,CompanyForUpdateDto company)
        {
            /*if (company is null)
            {
                return BadRequest("CompanyForUpdateDto object is null");
            }
            //unable to process validation rules applied on the entity inside the request body.
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }*/
            await _serviceManager.CompanyService.UpdateCompanyAsync(id, company,trackChanges:true);
            return NoContent();
        }
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
