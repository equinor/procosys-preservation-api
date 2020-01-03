using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQueryHandler : IRequestHandler<GetModeByIdQuery, Result<ModeDto>>
    {
        private readonly IModeRepository _modeRepository;

        public GetModeByIdQueryHandler(IModeRepository modeRepository) => _modeRepository = modeRepository;

        public async Task<Result<ModeDto>> Handle(GetModeByIdQuery request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.Id);
            return new SuccessResult<ModeDto>(new ModeDto(mode.Id, mode.Title));
        }
    }
}
