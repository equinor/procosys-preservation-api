using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.VoidTag
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
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            tag.Void();
            tag.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(tag.RowVersion.ConvertToString());
        }
    }
}
