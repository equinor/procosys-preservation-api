using System.ComponentModel;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate
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
        Rescheduled,
        [Description("Undo \"Preservation started\"")]
        UndoPreservationStarted,
        [Description("Tag voided in source system")]
        TagVoidedInSource,
        [Description("Tag unvoided in source system")]
        TagUnvoidedInSource,
        [Description("Tag deleted in source system")]
        TagDeletedInSource
    }
}
