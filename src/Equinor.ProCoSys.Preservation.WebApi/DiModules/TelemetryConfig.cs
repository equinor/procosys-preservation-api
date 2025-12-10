using Azure.Core;
using Equinor.ProCoSys.Common.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class TelemetryConfig
{
    public static void ConfigureTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        TokenCredential credential)
    {
        services.AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>();

        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = configuration.GetValue<string>("ApplicationInsights:ConnectionString");
        });

        services.Configure<TelemetryConfiguration>(config =>
        {
            config.SetAzureTokenCredential(credential);
        });
    }
}
