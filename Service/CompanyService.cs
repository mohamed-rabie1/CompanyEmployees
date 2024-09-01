using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CompanyService: ICompanyService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        public CompanyService(IRepositoryManager repositoryManager,ILoggerManager loggerManager, IMapper mapper)
        {
            _repositoryManager= repositoryManager;
            _loggerManager= loggerManager;
            _mapper= mapper;

        }

        public async Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync(bool trakChanges)
        {
            
                var companies =await _repositoryManager.Company.GetAllCompaniesAsync(trakChanges);
                //var companiesDTO = companies.Select(c => new CompanyDTO(c.Id, c.Name?? "", string.Join(' ',c.Address,c.Country))).ToList(); // when name is null equal it to ""
                var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
                return companiesDTO;
            
            
        }
        public async Task<CompanyDTO> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
            var companyDTO= _mapper.Map<CompanyDTO>(company);
            return companyDTO;
        }
        public async Task<CompanyDTO> CreateCompanyAsync(CompanyForCreationDTO company)
        {
            var companyEntity = _mapper.Map<Company>(company);
             _repositoryManager.Company.CreateCompany(companyEntity);
            await _repositoryManager.SaveAsync();
            var companyDTO = _mapper.Map<CompanyDTO>(companyEntity);
            return companyDTO;
        }
        public async Task<IEnumerable<CompanyDTO>> GetByIdsAsync(IEnumerable<Guid> ids, bool trakChanges)
        {
            if (ids is null)
            {
                throw new IdParametersBadRequestException();
            }
            var companies = await _repositoryManager.Company.GetByIdsAsync(ids, trakChanges);
            if (ids.Count() != companies.Count())
            {
                throw new CollectionByIdsBadRequestException();
            }
            var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
            return companiesDTO;
        }
        public async Task<(IEnumerable<CompanyDTO> companies,string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDTO> companyCollection)
        {
            if(companyCollection is null)
            {
                throw new CompanyCollectionBadRequest();
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                 _repositoryManager.Company.CreateCompany(company);               
            }
            await _repositoryManager.SaveAsync();
            var companyDTOs = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(',', companyEntities.Select(c => c.Id));
            return (companies: companyDTOs, ids: ids);
           
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _repositoryManager.Company.DeleteCompany(company);
            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            /*var companyEntity= await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges);
            if (companyEntity is null)
            {
                throw new CompanyNotFoundException(companyId);
            }*/
            var companyEntity = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _mapper.Map(companyForUpdate,companyEntity);
            await _repositoryManager.SaveAsync();
        }
        private async Task<Company> GetCompanyAndCheckIfItExists(Guid companyId,bool trackChanges)
        {
            var company= await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            return company;
        }
    }
}
