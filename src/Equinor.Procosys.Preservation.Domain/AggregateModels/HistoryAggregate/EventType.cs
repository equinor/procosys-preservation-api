using System.ComponentModel;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public enum EventType
    {
        [Description("Requirement added")]
        TagRequirementAdded,
        [Description("Requirement deleted")]
        TagRequirementDeleted,
        [Description("Requirement voided")]
        TagRequirementVoided,
        [Description("Requirement unvoided")]
        TagRequirementUnvoided,
        [Description("Requirement preserved")]
        TagRequirementPreserved,
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
        [Description("Journey changed")]
        JourneyChanged,
        [Description("Step changed")]
        StepChanged,
        [Description("Transferred manually")]
        TransferredManually,
        [Description("Transferred automatically")]
        TransferredAutomatically,
        [Description("Action added")]
        ActionAdded,
        [Description("Action closed")]
        ActionClosed,
        [Description("Rescheduled")]
        Rescheduled
    }
}
