namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface ITagRequirementEventV1 : IIntegrationEvent
{
    int IntervalWeeks { get; }
    string Usage { get; }
    DateTime? NextDueTimeUtc { get; }
    bool IsVoided { get; }
    bool IsInUse { get; }
    Guid RequirementDefinitionGuid { get; }
    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
    bool ReadyToBePreserved { get; }
}
