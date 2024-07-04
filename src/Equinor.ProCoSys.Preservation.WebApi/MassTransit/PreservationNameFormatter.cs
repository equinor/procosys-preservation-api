using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.MessageContracts;
using MassTransit;

namespace Equinor.ProCoSys.Preservation.WebApi.MassTransit;

public class PreservationNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>() =>
        typeof(T).Name switch
        {
            nameof(BusEventMessage) => "preservation",
            nameof(IIntegrationEvent) => nameof(IIntegrationEvent),
            nameof(ITagEventV1) => "PreservationTag",
            nameof(ITagRequirementEventV1) => "PreservationTagRequirement",
            nameof(IRequirementTypeEventV1) => "PreservationRequirementType",
            nameof(IActionEventV1) => "PreservationTagAction",
            nameof(IFieldEventV1) => "PreservationField",
            nameof(IPreservationPeriodEventV1) => "PreservationPeriod",
            nameof(IRequirementDefinitionEventV1) => "PreservationRequirementDefinition",

            _ => throw new ArgumentException($"Preservation error: {typeof(T).Name} is not configured with a topic name mapping.")
        };
}
