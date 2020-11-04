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
        private readonly IProjectApiService _projectApiService;
        private readonly IDisciplineApiService _disciplineApiService;
        private readonly IAreaApiService _areaApiService;

        public DuplicateAreaTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IProjectApiService projectApiService,
            IDisciplineApiService disciplineApiService,
            IAreaApiService areaApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _projectApiService = projectApiService;
            _disciplineApiService = disciplineApiService;
            _areaApiService = areaApiService;
        }

        public async Task<Result<int>> Handle(DuplicateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var sourceTag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var areaTagToAdd = await DuplicateAreaTagAsync(request, sourceTag);

            if (!await SetAreaDataSuccessfullyAsync(areaTagToAdd, request.AreaCode))
            {
                return new NotFoundResult<int>($"Area with code {request.AreaCode} not found");
            }

            if (!await SetDisciplineDataSuccessfullyAsync(areaTagToAdd, request.DisciplineCode))
            {
                return new NotFoundResult<int>($"Discipline with code {request.DisciplineCode} not found");
            }
            
            var project = await _projectRepository.GetProjectOnlyByTagIdAsync(request.TagId);
            project.AddTag(areaTagToAdd);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(areaTagToAdd.Id);
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

        private async Task<Tag> DuplicateAreaTagAsync(DuplicateAreaTagCommand request, Tag sourceTag)
        {
            //var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            //var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);


            //var requirements = new List<TagRequirement>();
            //foreach (var requirement in request.Requirements)
            //{
            //    var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
            //    requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            //}

            //string purchaseOrderNo = null;
            //string calloff = null;

            //if (request.TagType == TagType.PoArea)
            //{
            //    if (string.IsNullOrEmpty(request.PurchaseOrderCalloffCode))
            //    {
            //        throw new Exception($"Tags of type {TagType.PoArea} must have {nameof(request.PurchaseOrderCalloffCode)}");
            //    }
            //    var poParts = request.PurchaseOrderCalloffCode.Split('/');
            //    purchaseOrderNo = poParts[0].Trim();
            //    if (poParts.Length > 1)
            //    {
            //        calloff = poParts[1].Trim();
            //    }
            //}

            //var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            //return new Tag(
            //    _plantProvider.Plant,
            //    request.TagType,
            //    request.GetTagNo(),
            //    request.Description,
            //    step,
            //    requirements)
            //{
            //    PurchaseOrderNo = purchaseOrderNo,
            //    Calloff = calloff,
            //    Remark = request.Remark,
            //    StorageArea = request.StorageArea
            //};
            throw new NotImplementedException();
        }

        private async Task<Project> CreateProjectAsync(string projectName)
        {
            var mainProject = await _projectApiService.TryGetProjectAsync(_plantProvider.Plant, projectName);
            if (mainProject == null)
            {
                return null;
            }

            var project = new Project(_plantProvider.Plant, projectName, mainProject.Description);
            _projectRepository.Add(project);
            return project;
        }
    }
}
