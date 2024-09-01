using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(IRepositoryManager repositoryManager,ILoggerManager loggerManager,IMapper mapper,IEmployeeLinks employeeLinks,UserManager<User> userManager,IOptions<JwtConfiguration> configuration)
        {
            //lazy loading:the object it wraps is not created until it is first accessed.
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, loggerManager , mapper, employeeLinks));
            _companyService= new Lazy<ICompanyService>(()=> new CompanyService(repositoryManager,loggerManager, mapper));
            // lazy constructor take delegate(lambda exp) that return object of class AuthenticationService
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(mapper, userManager,configuration));
        }
        public IEmployeeService EmployeeService
        {
            get { return _employeeService.Value; } 
        }

        public ICompanyService CompanyService
        {
            get { return _companyService.Value; }
        }

        public IAuthenticationService AuthenticationService
        {
            get { return _authenticationService.Value; }
        } 
    }
}
