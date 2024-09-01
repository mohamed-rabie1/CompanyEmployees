using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trakChanges);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId,bool trakChanges);
        void CreateEmployeeForCompany(Guid CompanyId,Employee employee);
        void DeleteEmployee(Employee employee);
    }
}
