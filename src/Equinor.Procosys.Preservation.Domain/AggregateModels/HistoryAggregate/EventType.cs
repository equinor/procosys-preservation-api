namespace Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate
{
    public enum EventType
    {
        AddRequirement,
        DeleteRequirement,
        VoidRequirement,
        UnvoidRequirement,
        VoidTag,
        UnvoidTag,
        StartPreservation,
        CompletePreservation,
        ChangeInterval,
        ManualTransfer,
        AutomaticTransfer
    }
}
