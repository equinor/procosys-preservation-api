using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameResponsible]
public class ResponsibleEvent : IResponsibleEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public string Plant { get; init; }

    [JsonIgnore] //ProjectName isnt needed for Responsible but is required for IIntegrationEvent
    public string ProjectName { get; init; } = null;
    
    public string Code { get; init; }
    public string Description { get; init; }
    public bool IsVoided { get; init; }
    
    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}
