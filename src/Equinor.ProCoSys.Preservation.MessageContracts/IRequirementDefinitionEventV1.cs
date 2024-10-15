namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IRequirementDefinitionEventV1 : IIntegrationEvent
{
    Guid RequirementTypeGuid { get; }
    string Title { get; }
    bool IsVoided { get; }
    int DefaultIntervalWeeks { get; }
    string Usage { get; }
    int SortKey { get; }
    bool NeedsUserInput { get; }
    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
}
