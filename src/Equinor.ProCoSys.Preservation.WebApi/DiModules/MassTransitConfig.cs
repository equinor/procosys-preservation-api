using System;
using System.Text.Json.Serialization;
using Azure.Core;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MassTransit;
using Microsoft.AspNetCore.Builder;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class MassTransitConfig
{
    public static void ConfigureMassTransit(this WebApplicationBuilder builder, TokenCredential credential) =>
        builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<PreservationContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            if (!builder.IsServiceBusEnabled())
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });

                return;
            }

            x.UsingAzureServiceBus((_, cfg) =>
            {
                var serviceBusNamespace = builder.GetConfig<string>("ServiceBus:Namespace");

                var serviceUri = new Uri($"sb://{serviceBusNamespace}.servicebus.windows.net/");
                cfg.Host(serviceUri, host =>
                {
                    host.TokenCredential = credential;
                });

                cfg.OverrideDefaultBusEndpointQueueName("preservationfamtransferqueue");
                cfg.UseRawJsonSerializer();
                cfg.ConfigureJsonSerializerOptions(opts =>
                {
                    opts.Converters.Add(new JsonStringEnumConverter());

                    // Set it to null to use the default .NET naming convention (PascalCase)
                    opts.PropertyNamingPolicy = null;
                    return opts;
                });

                cfg.AutoStart = true;
            });
        });
}
