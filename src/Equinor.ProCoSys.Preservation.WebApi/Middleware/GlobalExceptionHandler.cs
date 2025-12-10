using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                await context.WriteForbidden(_logger);
            }
            catch (FluentValidation.ValidationException ve)
            {
                var errors = new Dictionary<string, string[]>();
                foreach (var error in ve.Errors)
                {
                    if (!errors.ContainsKey(error.PropertyName))
                    {
                        errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                    }
                    else
                    {
                        var errorsForProperty = errors[error.PropertyName].ToList();
                        errorsForProperty.Add(error.ErrorMessage);
                        errors[error.PropertyName] = errorsForProperty.ToArray();
                    }
                }

                await context.WriteBadRequestAsync(errors, _logger);
            }
            catch (InValidProjectException ipe)
            {
                var errors = new Dictionary<string, string[]> { { "ProjectName", new[] { ipe.Message } } };
                await context.WriteBadRequestAsync(errors, _logger);
            }
            catch (ConcurrencyException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/text";
                const string message = "Data store operation failed. Data may have been modified or deleted since entities were loaded.";
                _logger.LogDebug(message);
                await context.Response.WriteAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync("Something went wrong!");
            }
        }
    }
}
