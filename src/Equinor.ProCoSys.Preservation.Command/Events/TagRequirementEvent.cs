﻿using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameTagRequirement]
public class TagRequirementEvent : ITagRequirementEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public string Plant { get; init; }
    public string ProjectName { get; init; }

    public Guid RequirementDefinitionGuid { get; init; }

    public Guid TagGuid { get; init; }
    public int IntervalWeeks { get; init; }
    public string Usage { get; init; }
    public DateTime? NextDueTimeUtc { get; init; }
    public bool IsVoided { get; init; }
    public bool IsInUse { get; init; }

    public bool ReadyToBePreserved { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init;  }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init;  }
}
