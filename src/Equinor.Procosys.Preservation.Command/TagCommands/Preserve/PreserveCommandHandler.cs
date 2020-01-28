using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommandHandler : IRequestHandler<PreserveCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeService _timeService;

        public PreserveCommandHandler(
            IProjectRepository projectRepository,
            IPersonRepository personRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _personRepository = personRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(PreserveCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.TagIds);
            var dummy = await _personRepository.GetByOidAsync(Guid.Empty);
            if (dummy == null)
            {
                dummy = new Person(Guid.Empty, "Ole", "Olsen");
                _personRepository.Add(dummy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            foreach (var tag in tags)
            {
                tag.Preserve(_timeService.GetCurrentTimeUtc(), dummy, request.BulkPreserved);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
