using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;

public interface IContextSemaphore
{
    Task WaitAsync();
    void Release();
}
