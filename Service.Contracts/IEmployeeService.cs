using Entities.LinkModels;
using Entities.Models;
using Shared.DTOs;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        //Task<(IEnumerable<EmployeeDTO> employees,MetaData metaData )> GetEmployeesAsync(Guid companyId,EmployeeParameters employeeParameters, bool trakChanges);
        Task<(LinkResponse linkResponse, MetaData metaData )> GetEmployeesAsync(Guid companyId,LinkParameters linkParameters , bool trakChanges);
        Task<EmployeeDTO> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trakChanges);
        Task<EmployeeDTO> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDTO employeeForCreation, bool trakChanges);
        Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges);
        //We are doing that because we won't track changes while fetching the company entity,
        //but we will track changes while fetching the employee.
        Task UpdateEmployeeForCompanyAsync(Guid companyId,Guid id,EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
        Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
        Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);

    }
}
