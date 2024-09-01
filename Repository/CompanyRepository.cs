using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            
        }
        //async: enable await if we use async without await method is still synch method
        //await: stop async method(GetAllCompanies) and return incompelete task
        //in this duration thread is back to threaad pool
        // when know the operation is compelete by Task continue excution
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trakChanges)
        {
           return await FindAll(trakChanges).OrderBy(c => c.Name).ToListAsync();
           
        }

        public async Task<Company> GetCompanyAsync(Guid id, bool trakChanges)
        {
            return await FindByCondition(c => c.Id==id, trakChanges).SingleOrDefaultAsync();
        }
        
        public void CreateCompany(Company company)
        {
            Create(company);
        }
        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trakChanges)
        {
            return await FindByCondition(c => ids.Contains(c.Id), trakChanges).ToListAsync();
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
