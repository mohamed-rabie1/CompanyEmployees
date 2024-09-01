using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RepositoryContext _repositoryContext;
        public ICompanyRepository CompanyRepository { get; private set; }

        public IEmployeeRepository EmployeeRepository { get; private set; }

        public UnitOfWork(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            CompanyRepository = new CompanyRepository(_repositoryContext);
            EmployeeRepository = new EmployeeRepository(_repositoryContext);
        }
       
        public void Save()
        {
            _repositoryContext.SaveChanges();
        }
    }
}
