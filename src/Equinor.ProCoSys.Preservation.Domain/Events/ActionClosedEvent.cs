﻿using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionClosedEvent : DomainEvent
    {
        public ActionClosedEvent(
            string plant,
            Guid objectGuid) : base("Action closed")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
