using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    //transfer required parameters from our controller to the service layer and avoid
    //the installation of an additional NuGet package inside the Service and Service.Contracts projects.
    public record LinkModels(EmployeeParameters EmployeeParameters,HttpContext HttpContext);
    
}
