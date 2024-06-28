namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IActionEventV1 : IIntegrationEvent
{
    string TagNr { get; }
    string Title { get; }
    string Description { get; }
    DateOnly? DueDate { get; }
    bool Overdue { get; }
    DateOnly? Closed { get; }
}
