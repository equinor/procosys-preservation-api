using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc;

public static class GetConfigExtension
{
    public static T GetConfig<T>(this WebApplicationBuilder builder, string configKey) => builder.Configuration.GetConfig<T>(configKey);

    public static T GetConfig<T>(this IConfiguration configuration, string configKey)
    {
        var value = configuration.GetValue<T>(configKey);
        if(value is null)
        {
            throw new ArgumentException($"Missing configuration for {configKey}");
        }

        return value;
    }
}
