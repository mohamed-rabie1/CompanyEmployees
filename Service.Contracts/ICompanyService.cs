using Entities.Models;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync(bool trakChanges);
        Task<CompanyDTO> GetCompanyAsync(Guid id,bool trakChanges);
        Task<CompanyDTO> CreateCompanyAsync(CompanyForCreationDTO company);
        Task<IEnumerable<CompanyDTO>> GetByIdsAsync(IEnumerable<Guid> ids,bool trackChanges);
        Task<(IEnumerable<CompanyDTO> companies,string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDTO> companyCollection);
        Task DeleteCompanyAsync(Guid companyId,bool trakChanges);
        Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);
    }
}
