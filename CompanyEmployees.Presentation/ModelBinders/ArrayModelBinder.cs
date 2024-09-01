using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ModelBinders
{
    internal class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //check if our parameter is Enumerable
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            // extract the enumerable value
            var providedValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
            //check whether it is null or empty.
            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result=ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            //we store the type the IEnumerable consists of(GUID Type)
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            //we create a converter to a GUID type.
            var converter =TypeDescriptor.GetConverter(genericType);
            //create an array of type object that consist of all the GUID values we sent to the API
            var objectArray = providedValue.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>converter.ConvertFromString((x.Trim()))).ToArray();
            //create an array of GUID types
            var guidArray =Array.CreateInstance(genericType, objectArray.Length);
            //copy all the values from the objectArray to the guidArray
            objectArray.CopyTo(guidArray, 0);

            bindingContext.Model=guidArray;
            bindingContext.Result=ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
