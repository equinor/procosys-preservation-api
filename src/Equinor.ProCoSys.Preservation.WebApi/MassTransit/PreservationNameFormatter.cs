using System;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.MessageContracts;
using MassTransit;

namespace Equinor.ProCoSys.Preservation.WebApi.MassTransit;

public class PreservationNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>() =>
        typeof(T).Name switch
        {
            nameof(IIntegrationEvent) => nameof(IIntegrationEvent),

            _ => throw new ArgumentException($"Preservation error: {typeof(T).Name} is not configured with a topic name mapping.")
        };}
