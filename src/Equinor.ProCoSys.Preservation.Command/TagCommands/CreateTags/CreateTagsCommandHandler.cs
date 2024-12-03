using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.CreateTags
{
    public class CreateTagsCommandHandler : IRequestHandler<CreateTagsCommand, Result<List<int>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public CreateTagsCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IModeRepository modeRepository, 
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<List<int>>> Handle(CreateTagsCommand request, CancellationToken cancellationToken)
        {
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);
            var mode = await _modeRepository.GetByIdAsync(step.ModeId);

            var addedTags = new List<Tag>();
            var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);
            
            var tagDetailList = await _tagApiService.GetTagDetailsAsync(_plantProvider.Plant, request.ProjectName, request.TagNos);
            
            foreach (var tagNo in request.TagNos)
            {
                var tagDetails = tagDetailList.FirstOrDefault(td => td.TagNo == tagNo);
                if (tagDetails == null)
                {
                    return new NotFoundResult<List<int>>($"Details for Tag {tagNo} not found in project {request.ProjectName}");
                }

                // Hack TECH This type of validation should be in validator not in handler.
                // Since we don't want to ask main api for same data both in validator and here, we do it here only
                if (mode.ForSupplier && string.IsNullOrEmpty(tagDetails.PurchaseOrderNo))
                {
                    return new NotFoundResult<List<int>>($"Purchase Order for {tagNo} not found in project {request.ProjectName}. Tag can not be in a Supplier step");
                }

                if (project == null)
                {
                    project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription, tagDetails.ProjectProCoSysGuid);
                    _projectRepository.Add(project);
                }

                var tagToAdd = CreateTag(request, step, tagDetails, reqDefs);

                project.AddTag(tagToAdd);
                addedTags.Add(tagToAdd);
            }

            // Todo Remove Migration handling when migration period from old to new preservation in ProCoSys is over
            await _tagApiService.MarkTagsAsMigratedAsync(_plantProvider.Plant, tagDetailList.Select(t => t.Id));
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<List<int>>(addedTags.Select(t => t.Id).ToList());
        }

        private Tag CreateTag(
            CreateTagsCommand request, 
            Step step,
            PCSTagDetails tagDetails,
            IList<RequirementDefinition> reqDefs)
        {
            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            var tag = new Tag(
                _plantProvider.Plant,
                TagType.Standard,
                tagDetails.ProCoSysGuid,
                tagDetails.TagNo,
                tagDetails.Description,
                step,
                requirements)
            {
                Calloff = tagDetails.CallOffNo,
                CommPkgNo = tagDetails.CommPkgNo,
                CommPkgProCoSysGuid = tagDetails.CommPkgProCoSysGuid,
                McPkgNo = tagDetails.McPkgNo,
                McPkgProCoSysGuid = tagDetails.McPkgProCoSysGuid,
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
