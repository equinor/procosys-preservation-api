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
        StartPreservation,
        CompletePreservation,
        ChangeInterval,
        ManualTransfer,
        AutomaticTransfer
    }
}
