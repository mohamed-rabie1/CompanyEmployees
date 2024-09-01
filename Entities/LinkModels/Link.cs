using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class Link
    {
        public string Href { get; set; } //represents a target URI.
        public string Rel { get; set; }//describes how the current context is related to the target resource.
        public string Method { get; set; } //http method
        public Link() // for xml serialization
        {
            
        }
        public Link(string href,string rel,string method)
        {
            Href=href;
            Rel=rel;
            Method=method;
        }
    }
}
