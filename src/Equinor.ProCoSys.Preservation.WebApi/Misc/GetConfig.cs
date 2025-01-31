using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc;

public static class GetConfigExtension
{
    public static T GetConfig<T>(this WebApplicationBuilder builder, string configKey)
    {
        var value = builder.Configuration.GetValue<T>(configKey);
        if(value is null)
        {
            throw new Exception($"Missing configuration for {configKey}");
        }

        return value;
    }
}
