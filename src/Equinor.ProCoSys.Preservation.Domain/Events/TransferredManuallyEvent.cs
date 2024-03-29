﻿using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TransferredManuallyEvent : IDomainEvent
    {
        public TransferredManuallyEvent(
            string plant,
            Guid sourceGuid,
            string fromStep,
            string toStep)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            FromStep = fromStep;
            ToStep = toStep;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public string FromStep { get; }
        public string ToStep { get; }
    }
}
