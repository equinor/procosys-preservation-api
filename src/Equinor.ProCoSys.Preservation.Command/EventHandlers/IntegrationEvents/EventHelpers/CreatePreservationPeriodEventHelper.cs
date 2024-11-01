using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreatePreservationPeriodEventHelper : ICreateEventHelper<PreservationPeriod, PreservationPeriodsEvent>
{
    private readonly IReadOnlyContext _context;
    private readonly ICreateChildEventHelper<TagRequirement, PreservationPeriod, PreservationPeriodsEvent> _createPreservationPeriodEventHelper;

    public CreatePreservationPeriodEventHelper(
        IReadOnlyContext context,
        ICreateChildEventHelper<TagRequirement, PreservationPeriod, PreservationPeriodsEvent> createPreservationPeriodEventHelper)
    {
        _context = context;
        _createPreservationPeriodEventHelper = createPreservationPeriodEventHelper;
    }

    public async Task<PreservationPeriodsEvent> CreateEvent(PreservationPeriod entity)
    {
        var tagRequirement = _context.QuerySet<TagRequirement>().Single(rd => rd.Id == entity.TagRequirementId);
        return await _createPreservationPeriodEventHelper.CreateEvent(tagRequirement, entity);
    }
}
