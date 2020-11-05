using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Project;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommandHandler : IRequestHandler<DuplicateAreaTagCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        // todo unit test
        public DuplicateAreaTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(DuplicateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var sourceTag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var duplicatedTag = await DuplicateTagAsync(request, sourceTag);
            
            var project = await _projectRepository.GetProjectOnlyByTagIdAsync(request.TagId);
            project.AddTag(duplicatedTag);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(duplicatedTag.Id);
        }

        private async Task<Tag> DuplicateTagAsync(DuplicateAreaTagCommand request, Tag sourceTag)
        {
            if (!sourceTag.IsReadyToBeDuplicated())
            {
                throw new Exception($"Tag {sourceTag.TagNo} of type {sourceTag.TagType} can't be duplicated");
            }

            var reqDefIds = sourceTag.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var requirements = new List<TagRequirement>();
            foreach (var requirement in sourceTag.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            var step = await _journeyRepository.GetStepByStepIdAsync(sourceTag.StepId);
            var duplicatedTag = new Tag(
                            _plantProvider.Plant,
                            request.TagType,
                            request.GetTagNo(),
                            request.Description,
                            step,
                            requirements)
            {
                Remark = request.Remark,
                StorageArea = request.StorageArea
            };
            duplicatedTag.SetDiscipline(sourceTag.DisciplineCode, sourceTag.DisciplineDescription);
            duplicatedTag.SetArea(sourceTag.AreaCode, sourceTag.AreaDescription);
            return duplicatedTag;
        }
    }
}
