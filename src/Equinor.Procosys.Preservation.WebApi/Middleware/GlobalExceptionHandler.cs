using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            catch (FluentValidation.ValidationException ve)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/text";
                int errorCount = ve.Errors.Count();
                for (int i = 0; i < errorCount; i++)
                {
                    await context.Response.WriteAsync(ve.Errors.ElementAt(i).ErrorMessage + ((i < errorCount - 1) ? Environment.NewLine : null));
                }
            }
            catch (ProcosysEntityNotFoundException penfe)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync(penfe.Message);
                //await context.Response.WriteAsync($"The entity could not be found");
            }
            catch (ProcosysException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync($"Something went wrong!");
            }
        }
    }
}
