namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IRequirementDefinitionEventV1 : IIntegrationEvent
{
    string Title { get; }
    bool IsVoided { get; }
    int DefaultIntervalWeeks { get; }
    string Usage { get; }
    int SortKey { get; }
    bool NeedsUserInput { get; }
    DateTime CreatedAtUtc { get; }
    int CreatedById { get; }
    DateTime? ModifiedAtUtc { get; }
    int? ModifiedById { get; }
}
