using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DTOs;

namespace CompanyEmployees.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDTO> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator,IDataShaper<EmployeeDTO> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }
        // employeesDto,fields to shape collection
        // companyId : routes to the employee resources contain the Id from the company
        // httpContext holds information about media types.
        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDTO> employeesDto, string fields, Guid companyId, HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeesDto, fields);
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkdedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);
            }
            return ReturnShapedEmployees(shapedEmployees);
        }
        //method executes data shaping and extracts only the entity part without the Id property.
        private List<Entity> ShapeData(IEnumerable<EmployeeDTO> employeesDto, string fields)
        {
            var entities= _dataShaper.ShapeData(employeesDto, fields)
                .Select(x => x.Entity)
                .ToList();
            return entities;
        }

        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees)
        {
            return new LinkResponse() { ShapedEntities = shapedEmployees };
        }

        private LinkResponse ReturnLinkdedEmployees(IEnumerable<EmployeeDTO> employeesDto, string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
        {
            var employeeDtoList = employeesDto.ToList();
            //iterate through each employee and create links for it
            //add it to the shapedEmployees collection
            for (int index = 0; index < employeeDtoList.Count(); index++)
            {
                var employeeLinks= CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
                shapedEmployees[index].Add("Links", employeeLinks);
            }
            var employeeCollection= new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployee(httpContext, employeeCollection);
            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEmployee(HttpContext httpContext, LinkCollectionWrapper<Entity> employeesWrapper)
        {
            employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany", values: new { }), "self", "GET")); 
            return employeesWrapper;
        }

        private object CreateLinksForEmployee(HttpContext httpContext, Guid companyId, object id, string fields)
        {
            var links = new List<Link> 
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", values: new { companyId, id, fields }), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "CreateEmployeeForCompany", values: new { companyId }), "create_employee", "Post"), 
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany", values: new { companyId, id }), "delete_employee", "DELETE"), 
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany", values: new { companyId, id }), "update_employee", "PUT"), 
                new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany", values: new { companyId, id }), "partially_update_employee", "PATCH") 
            }; 
            return links;
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            //extract media type from httpcontext
            var mediaType =(MediaTypeHeaderValue?) httpContext.Items["AcceptHeaderMediaType"];
            // if media type end withh hateas return true else false
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
