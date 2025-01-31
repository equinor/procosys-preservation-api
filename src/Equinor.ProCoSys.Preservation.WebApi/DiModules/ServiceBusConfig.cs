using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class ServiceBusConfig
{
    public static void ConfigureServiceBus(this WebApplicationBuilder builder)
    {
        if (!builder.IsServiceBusEnabled())
        {
            return;
        }
        
        // Env variable used in radix. Configuration is added for easier use locally
        // Url will be validated during startup of service bus integration and give a
        // Uri exception if invalid.
        builder.Services.AddPcsServiceBusIntegration(options => options
            .UseBusConnection(builder.GetConfig<string>("ConnectionStrings:ServiceBus"))
            .WithLeaderElector(builder.GetLeaderElectorUri())
            .WithRenewLeaseInterval(builder.GetConfig<int>("ServiceBus:LeaderElectorRenewLeaseInterval"))
            .WithSubscription(PcsTopicConstants.Tag, "preservation_tag")
            .WithSubscription(PcsTopicConstants.TagFunction, "preservation_tagfunction")
            .WithSubscription(PcsTopicConstants.Project, "preservation_project")
            .WithSubscription(PcsTopicConstants.CommPkg, "preservation_commpkg")
            .WithSubscription(PcsTopicConstants.McPkg, "preservation_mcpkg")
            .WithSubscription(PcsTopicConstants.Responsible, "preservation_responsible")
            .WithSubscription(PcsTopicConstants.Certificate, "preservation_certificate")
            //THIS METHOD SHOULD BE FALSE IN NORMAL OPERATION.
            //ONLY SET TO TRUE WHEN A LARGE NUMBER OF MESSAGES HAVE FAILED AND ARE COPIED TO DEAD LETTER.
            //WHEN SET TO TRUE, MESSAGES ARE READ FROM DEAD LETTER QUEUE INSTEAD OF NORMAL QUEUE
            .WithReadFromDeadLetterQueue(builder.Configuration.GetValue("ServiceBus:ReadFromDeadLetterQueue", defaultValue: false)));
    }

    private static string GetLeaderElectorUri(this WebApplicationBuilder builder)
    {
        var uriString = Environment.GetEnvironmentVariable("LEADERELECTOR_SERVICE");
        if (uriString != null)
        {
            return uriString;
        }

        return builder.Configuration["ServiceBus:LeaderElectorUrl"] + ":3003";
    }
}
