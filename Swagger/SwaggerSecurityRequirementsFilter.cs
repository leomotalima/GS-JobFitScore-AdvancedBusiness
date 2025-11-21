using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JobFitScoreAPI.Swagger
{
    public class SwaggerSecurityRequirementsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .Any(attr => attr is AllowAnonymousAttribute);

            var controllerHasAllowAnonymous = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .Any(attr => attr is AllowAnonymousAttribute) ?? false;

            if (!hasAllowAnonymous && !controllerHasAllowAnonymous)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
            }
        }
    }
}