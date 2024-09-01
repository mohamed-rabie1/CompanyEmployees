﻿using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DTOs;
using System.Text;

namespace CompanyEmployees
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv")); 
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        protected override bool CanWriteType(Type? type) //indicates whether or not the CompanyDto type can be written by this serializer
        {
            if (typeof(CompanyDTO).IsAssignableFrom(type)
                || typeof(IEnumerable<CompanyDTO>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }

            return false;
        }
        // constructs the response.
        public async override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<CompanyDTO>)
            {
                foreach (var company in (IEnumerable<CompanyDTO>)context.Object)
                {
                    FormatCsv(buffer, company);
                }
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                FormatCsv(buffer, (CompanyDTO)context.Object);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
            }

            await response.WriteAsync(buffer.ToString());
        }
        // formats a response the way we want it.
        private static void FormatCsv(StringBuilder buffer, CompanyDTO company)
        {
            buffer.AppendLine($"{company.Id},\"{company.Name}\",\"{company.FullAddress}\"");
        }
    }
}
