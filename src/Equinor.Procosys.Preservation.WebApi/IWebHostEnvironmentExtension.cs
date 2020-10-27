using Microsoft.AspNetCore.Hosting;

namespace Equinor.Procosys.Preservation.WebApi
{
    public static class IWebHostEnvironmentExtension
    {
        public static bool IsIntegrationTestEnvironment(this IWebHostEnvironment environment)
            => environment.EnvironmentName == Startup.IntegrationTestEnvironment;
    }
}
