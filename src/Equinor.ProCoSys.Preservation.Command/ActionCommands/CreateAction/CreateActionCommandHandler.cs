using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.ActionCommands.CreateAction
{
    public class CreateActionCommandHandler : IRequestHandler<CreateActionCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateActionCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(CreateActionCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithActionsByTagIdAsync(request.TagId);

            var actionToAdd = new Action(_plantProvider.Plant,
                request.Title,
                request.Description,
                request.DueTimeUtc);

            tag.AddAction(actionToAdd);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(actionToAdd.Id);
        }
    }
}
