namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IDeleteEventV1 : IIntegrationEvent
{
    string Behavior { get; }
}
