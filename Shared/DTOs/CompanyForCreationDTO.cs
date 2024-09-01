using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    //public record CompanyForCreationDTO(string Name,string Address,string Country,IEnumerable<EmployeeForCreationDTO> Employees);
    public record CompanyForCreationDTO : CompanyForManipulationDto
    {
        //public IEnumerable<EmployeeForCreationDTO>? Employees { get; init; }
    }
}
