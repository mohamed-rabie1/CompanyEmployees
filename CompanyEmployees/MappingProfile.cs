using AutoMapper;
using Entities.Models;
using Shared.DTOs;

namespace CompanyEmployees
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDTO>().ForMember(c=>c.FullAddress,
                opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
            //CreateMap<Company, CompanyDTO>().ForCtorParam("FullAddress",  
               // opt=>opt.MapFrom(x=> string.Join(' ',x.Address,x.Country))); // for diffrent properties
            
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<CompanyForCreationDTO,Company>();
            CreateMap<EmployeeForCreationDTO, Employee>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
            CreateMap<CompanyForUpdateDto, Company>().ReverseMap();
            CreateMap<UserForRegistrationDTO, User>();
            
        }
    }
}
