using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class RequirementDeletedEvent : INotification
    {
        public RequirementDeletedEvent(
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
