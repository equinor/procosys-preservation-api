using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
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
                _logger.LogWarning("Unauthorized");
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync("Unauthorized!");
            }
            catch (FluentValidation.ValidationException ve)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";
                var errors = new Dictionary<string, string[]>();
                foreach (var error in ve.Errors)
                {
                    errors.Add(error.PropertyName, new[] {error.ErrorMessage});
                }
                var problems = new ValidationProblemDetails(errors)
                {
                    Status = context.Response.StatusCode,
                    Title = $"One or more business validation errors occurred. ({errors.Count})"
                };
                var json = JsonSerializer.Serialize(problems);
                _logger.LogInformation(json);

                await context.Response.WriteAsync(json);
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
