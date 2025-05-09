﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
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
            requirementDefinition.DefaultIntervalWeeks = request.DefaultIntervalWeeks;

            var updateFieldIds = request.UpdateFields.Select(u => u.Id);
            var excludedFields = requirementDefinition.Fields.Where(f => !updateFieldIds.Contains(f.Id)).ToList();
            foreach (var fieldToDelete in excludedFields)
            {
                requirementDefinition.RemoveField(fieldToDelete);
                _requirementTypeRepository.RemoveField(fieldToDelete);
            }

            foreach (var f in request.UpdateFields)
            {
                var fieldToUpdate = requirementDefinition.Fields.Single(field => field.Id == f.Id);
                if (fieldToUpdate.IsVoided && !f.IsVoided)
                {
                    fieldToUpdate.IsVoided = false;
                }
                else if (!fieldToUpdate.IsVoided && f.IsVoided)
                {
                    fieldToUpdate.IsVoided = true;
                }
                fieldToUpdate.Label = f.Label;
                fieldToUpdate.Unit = f.Unit;
                fieldToUpdate.ShowPrevious = f.ShowPrevious;
                fieldToUpdate.SortKey = f.SortKey;
                fieldToUpdate.SetRowVersion(f.RowVersion);
                
                requirementDefinition.AddDomainEvent(new ChildModifiedEvent<RequirementDefinition, Field>(requirementDefinition, fieldToUpdate));
            }

            foreach (var f in request.NewFields)
            {
                requirementDefinition.AddField(new Field(_plantProvider.Plant, f.Label, f.FieldType, f.SortKey, f.Unit, f.ShowPrevious));
            }

            requirementDefinition.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(requirementDefinition.RowVersion.ConvertToString());
        }
    }
}
