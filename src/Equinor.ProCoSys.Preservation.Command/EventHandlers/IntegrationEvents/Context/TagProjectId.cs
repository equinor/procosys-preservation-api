using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;

public class TagProjectId : ITagProjectId
{
    private readonly IContextSemaphore _semaphore;
    private readonly IReadOnlyContext _context;

    public TagProjectId(IContextSemaphore semaphore, IReadOnlyContext context)
    {
        _semaphore = semaphore;
        _context = context;
    }

    public async Task<int> Retrieve(Tag tag)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            return await _context.QuerySet<Tag>()
                .AsNoTracking()
                .Where(tg => tg.Guid == tag.Guid)
                .Select(tg => EF.Property<int>(tg, "ProjectId"))
                .SingleAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
