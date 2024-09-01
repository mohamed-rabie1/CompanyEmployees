using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<EmployeeDTO> employeesDto, string fields
            , Guid companyId, HttpContext httpContext);
    }
}
