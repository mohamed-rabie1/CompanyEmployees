using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    // custom exception
    public abstract class BadRequestException:Exception
    {
        protected BadRequestException(string message):base(message)
        {
            
        }
    }
}
