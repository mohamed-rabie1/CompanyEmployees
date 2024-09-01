using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    //Id property will be accepted through the URI
    public record EmployeeForUpdateDto:EmployeeForManipulationDto;
}
