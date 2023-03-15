using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.VoidTag
{
    public class VoidTagCommandHandler : IRequestHandler<VoidTagCommand, Result<string>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VoidTagCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(VoidTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagOnlyByTagIdAsync(request.TagId);

            tag.IsVoided = true;
            tag.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(tag.RowVersion.ConvertToString());
        }
    }
}
