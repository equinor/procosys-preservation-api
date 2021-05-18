using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    /// Used when service bus is disabled
    class DisabledServiceBusSender : IPcsBusSender
    {
        public Task SendAsync(string topic, string jsonMessage) => Task.CompletedTask;

        public Task CloseAllAsync() => Task.CompletedTask;
    }
}
