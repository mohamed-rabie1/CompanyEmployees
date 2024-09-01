using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        //Initializes a new ExpandoObject that does not have members.
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities,string fieldsString);
        ShapedEntity ShapeData(T entity,string fieldsString);
    }
}
