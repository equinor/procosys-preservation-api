﻿using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagVoidedInSourceEvent : DomainEvent
    {
        public TagVoidedInSourceEvent(
            string plant,
            Guid objectGuid) : base("Tag voided in source")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
