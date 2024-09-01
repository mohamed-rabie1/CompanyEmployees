using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trakChanges);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trakChanges); // delegate of method that take entity and return bool
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
