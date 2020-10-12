using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public static class HttpContextExtension
    {
        public static async Task WriteBadRequestAsync(
            this HttpContext context,
            Dictionary<string, string[]> errors,
            ILogger _logger)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/problem+json";
            var problems = new ValidationProblemDetails(errors)
            {
                Status = context.Response.StatusCode,
                Title = "One or more business validation errors occurred"
            };
            var json = JsonSerializer.Serialize(problems);
            _logger.LogInformation(json);

            await context.Response.WriteAsync(json);
        }
    }
}
