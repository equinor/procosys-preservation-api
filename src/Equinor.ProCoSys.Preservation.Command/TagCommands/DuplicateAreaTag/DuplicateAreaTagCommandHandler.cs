using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommandHandler : IRequestHandler<DuplicateAreaTagCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IDisciplineApiService _disciplineApiService;
        private readonly IAreaApiService _areaApiService;

        public DuplicateAreaTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider, 
            IDisciplineApiService disciplineApiService, 
            IAreaApiService areaApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _disciplineApiService = disciplineApiService;
            _areaApiService = areaApiService;
        }

        public async Task<Result<int>> Handle(DuplicateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var sourceTag = await _projectRepository.GetTagWithPreservationHistoryByTagIdAsync(request.TagId);

            var duplicatedTag = await DuplicateTagAsync(request, sourceTag);

            if (!await SetAreaDataSuccessfullyAsync(duplicatedTag, request.AreaCode))
            {
                return new NotFoundResult<int>($"Area with code {request.AreaCode} not found");
            }

            if (!await SetDisciplineDataSuccessfullyAsync(duplicatedTag, request.DisciplineCode))
            {
                return new NotFoundResult<int>($"Discipline with code {request.DisciplineCode} not found");
            }
            
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
                            Guid.NewGuid(),
                            request.GetTagNo(),
                            request.Description,
                            step,
                            requirements)
            {
                Remark = request.Remark,
                StorageArea = request.StorageArea
            };
            return duplicatedTag;
        }

        private async Task<bool> SetDisciplineDataSuccessfullyAsync(Tag tag, string disciplineCode)
        {
            var discipline = await _disciplineApiService.TryGetDisciplineAsync(_plantProvider.Plant, disciplineCode);
            if (discipline == null)
            {
                return false;
            }
            tag.SetDiscipline(disciplineCode, discipline.Description);
            return true;
        }

        private async Task<bool> SetAreaDataSuccessfullyAsync(Tag tag, string areaCode)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                return true;
            }
            var area = await _areaApiService.TryGetAreaAsync(_plantProvider.Plant, areaCode);
            if (area == null)
            {
                return false;
            }
            tag.SetArea(areaCode, area.Description);
            return true;
        }
    }
}
