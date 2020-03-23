using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommandHandler : IRequestHandler<UpdateRequirementsCommand, Result<Unit>>
    {
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UpdateRequirementsCommandHandler(
            ITagFunctionRepository tagFunctionRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _tagFunctionRepository = tagFunctionRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<Unit>> Handle(UpdateRequirementsCommand request, CancellationToken cancellationToken)
        {
            var tagFunction = await _tagFunctionRepository.GetByNameAsync(request.TagFunctionCode, request.RegisterCode);

            if (tagFunction == null)
            {
                // todo Need to get TagFunction Description from Main
                tagFunction = new TagFunction(_plantProvider.Plant, request.TagFunctionCode, "ToDo", request.RegisterCode);
                _tagFunctionRepository.Add(tagFunction);
            }
            else
            {
                RemoveChangedOrRemovedRequirements(tagFunction, request.Requirements.ToList());
            }

            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs =
                await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            foreach (var requirement in request.Requirements)
            {
                if (tagFunction.Requirements.All(r => r.RequirementDefinitionId != requirement.RequirementDefinitionId))
                {
                    var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);

                    tagFunction.AddRequirement(new TagFunctionRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private void RemoveChangedOrRemovedRequirements(TagFunction existingTagFunction, IList<Requirement> updatedRequirements)
        {
            var tagFunctionRequirements = existingTagFunction.Requirements;
            var requirementsToBeRemoved = new List<TagFunctionRequirement>();

            foreach (var existingRequirement in tagFunctionRequirements)
            {
                var updatedRequirement = updatedRequirements.FirstOrDefault(r =>
                    r.RequirementDefinitionId == existingRequirement.RequirementDefinitionId);

                if (updatedRequirement == null || existingRequirement.IntervalWeeks != updatedRequirement.IntervalWeeks)
                {
                    requirementsToBeRemoved.Add(existingRequirement);
                }
            }

            requirementsToBeRemoved.ForEach(existingTagFunction.RemoveRequirement);
        }
    }
}
