using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;
using MassTransit;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class DeleteEvent : IDeleteEventV1
{
    private DeleteEvent(Guid guid, string plant, string projectName)
    {
        ProCoSysGuid = guid;
        Plant = plant;
        ProjectName = projectName;
    }

    public Guid ProCoSysGuid { get; init; }
    public string Plant { get; init; }
    public string ProjectName { get; init; }
    public string Behavior { get; init; } = "delete";


    [TagEntityName]
    public class TagDeleteEvent : DeleteEvent
    {
        public TagDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [TagRequirementEntityName]
    public class TagRequirementDeleteEvent : DeleteEvent
    {
        public TagRequirementDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [ActionEntityName]
    public class ActionDeleteEvent : DeleteEvent
    {
        public ActionDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [RequirementFieldEntityName]
    public class RequirementFieldDeleteEvent : DeleteEvent
    {
        public RequirementFieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [RequirementTypeEntityName]
    public class RequirementTypeDeleteEvent : DeleteEvent
    {
        public RequirementTypeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [RequirementDefinitionEntityName]
    public class RequirementDefinitionDeleteEvent : DeleteEvent
    {
        public RequirementDefinitionDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [PreservationPeriodEntityName]
    public class PreservationPeriodDeleteEvent : DeleteEvent
    {
        public PreservationPeriodDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [ModeEntityName]
    public class ModeDeleteEvent : DeleteEvent
    {
        public ModeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [JourneyEntityName]
    public class JourneyDeleteEvent : DeleteEvent
    {
        public JourneyDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [StepEntityName]
    public class StepDeleteEvent : DeleteEvent
    {
        public StepDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [ResponsibleEntityName]
    public class ResponsibleDeleteEvent : DeleteEvent
    {
        public ResponsibleDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }
}
