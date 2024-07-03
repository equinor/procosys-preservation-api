using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedInSourceEvent : IDomainEvent
    {
        public TagUnvoidedInSourceEvent(string plant, Tag tag)
        {
            Plant = plant;
            Tag = tag;
        }
        public string Plant { get; }
        public Guid SourceGuid => Tag.Guid;
        public Tag Tag { get; }
    }
}
