using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.AutoScopeTags
{
    public class AutoScopeTagsCommandHandler : IRequestHandler<AutoScopeTagsCommand, Result<List<int>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public AutoScopeTagsCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository, 
            ITagFunctionRepository tagFunctionRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _tagFunctionRepository = tagFunctionRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<List<int>>> Handle(AutoScopeTagsCommand request, CancellationToken cancellationToken)
        {
            var tagDetailList = await _tagApiService.GetTagDetailsAsync(_plantProvider.Plant, request.ProjectName, request.TagNos);

            var uniqueTagFunctionCodesRegisterCodes = tagDetailList
                .Distinct(new ProcosysTagDetailsComparer()).Select(Key);

            var tagFunctionsWithRequirements = await _tagFunctionRepository.GetAllWithRequirementsAsync();

            var neededTagFunctionsWithRequirements = tagFunctionsWithRequirements
                .Where(tf => uniqueTagFunctionCodesRegisterCodes.Contains(Key(tf)))
                .ToList();

            var reqDefIds = neededTagFunctionsWithRequirements
                .SelectMany(r => r.Requirements)
                .Select(r => r.RequirementDefinitionId)
                .Distinct()
                .ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var addedTags = new List<Tag>();
            var project = await _projectRepository.GetByNameAsync(request.ProjectName);
            
            foreach (var tagNo in request.TagNos)
            {
                var tagDetails = tagDetailList.FirstOrDefault(td => td.TagNo == tagNo);
                if (tagDetails == null)
                {
                    return new NotFoundResult<List<int>>($"Details for Tag {tagNo} not found in project {request.ProjectName}");
                }

                var tagFunctionWithRequirement =
                    neededTagFunctionsWithRequirements.SingleOrDefault(tf => Key(tf) == Key(tagDetails));
                
                if (tagFunctionWithRequirement == null)
                {
                    return new NotFoundResult<List<int>>($"TagFunction for {Key(tagDetails)} not found with Requirements defined");
                }

                if (project == null)
                {
                    project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription);
                    _projectRepository.Add(project);
                }

                var tagToAdd = await CreateTagAsync(tagDetails, request, tagFunctionWithRequirement, reqDefs);
                project.AddTag(tagToAdd);
                addedTags.Add(tagToAdd);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<List<int>>(addedTags.Select(t => t.Id).ToList());
        }

        private string Key(ProcosysTagDetails details) => $"{details.TagFunctionCode}|{details.RegisterCode}";
        
        private string Key(TagFunction tagFunction) => $"{tagFunction.Code}|{tagFunction.RegisterCode}";

        private async Task<Tag> CreateTagAsync(
            ProcosysTagDetails tagDetails,
            AutoScopeTagsCommand request, 
            TagFunction tagFunctionWithRequirements,
            IList<RequirementDefinition> reqDefs)
        {
            var requirements = new List<TagRequirement>();
            foreach (var requirement in tagFunctionWithRequirements.Requirements)
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

        class ProcosysTagDetailsComparer : IEqualityComparer<ProcosysTagDetails>
        {
            public bool Equals(ProcosysTagDetails d1, ProcosysTagDetails d2)
            {
                if (d2 == null && d1 == null)
                {
                    return true;
                }

                if (d1 == null || d2 == null)
                {
                    return false;
                }

                if (d1.RegisterCode == d2.RegisterCode && d1.TagFunctionCode == d2.TagFunctionCode)
                {
                    return true;
                }
                
                return false;
            }

            public int GetHashCode(ProcosysTagDetails d)
            {
                var hCode = d.RegisterCode.GetHashCode() ^ d.TagFunctionCode.GetHashCode();
                return hCode.GetHashCode();
            }
        }
    }
}
