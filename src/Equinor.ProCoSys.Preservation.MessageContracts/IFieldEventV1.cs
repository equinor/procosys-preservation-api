namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IFieldEventV1 : IIntegrationEvent
{
    Guid RequirementDefinitionGuid { get;}

    string Label { get; }
    string Unit { get; }
    int SortKey { get; }
    string FieldType { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
}
