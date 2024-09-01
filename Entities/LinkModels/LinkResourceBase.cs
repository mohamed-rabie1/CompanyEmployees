using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class LinkResourceBase //contain all of our links
    {
        public LinkResourceBase()
        {
            
        }
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
