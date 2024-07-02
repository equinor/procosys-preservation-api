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
            nameof(IDeleteEventV1) => "preservationdelete",

            nameof(IIntegrationEvent) => nameof(IIntegrationEvent),
            nameof(ITagEventV1) => "preservationtag",
            nameof(ITagRequirementEventV1) => "preservationtagrequirement",
            nameof(IActionEventV1) => "preservationtagaction",
            nameof(IFieldEventV1) => "preservationfield",
            nameof(IPreservationPeriodEventV1) => "preservationperiod",
            nameof(IPreservationRecordEventV1) => "preservationrecord",
            nameof(IRequirementDefinitionEventV1) => "preservationrequirementdefinition",

            _ => throw new ArgumentException($"Preservation error: {typeof(T).Name} is not configured with a topic name mapping.")
        };}
