using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record UserForRegistrationDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "UserName is rquired ")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")] 
        public string? Password { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public ICollection<string>? Roles { get; init; }
    }
}
