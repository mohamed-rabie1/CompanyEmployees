using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public abstract record EmployeeForManipulationDto
    {

        [Required(ErrorMessage = "Employee name is required field")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters")]
        public string? Name { get; init; } // use init setter because normal record lose readability

        [Range(18, int.MaxValue, ErrorMessage = "Age is required and it can't be lower than 18")]
        public int Age { get; init; } // if you don't send value default vaule will be sent

        [Required(ErrorMessage = "Position is required field")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters")]
        public string? Position { get; init; }
    }
}
