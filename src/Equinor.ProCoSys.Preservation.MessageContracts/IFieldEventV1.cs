namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IFieldEventV1 : IIntegrationEvent
{
    Guid RequirementDefinitionGuid { get;}
    int FieldId { get; }

    string Label { get; }
    string Unit { get; }
    int SortKey { get; }
    string FieldType { get; }

    DateTime CreatedAtUtc { get; }
    int CreatedById { get; }
    DateTime? ModifiedAtUtc { get; }
    int? ModifiedById { get; }
}
