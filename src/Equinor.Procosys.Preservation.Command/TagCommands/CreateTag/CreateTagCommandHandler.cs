using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<List<int>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public CreateTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<List<int>>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var addedTags = new List<Tag>();
            var project = await _projectRepository.GetByNameAsync(request.ProjectName);
            
            var tagDetailList = await _tagApiService.GetTagDetailsAsync(_plantProvider.Plant, request.ProjectName, request.TagNos);
            
            foreach (var tagNo in request.TagNos)
            {
                var tagDetails = tagDetailList.FirstOrDefault(td => td.TagNo == tagNo);
                if (tagDetails == null)
                {
                    return new NotFoundResult<List<int>>($"Details for Tag {tagNo} not found in project {request.ProjectName}");
                }
                if (project == null)
                {
                    project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription);
                    _projectRepository.Add(project);
                }

                var tagToAdd = await CreateTagAsync(tagDetails, request, reqDefs);
                project.AddTag(tagToAdd);
                addedTags.Add(tagToAdd);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<List<int>>(addedTags.Select(t => t.Id).ToList());
        }

        private async Task<Tag> CreateTagAsync(
            ProcosysTagDetails tagDetails,
            CreateTagCommand request, 
            IList<RequirementDefinition> reqDefs)
        {
            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            var tag = new Tag(
                _plantProvider.Plant,
                TagType.Standard,
                tagDetails.TagNo,
                tagDetails.Description,
                step,
                requirements)
            {
                Calloff = tagDetails.CallOffNo,
                CommPkgNo = tagDetails.CommPkgNo,
                McPkgNo = tagDetails.McPkgNo,
                PurchaseOrderNo = tagDetails.PurchaseOrderNo,
                Remark = request.Remark,
                StorageArea = request.StorageArea,
                TagFunctionCode = tagDetails.TagFunctionCode
            };
            tag.SetArea(tagDetails.AreaCode, tagDetails.AreaDescription);
            tag.SetDiscipline(tagDetails.DisciplineCode, tagDetails.DisciplineDescription);
            
            return tag;
        }
    }
}
