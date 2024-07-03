namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IRequirementTypeEventV1 : IIntegrationEvent
{
    string Code { get; }
    string Title { get; }
    bool IsVoided { get; }
    int SortKey { get; }
    DateTime CreatedAtUtc { get; }
    int CreatedById { get; }
    DateTime? ModifiedAtUtc { get; }
    int? ModifiedById { get; }
}
