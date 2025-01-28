﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommandHandler : IRequestHandler<UpdateRequirementsCommand, Result<string>>
    {
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagFunctionApiService _tagFunctionApiService;

        public UpdateRequirementsCommandHandler(
            ITagFunctionRepository tagFunctionRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagFunctionApiService tagFunctionApiService)
        {
            _tagFunctionRepository = tagFunctionRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagFunctionApiService = tagFunctionApiService;
        }

        public async Task<Result<string>> Handle(UpdateRequirementsCommand request, CancellationToken cancellationToken)
        {
            var tagFunction = await _tagFunctionRepository.GetByCodesAsync(request.TagFunctionCode, request.RegisterCode);
            var requirements = request.Requirements.ToList();

            if (tagFunction == null)
            {
                tagFunction = await CreateNewTagFunctionAsync(request.TagFunctionCode, request.RegisterCode, cancellationToken);
                if (tagFunction == null)
                {
                    return new NotFoundResult<string>($"TagFunction {request.TagFunctionCode} not found in register {request.RegisterCode}");
                }
            }
            else
            {
                RemoveChangedOrRemovedRequirementsFromTagFunction(tagFunction, requirements);
            }

            await AddRequirementsToTagFunctionAsync(tagFunction, requirements);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(tagFunction.RowVersion.ConvertToString());
        }

        private async Task<TagFunction> CreateNewTagFunctionAsync(
            string tagFunctionCode,
            string registerCode,
            CancellationToken cancellationToken)
        {
            var pcsTagFunction = await _tagFunctionApiService.TryGetTagFunctionAsync(
                _plantProvider.Plant,
                tagFunctionCode,
                registerCode,
                cancellationToken);
            
            if (pcsTagFunction == null)
            {
                return null;
            }
            var tagFunction = new TagFunction(_plantProvider.Plant, tagFunctionCode, pcsTagFunction.Description, registerCode);
            _tagFunctionRepository.Add(tagFunction);
            return tagFunction;
        }

        private async Task AddRequirementsToTagFunctionAsync(TagFunction tagFunction, IList<RequirementForCommand> requirements)
        {
            var reqDefIds = requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            foreach (var requirement in requirements)
            {
                if (tagFunction.Requirements.All(r => r.RequirementDefinitionId != requirement.RequirementDefinitionId))
                {
                    var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);

                    tagFunction.AddRequirement(new TagFunctionRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
                }
            }
        }

        private void RemoveChangedOrRemovedRequirementsFromTagFunction(TagFunction tagFunction, IList<RequirementForCommand> updatedRequirements)
        {
            var tagFunctionRequirements = tagFunction.Requirements;
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

            requirementsToBeRemoved.ForEach(tagFunction.RemoveRequirement);
        }
    }
}
