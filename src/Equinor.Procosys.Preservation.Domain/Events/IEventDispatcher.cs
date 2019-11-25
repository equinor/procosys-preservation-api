using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEnumerable<Entity> entities);
    }
}
