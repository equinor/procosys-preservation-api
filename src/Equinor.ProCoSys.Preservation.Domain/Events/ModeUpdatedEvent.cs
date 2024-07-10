﻿using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModeUpdatedEvent : IDomainEvent
{
    public ModeUpdatedEvent(Mode mode) => Mode = mode;
    public Mode Mode { get; }
}
