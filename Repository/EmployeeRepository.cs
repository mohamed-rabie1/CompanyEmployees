using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Extensions;
namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee> , IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {
            
        }

        public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trakChanges)
        {
            return await FindByCondition(e => e.CompanyId == companyId && e.Id == employeeId, trakChanges).SingleOrDefaultAsync();
        }

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId , EmployeeParameters employeeParameters, bool trakChanges)
        {
            var emplyees= await FindByCondition(e => e.CompanyId == companyId, trakChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .ToListAsync();
            return PagedList<Employee>.ToPagedList(emplyees,employeeParameters.PageNumber,employeeParameters.PageSize);
        }
        public void CreateEmployeeForCompany(Guid CompanyId, Employee employee)
        {
            employee.CompanyId= CompanyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }
    }
}
