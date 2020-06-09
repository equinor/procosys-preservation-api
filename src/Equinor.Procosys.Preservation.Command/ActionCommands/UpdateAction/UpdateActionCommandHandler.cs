using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction
{
    public class UpdateActionCommandHandler : IRequestHandler<UpdateActionCommand, Result<string>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateActionCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateActionCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var action = tag.Actions.Single(a => a.Id == request.ActionId);

            action.Title = request.Title;
            action.Description = request.Description;
            action.SetDueTime(request.DueTimeUtc);
            action.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<string>(action.RowVersion.ConvertToString());
        }
    }
}
