using Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly RepositoryContext _repositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll(bool trakChanges)
        {
            //return !trakChanges ? _repositoryContext.Set<T>().AsNoTracking():_repositoryContext.Set<T>();
        
            if (! trakChanges)
            {
                return _repositoryContext.Set<T>().AsNoTracking();
            }
            else
            {
                return _repositoryContext.Set<T>();
            }
           
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trakChanges)
        {
            if (!trakChanges)
            {
                return _repositoryContext.Set<T>().Where(expression).AsNoTracking();
            }
            else
            {
                return _repositoryContext.Set<T>().Where(expression);
            }
            
        }
        public void Create(T entity) => _repositoryContext.Set<T>().Add(entity); // Set<T>() because generic
        public void Update(T entity) => _repositoryContext.Set<T>().Update(entity);
        public void Delete(T entity) => _repositoryContext.Set<T>().Remove(entity);
    }
}
