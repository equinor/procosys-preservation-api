using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DeleteTag
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTagCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectAndTagWithPreservationHistoryByTagIdAsync(request.TagId);
            var tag = project.Tags.Single(t => t.Id == request.TagId);
            
            tag.SetRowVersion(request.RowVersion);
            project.RemoveTag(tag);
            _projectRepository.RemoveTag(tag);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
