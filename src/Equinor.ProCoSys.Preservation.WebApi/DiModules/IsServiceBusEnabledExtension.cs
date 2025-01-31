using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class IsServiceBusEnabledExtension
{
    public static bool IsServiceBusEnabled(this WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("ServiceBus:Enable"))
        {
            return false;
        }

        if (!builder.Environment.IsDevelopment())
        {
            return true;
        }

        return builder.Configuration.GetValue<bool>("ServiceBus:EnableInDevelopment");
    }
}
