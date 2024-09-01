using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        // task:know where the operation is compelete or not
        //Task<Entity>: async method return entity
        //Task: async method doesn't return value
        //void: async method for event handler
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trakChanges);
        Task<Company> GetCompanyAsync(Guid id, bool trakChanges);
        // they are left synch method because we are not making any changes in the database.
        //All we're doing is changing the state of the entity to Added or  Deleted.
        void CreateCompany(Company company);
        Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids,bool trakChanges);
        void DeleteCompany(Company company);
    }
}
