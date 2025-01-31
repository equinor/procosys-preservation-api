using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class IsServiceBusEnabledExtension
{
    public static bool IsServiceBusEnabled(this WebApplicationBuilder builder) => IsServiceBusEnabled(builder.Configuration, builder.Environment);

    private static bool IsServiceBusEnabled(IConfiguration config, IHostEnvironment environment)
    {
        if (!config.GetValue<bool>("ServiceBus:Enable"))
        {
            return false;
        }

        if (!environment.IsDevelopment())
        {
            return true;
        }

        return config.GetValue<bool>("ServiceBus:EnableInDevelopment");
    }
}
