namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public enum EventType
    {
        AddRequirement,
        DeleteRequirement,
        VoidRequirement,
        UnvoidRequirement,
        PreserveRequirement,
        VoidTag,
        UnvoidTag,
        CreateTag,
        DeleteTag,
        StartPreservation,
        CompletePreservation,
        ChangeInterval,
        ManualTransfer,
        AutomaticTransfer,
        AddAction,
        CloseAction
    }
}
