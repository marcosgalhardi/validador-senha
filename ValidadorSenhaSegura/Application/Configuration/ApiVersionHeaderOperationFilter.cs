using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ValidadorSenhaSegura.Application.Configuration
{
    public class ApiVersionHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            // Add the api-version header parameter if it doesn't already exist
            if (!operation.Parameters.Any(p => p.Name == "api-version"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "api-version",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}
