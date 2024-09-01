using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record CompanyDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? FullAddress { get; set; }

    }
    //[Serializable]
    //public record CompanyDTO(Guid Id,string Name,string FullAddress);
}
