using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ProductionTrackerAPI
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileParameter = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(IFormFile) || 
                         p.ParameterType == typeof(IFormFile[]) ||
                         p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile)));

            if (!hasFileParameter) return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = context.MethodInfo.GetParameters()
                                .Where(p => p.GetCustomAttribute<FromFormAttribute>() != null)
                                .SelectMany(p => GetFormProperties(p.ParameterType))
                                .ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value
                                )
                        }
                    }
                }
            };
        }

        private static IEnumerable<KeyValuePair<string, OpenApiSchema>> GetFormProperties(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType == typeof(IFormFile))
                {
                    yield return new KeyValuePair<string, OpenApiSchema>(
                        property.Name,
                        new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        });
                }
                else
                {
                    yield return new KeyValuePair<string, OpenApiSchema>(
                        property.Name,
                        new OpenApiSchema
                        {
                            Type = GetSchemaType(property.PropertyType)
                        });
                }
            }
        }

        private static string GetSchemaType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            
            if (underlyingType == typeof(int) || underlyingType == typeof(long))
                return "integer";
            if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float))
                return "number";
            if (underlyingType == typeof(bool))
                return "boolean";
            if (underlyingType == typeof(DateTime))
                return "string";
            
            return "string";
        }
    }
}