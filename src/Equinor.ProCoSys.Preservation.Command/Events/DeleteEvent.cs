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
        Guid = guid;
        Plant = plant;
        ProjectName = projectName;
    }


    public Guid Guid { get; init; }
    public string Plant { get; init; }
    public string ProjectName { get; init; }
    public string Behavior { get; init; } = "delete";


    [EntityName("PreservationTagDelete")]
    public class TagDeleteEvent : DeleteEvent
    {
        public TagDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationTagRequirementDelete")]
    public class TagRequirementDeleteEvent : DeleteEvent
    {
        public TagRequirementDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationActionDelete")]
    public class ActionDeleteEvent : DeleteEvent
    {
        public ActionDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
    }

    [EntityName("PreservationRequirementFieldDelete")]
    public class RequirementFieldDeleteEvent : DeleteEvent
    {
        public RequirementFieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationRequirementTypeDelete")]
    public class RequirementTypeDeleteEvent : DeleteEvent
    {
        public RequirementTypeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationRequirementDefinitionDelete")]
    public class RequirementDefinitionDeleteEvent : DeleteEvent
    {
        public RequirementDefinitionDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }

    [EntityName("PreservationPeriodDelete")]
    public class PreservationPeriodDeleteEvent : DeleteEvent
    {
        public PreservationPeriodDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
    }
}
