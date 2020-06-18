﻿using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class CurrentBearerTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentBearerTokenMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IHttpContextAccessor httpContextAccessor, IBearerTokenSetter bearerTokenSetter)
        {
            var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var token = authorizationHeader.ToString().Split(' ')[1];

            bearerTokenSetter.SetBearerToken(token);

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}