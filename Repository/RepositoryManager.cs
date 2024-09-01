using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    // unitofwork
    public class RepositoryManager : IRepositoryManager
    {
        // Lazy loading:improve performance by loading only the data that is necessary 
        // eager loading: loading related data in advance
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ICompanyRepository> _companyRepository;
        private readonly Lazy<IEmployeeRepository> _employeeRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(repositoryContext));
            _employeeRepository= new Lazy<IEmployeeRepository>(() => new EmployeeRepository(repositoryContext));
        }

        public ICompanyRepository Company
        {
            get {  return _companyRepository.Value; }  
        }
        public IEmployeeRepository Employee
        {
            get { return _employeeRepository.Value; }
        }
        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
