﻿using System.ComponentModel;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public enum EventType
    {
        [Description("Requirement added")]
        RequirementAdded,
        [Description("Requirement deleted")]
        RequirementDeleted,
        [Description("Requirement voided")]
        RequirementVoided,
        [Description("Requirement unvoided")]
        RequirementUnvoided,
        [Description("Requirement preserved")]
        RequirementPreserved,
        [Description("Tag voided")]
        TagVoided,
        [Description("Tag unvoided")]
        TagUnvoided,
        [Description("Tag created")]
        TagCreated,
        [Description("Tag deleted")]
        TagDeleted,
        [Description("Preservation started")]
        PreservationStarted,
        [Description("Preservation completed")]
        PreservationCompleted,
        [Description("Interval changed")]
        IntervalChanged,
        [Description("Transferred manually")]
        TransferredManually,
        [Description("Transferred automatically")]
        TransferredAutomatically,
        [Description("Action added")]
        ActionAdded,
        [Description("Action closed")]
        ActionClosed
    }
}