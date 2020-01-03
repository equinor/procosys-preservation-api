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
    public class GetJourneyByIdQueryHandler : IRequestHandler<GetJourneyByIdQuery, Result<JourneyDto>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;

        public GetJourneyByIdQueryHandler(IJourneyRepository journeyRepository, IModeRepository modeRepository, IResponsibleRepository responsibleRepository)
        {
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
        }

        public async Task<Result<JourneyDto>> Handle(GetJourneyByIdQuery request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.Id);
            var modeIds = journey.Steps.Select(x => x.ModeId);
            var responsibleIds = journey.Steps.Select(x => x.ResponsibleId);

            var modes = (await _modeRepository.GetByIdsAsync(modeIds)).Select(x => new ModeDto(x.Id, x.Title));
            var responsibles = (await _responsibleRepository.GetByIdsAsync(responsibleIds)).Select(x => new ResponsibleDto(x.Id, x.Name));

            return new SuccessResult<JourneyDto>(new JourneyDto(
                journey.Id,
                journey.Title,
                journey.Steps.Select(step =>
                    new StepDto(
                        step.Id,
                        modes.FirstOrDefault(x => x.Id == step.ModeId),
                        responsibles.FirstOrDefault(x => x.Id == step.ResponsibleId)
                    )
                )
            ));
        }
    }
}
