using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        //array of PropertyInfo’s that we’re going to pull out of the input type
        public PropertyInfo[] Properties {  get; set; }
        public DataShaper()
        {
            Properties= typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> shapedEntities, string fieldsString)
        {
            //It parses the input string and returns just the properties
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchData(shapedEntities, requiredProperties);
        }

        public ShapedEntity ShapeData(T shapedEntity, string fieldsString)
        {
            //It parses the input string and returns just the properties
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(shapedEntity, requiredProperties);
        }
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();
            if (!string.IsNullOrEmpty(fieldsString))
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in fields)
                {
                    var property = Properties.FirstOrDefault(pi=>pi.Name.Equals(field.Trim(),StringComparison.CurrentCultureIgnoreCase));
                    if (property is null)
                    {
                        continue;
                    }
                    requiredProperties.Add(property);
                }
            }
            return requiredProperties;
        }
        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> shapedEntities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();
            foreach (var shapedEntity in shapedEntities) 
            { 
                var shapedObject = FetchDataForEntity(shapedEntity, requiredProperties); 
                shapedData.Add(shapedObject); 
            }
            return shapedData;
        }
        private ShapedEntity FetchDataForEntity(T ShapedEntity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();
            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(ShapedEntity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
                //shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }
            var objectProperty = ShapedEntity.GetType().GetProperty("Id"); 
            shapedObject.Id = (Guid)objectProperty.GetValue(ShapedEntity);
            
            return shapedObject;
        }
    }
}
