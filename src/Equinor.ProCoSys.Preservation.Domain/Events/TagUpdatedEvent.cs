﻿using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUpdatedEvent : IDomainEvent
    {
        public TagUpdatedEvent(Tag tag) => Tag = tag;
        public Tag Tag { get; }
    }
}