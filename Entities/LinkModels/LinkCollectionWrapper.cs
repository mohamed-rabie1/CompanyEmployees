using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class LinkCollectionWrapper<T>: LinkResourceBase //wrapper for our links
    {
        public List<T> Values { get; set; }=new List<T>();
        public LinkCollectionWrapper()
        {
            
        }
        public LinkCollectionWrapper(List<T> values)
        {
            Values=values;
        }
    }
}
