using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next) => _next = next;

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
                var response = new ValidationErrorResponse(ve.Errors.Count(), ve.Errors.Select(x => new ValidationError(x.PropertyName, x.ErrorMessage, x.AttemptedValue)));
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (ProcosysException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync($"Something went wrong!");
            }
        }

        private class ValidationErrorResponse
        {
            public ValidationErrorResponse(int errorCount, IEnumerable<ValidationError> errors)
            {
                ErrorCount = errorCount;
                Errors = errors;
            }

            public int ErrorCount { get; }
            public IEnumerable<ValidationError> Errors { get; }
        }

        private class ValidationError
        {
            public ValidationError(string propertyName, string errorMessage, object attemptedValue)
            {
                PropertyName = propertyName;
                ErrorMessage = errorMessage;
                AttemptedValue = attemptedValue;
            }

            public string PropertyName { get; set; }
            public string ErrorMessage { get; set; }
            public object AttemptedValue { get; set; }
        }
    }
}
