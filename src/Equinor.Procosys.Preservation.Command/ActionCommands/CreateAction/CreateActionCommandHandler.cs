using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction
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
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var actionToAdd = new Action(
                 _plantProvider.Plant,
                request.Title,
                request.Description,
                request.DueTimeUtc);

            tag.AddAction(actionToAdd);

            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<int>(actionToAdd.Id);
        }
    }
}
