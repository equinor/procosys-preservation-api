using Microsoft.AspNetCore.Hosting;

namespace Equinor.Procosys.Preservation.WebApi
{
    public static class IWebHostEnvironmentExtension
    {
        public static bool IsE2ETestEnvironment(this IWebHostEnvironment environment)
            => environment.EnvironmentName == Startup.E2ETestEnvironment;
    }
}
