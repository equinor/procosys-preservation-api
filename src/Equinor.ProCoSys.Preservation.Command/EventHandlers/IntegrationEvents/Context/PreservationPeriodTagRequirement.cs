using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;

public class PreservationPeriodTagRequirement : IPreservationPeriodTagRequirement
{
    private readonly IContextSemaphore _semaphore;
    private readonly IReadOnlyContext _context;

    public PreservationPeriodTagRequirement(IContextSemaphore semaphore, IReadOnlyContext context)
    {
        _semaphore = semaphore;
        _context = context;
    }
    
    public async Task<TagRequirement> Retrieve(PreservationPeriod preservationPeriod)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            return _context.QuerySet<TagRequirement>().Single(rd => rd.Id == preservationPeriod.TagRequirementId);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
