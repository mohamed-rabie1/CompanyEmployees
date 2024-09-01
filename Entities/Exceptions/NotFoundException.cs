using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    //base class for all the individual not found exception classes
    public abstract class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
