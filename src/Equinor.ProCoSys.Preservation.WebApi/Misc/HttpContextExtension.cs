using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public static class HttpContextExtension
    {
        public static async Task WriteBadRequestAsync(
            this HttpContext context,
            Dictionary<string, string[]> errors,
            ILogger logger)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/problem+json";
            var problems = new ValidationProblemDetails(errors)
            {
                Status = context.Response.StatusCode,
                Title = "One or more business validation errors occurred"
            };
            var json = JsonSerializer.Serialize(problems);
            logger.LogInformation(json);

            await context.Response.WriteAsync(json);
        }

        public static async Task WriteForbidden(this HttpContext context, ILogger logger)
        {
            logger.LogWarning("Unauthorized");
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/text";
            await context.Response.WriteAsync("Unauthorized!");
        }
    }
}
