using System.ComponentModel;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public enum EventType
    {
        [Description("Added requirement")]
        AddRequirement,
        [Description("Deleted requirement")]
        DeleteRequirement,
        [Description("Voided requirement")]
        VoidRequirement,
        [Description("Unvoided requirement")]
        UnvoidRequirement,
        [Description("Preserved requirement")]
        PreserveRequirement,
        [Description("Voided tag")]
        VoidTag,
        [Description("Unvoided tag")]
        UnvoidTag,
        [Description("Created tag")]
        CreateTag,
        [Description("Deleted tag")]
        DeleteTag,
        [Description("Started preservation")]
        StartPreservation,
        [Description("Completed preservation")]
        CompletePreservation,
        [Description("Changed interval")]
        ChangeInterval,
        [Description("Manually transferred")]
        ManualTransfer,
        [Description("Automatically transferred")]
        AutomaticTransfer,
        [Description("Added action")]
        AddAction,
        [Description("Closed action")]
        CloseAction
    }
}
