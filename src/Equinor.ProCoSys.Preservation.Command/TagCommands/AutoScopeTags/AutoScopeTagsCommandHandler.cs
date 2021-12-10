using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.AutoScopeTags
{
    public class AutoScopeTagsCommandHandler : IRequestHandler<AutoScopeTagsCommand, Result<List<int>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public AutoScopeTagsCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository, 
            IModeRepository modeRepository, 
            ITagFunctionRepository tagFunctionRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _tagFunctionRepository = tagFunctionRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<List<int>>> Handle(AutoScopeTagsCommand request, CancellationToken cancellationToken)
        {
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            var tagDetailList = await _tagApiService.GetTagDetailsAsync(_plantProvider.Plant, request.ProjectName, request.TagNos);
            var mode = await _modeRepository.GetByIdAsync(step.ModeId);

            var tagFunctionsWithRequirements = await GetNeededTagFunctionsWithRequirementsAsync(tagDetailList);

            var reqDefIds = tagFunctionsWithRequirements
                .SelectMany(r => r.Requirements)
                .Where(r => !r.IsVoided)
                .Select(r => r.RequirementDefinitionId)
                .Distinct()
                .ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var addedTags = new List<Tag>();
            var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);
            
            foreach (var tagNo in request.TagNos)
            {
                var tagDetails = tagDetailList.FirstOrDefault(td => td.TagNo == tagNo);
                if (tagDetails == null)
                {
                    return new NotFoundResult<List<int>>($"Details for Tag {tagNo} not found in project {request.ProjectName}");
                }

                var tagFunctionWithRequirement =
                    tagFunctionsWithRequirements.SingleOrDefault(tf => Key(tf) == Key(tagDetails));
                
                if (tagFunctionWithRequirement == null)
                {
                    return new NotFoundResult<List<int>>($"TagFunction for {Key(tagDetails)} not found with requirements defined");
                }

                // Hack This type of validation should be in validator not in handler.
                // Since we don't want to ask main api for same data both in validator and here, we do it here only
                if (mode.ForSupplier && string.IsNullOrEmpty(tagDetails.PurchaseOrderNo))
                {
                    return new NotFoundResult<List<int>>($"Purchase Order for {tagNo} not found in project {request.ProjectName}.");
                }

                if (project == null)
                {
                    project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription);
                    _projectRepository.Add(project);
                }

                var tagToAdd = CreateTag(request, step, tagDetails, tagFunctionWithRequirement, reqDefs);

                project.AddTag(tagToAdd);
                addedTags.Add(tagToAdd);
            }
            
            // Todo Remove Migration handling when migration period from old to new preservation in ProCoSys is over
            await _tagApiService.MarkTagsAsMigratedAsync(_plantProvider.Plant, tagDetailList.Select(t => t.Id));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<List<int>>(addedTags.Select(t => t.Id).ToList());
        }

        private string Key(PCSTagDetails details) => $"{details.TagFunctionCode}|{details.RegisterCode}";
        
        private string Key(TagFunction tagFunction) => $"{tagFunction.Code}|{tagFunction.RegisterCode}";

        private async Task<List<TagFunction>> GetNeededTagFunctionsWithRequirementsAsync(IList<PCSTagDetails> tagDetailList)
        {
            var uniqueTagFunctionCodesRegisterCodes = tagDetailList.Distinct(new PCSTagDetailsComparer()).Select(Key);

            var tagFunctionsWithRequirements = await _tagFunctionRepository.GetAllNonVoidedWithRequirementsAsync();

            return tagFunctionsWithRequirements
                .Where(tf => uniqueTagFunctionCodesRegisterCodes.Contains(Key(tf)))
                .ToList();
        }

        private Tag CreateTag(
            AutoScopeTagsCommand request, 
            Step step,
            PCSTagDetails tagDetails,
            TagFunction tagFunctionWithRequirements,
            IList<RequirementDefinition> reqDefs)
        {
            var requirements = new List<TagRequirement>();
            foreach (var requirement in tagFunctionWithRequirements.Requirements.Where(r => !r.IsVoided))
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

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
