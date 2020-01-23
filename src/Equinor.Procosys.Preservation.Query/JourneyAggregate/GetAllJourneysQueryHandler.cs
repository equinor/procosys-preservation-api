using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetAllJourneysQueryHandler : IRequestHandler<GetAllJourneysQuery, Result<IEnumerable<JourneyDto>>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;

        public GetAllJourneysQueryHandler(IJourneyRepository journeyRepository, IModeRepository modeRepository, IResponsibleRepository responsibleRepository)
        {
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
        }

        public async Task<Result<IEnumerable<JourneyDto>>> Handle(GetAllJourneysQuery request, CancellationToken cancellationToken)
        {
            var journeys = await _journeyRepository.GetAllAsync();

            var modeIds = journeys.SelectMany(j => j.Steps).Select(x => x.ModeId).Distinct();
            var responsibleIds = journeys.SelectMany(j => j.Steps).Select(x => x.ResponsibleId).Distinct();

            var modes = (await _modeRepository.GetByIdsAsync(modeIds)).Select(x => new ModeDto(x.Id, x.Title));
            var responsibles = (await _responsibleRepository.GetByIdsAsync(responsibleIds)).Select(x => new ResponsibleDto(x.Id, x.Name));

            var journeyDtos =
                journeys.Where(j => !j.IsVoided || request.IncludeVoided)
                    .Select(j => new JourneyDto(
                        j.Id,
                        j.Title,
                        j.IsVoided,
                        j.Steps.Where(s => !s.IsVoided || request.IncludeVoided)
                            .Select(s => new StepDto(
                                s.Id,
                                s.IsVoided,
                                modes.FirstOrDefault(m => m.Id == s.ModeId),
                                responsibles.FirstOrDefault(r => r.Id == s.ResponsibleId)))));
            
            return new SuccessResult<IEnumerable<JourneyDto>>(journeyDtos);
        }
    }
}
