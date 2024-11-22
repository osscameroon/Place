using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Swagger;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiDescription? apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (ApiResponseType responseType in context.ApiDescription.SupportedResponseTypes)
        {
            string responseKey = responseType.IsDefaultResponse
                ? "default"
                : responseType.StatusCode.ToString();
            OpenApiResponse? response = operation.Responses[responseKey];

            foreach (string? contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (OpenApiParameter? parameter in operation.Parameters)
        {
            ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p =>
                p.Name == parameter.Name
            );

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
