using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTOs;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId:guid}/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public EmployeesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        [HttpHead]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);

            var pagedResult = await _serviceManager.EmployeeService.GetEmployeesAsync(companyId, linkParams, trakChanges: false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return pagedResult.linkResponse.HasLinks ? Ok(pagedResult.linkResponse.LinkedEntities) : Ok(pagedResult.linkResponse.ShapedEntities);
            
        }
        [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            var employee = await _serviceManager.EmployeeService.GetEmployeeAsync(companyId, employeeId, trakChanges: false);
            return Ok(employee);
        }
        [HttpPost(Name = "CreateEmployeeForCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDTO employee)
        {
            /*if(employee is null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            //unable to process validation rules applied on the entity inside the request body.
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }*/
            var createdEmployee = await _serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trakChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, employeeId = createdEmployee.Id }, createdEmployee);
        }

        [HttpDelete("{id:guid}",Name = "DeleteEmployeeForCompany")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            await _serviceManager.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id:guid}",Name = "UpdateEmployeeForCompany")] // put is for full update
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            /*if(employee is null)
            {
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }*/
            await _serviceManager.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, compTrackChanges: false, empTrackChanges: true);
            return NoContent();
        }

        [HttpPatch("{id:guid}",Name = "PartiallyUpdateEmployeeForCompany")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("patchDoc object sent from client is null.");
            }
            var result = await _serviceManager.EmployeeService.GetEmployeeForPatchAsync(companyId, id, false, true);
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);
            TryValidateModel(result.employeeToPatch);
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            await _serviceManager.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }

    }
}
