using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagDeletedEvent : IDomainEvent
    {
        public TagDeletedEvent(Tag tag) => Tag = tag;
        public Tag Tag { get; }
    }
}
