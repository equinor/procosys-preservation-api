﻿using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementDeletedEvent : IPlantEntityEvent<TagRequirement>, IDomainEvent
    {
        public TagRequirementDeletedEvent(string plant, Guid sourceGuid, TagRequirement tagRequirement)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Entity = tagRequirement;
        }

        public string Plant { get; }
        public Guid SourceGuid { get; }
        public TagRequirement Entity { get; }
    }
}
