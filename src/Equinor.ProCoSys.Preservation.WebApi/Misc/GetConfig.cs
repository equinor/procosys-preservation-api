using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc;

public static class GetConfigExtension
{
    public static T GetConfig<T>(this WebApplicationBuilder builder, string configKey) => builder.Configuration.GetValue<T>(configKey);

    public static T GetConfig<T>(this IConfiguration configuration, string configKey)
    {
        var value = configuration.GetValue<T>(configKey);
        if(value is null)
        {
            throw new Exception($"Missing configuration for {configKey}");
        }

        return value;
    }
}
