using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DTOs;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;
        
        public EmployeeService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IMapper mapper,IEmployeeLinks employeeLinks)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
            _mapper=mapper;
            _employeeLinks = employeeLinks;
            

        }

        public async Task<EmployeeDTO> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trakChanges)
        {
            /*var company = await _repositoryManager.Company.GetCompanyAsync(companyId, trakChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employee=await _repositoryManager.Employee.GetEmployeeAsync(companyId, employeeId, trakChanges);
            if (employee is null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }*/
            
            await CheckIfCompanyExists(companyId, trakChanges);
            var employee=await GetEmployeeForCompanyAndCheckIfItExists(companyId,employeeId, trakChanges);
            var employeeDTO= _mapper.Map<EmployeeDTO>(employee);
            return employeeDTO;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId,LinkParameters linkParameters, bool trakChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
            {
                throw new MaxAgeRangeBadRequestException();
            }
            await CheckIfCompanyExists(companyId, trakChanges);
            var employeesWithMetaData = await _repositoryManager.Employee.GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trakChanges);
            var employeesDTO = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesWithMetaData);
            //var shapedData = _dataShaper.ShapeData(employeesDTO, employeeParameters.Fields);
            //return (employees: employeesDTO, metaData: employeesWithMetaData.MetaData);
            var links = _employeeLinks.TryGenerateLinks(employeesDTO, linkParameters.EmployeeParameters.Fields, companyId, linkParameters.Context);
            return (linkResponse:links, metaData: employeesWithMetaData.MetaData);

        }
        public async Task<EmployeeDTO> CreateEmployeeForCompanyAsync(Guid CompanyId, EmployeeForCreationDTO employeeForCreation, bool trakChanges)
        {
            await CheckIfCompanyExists(CompanyId, trakChanges);
            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
            _repositoryManager.Employee.CreateEmployeeForCompany(CompanyId,employeeEntity);
            await _repositoryManager.SaveAsync();
            var employeeDTO=_mapper.Map<EmployeeDTO>(employeeEntity);
            return employeeDTO;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeForCompany = await GetEmployeeForCompanyAndCheckIfItExists(companyId,id,trackChanges);
            _repositoryManager.Employee.DeleteEmployee(employeeForCompany);
            await _repositoryManager.SaveAsync();

        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges,bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);
            // if trackchanges is true This means we change any property in this entity,
            // EF Core will set the state of that entity to Modified.
            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            // this changing the state of the employeeEntity object to Modified.
            //connected update:fetch data and update it with same context
            _mapper.Map(employeeForUpdate,employeeEntity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);
            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            return (employeeToPatch, employeeEntity);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repositoryManager.SaveAsync();
        }
        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges) 
        {
            var employeeDb = await _repositoryManager.Employee.GetEmployeeAsync(companyId, id, trackChanges); 
            if (employeeDb is null)
            {
                throw new EmployeeNotFoundException(id);
            } 
            return employeeDb; 
        }
    }
}