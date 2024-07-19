using System;
using System.Text.Json.Serialization;
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


    [EntityName("PreservationTag")]
    public class TagDeleteEvent : DeleteEvent
    {
        public TagDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationTagRequirement")]
    public class TagRequirementDeleteEvent : DeleteEvent
    {
        public TagRequirementDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationAction")]
    public class ActionDeleteEvent : DeleteEvent
    {
        public ActionDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationRequirementField")]
    public class RequirementFieldDeleteEvent : DeleteEvent
    {
        public RequirementFieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationRequirementType")]
    public class RequirementTypeDeleteEvent : DeleteEvent
    {
        public RequirementTypeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationRequirementDefinition")]
    public class RequirementDefinitionDeleteEvent : DeleteEvent
    {
        public RequirementDefinitionDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationPeriod")]
    public class PreservationPeriodDeleteEvent : DeleteEvent
    {
        public PreservationPeriodDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationModes")]
    public class ModeDeleteEvent : DeleteEvent
    {
        public ModeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationJourneys")]
    public class JourneyDeleteEvent : DeleteEvent
    {
        public JourneyDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationSteps")]
    public class StepDeleteEvent : DeleteEvent
    {
        public StepDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationResponsible")]
    public class ResponsibleDeleteEvent : DeleteEvent
    {
        public ResponsibleDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }
}
