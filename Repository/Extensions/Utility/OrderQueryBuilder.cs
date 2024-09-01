using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions.Utility
{
    public class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            //splitting our query string to get the individual fields
            var orderParams = orderByQueryString.Trim().Split(',');
            //using  reflection to get properities of entity
            var properityInfo = typeof(T).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = properityInfo.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, comparisonType: StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' '); //removing excess commas
            return orderQuery;
        }

    }
}
