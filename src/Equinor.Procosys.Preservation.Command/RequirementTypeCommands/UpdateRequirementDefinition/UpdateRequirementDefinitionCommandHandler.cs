using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommandHandler : IRequestHandler<UpdateRequirementDefinitionCommand, Result<string>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UpdateRequirementDefinitionCommandHandler(
            IRequirementTypeRepository requirementTypeRepository, 
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<string>> Handle(UpdateRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);
            var requirementDefinition =
                requirementType.RequirementDefinitions.Single(rd => rd.Id == request.RequirementDefinitionId);

            requirementDefinition.Title = request.Title;
            requirementDefinition.SortKey = request.SortKey;
            requirementDefinition.Usage = request.Usage;
            //todo må legge til needs user input?
            requirementDefinition.DefaultIntervalWeeks = request.DefaultIntervalWeeks;

            var fieldsToDelete = requirementDefinition.Fields.Where(f => request.UpdateFields.All(up => up.Id != f.Id));

            foreach (var f in request.UpdateFields)
            {
                requirementDefinition.UpdateField(f.Id, f.Label, f.FieldType, f.SortKey, f.RowVersion, f.Unit, f.ShowPrevious);
            }

            foreach (var f in fieldsToDelete)
            {
                requirementDefinition.RemoveField(new Field(_plantProvider.Plant, f.Label, f.FieldType, f.SortKey, f.Unit, f.ShowPrevious));
            }

            foreach (var f in request.NewFields)
            {
                requirementDefinition.AddField(new Field(_plantProvider.Plant, f.Label, f.FieldType, f.SortKey, f.Unit, f.ShowPrevious));
            }

            requirementDefinition.SetRowVersion(request.RowVersion);
            requirementType.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(requirementType.RowVersion.ConvertToString());
        }
    }
}
