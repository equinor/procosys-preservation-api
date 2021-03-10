using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class TagRequirementAddedEvent : INotification
    {
        public TagRequirementAddedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            RequirementDefinitionId = requirementDefinitionId;
        }

        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int RequirementDefinitionId { get; }
    }
}
