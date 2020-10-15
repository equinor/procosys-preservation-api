using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Reschedule
{
    public class RescheduleCommandHandler : IRequestHandler<RescheduleCommand, Result<IEnumerable<IdAndRowVersion>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;

        // todo unit tests
        public RescheduleCommandHandler(
            IProjectRepository projectRepository,
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider)
        {
            _projectRepository = projectRepository;
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<IEnumerable<IdAndRowVersion>>> Handle(RescheduleCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.Tags.Select(x => x.Id));

            var tagsWithUpdatedRowVersion = new List<IdAndRowVersion>();

            foreach (var tag in tags)
            {
                tag.SetRowVersion(request.Tags.Single(x => x.Id == tag.Id).RowVersion);
                
                tag.Reschedule(request.Weeks, request.Direction);

                // todo check if this is correct! I guess that RowVersion is changed when saving
                tagsWithUpdatedRowVersion.Add(new IdAndRowVersion(tag.Id, tag.RowVersion.ConvertToString()));
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<IEnumerable<IdAndRowVersion>>(tagsWithUpdatedRowVersion);
        }
    }
}
