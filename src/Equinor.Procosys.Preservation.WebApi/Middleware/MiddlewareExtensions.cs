using Microsoft.AspNetCore.Builder;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void AddGlobalExtensionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
