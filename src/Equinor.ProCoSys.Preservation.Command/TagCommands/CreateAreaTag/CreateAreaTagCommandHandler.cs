using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommandHandler : IRequestHandler<CreateAreaTagCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IDisciplineApiService _disciplineApiService;
        private readonly IAreaApiService _areaApiService;
        private readonly IProjectImportService _projectImportService;

        public CreateAreaTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IDisciplineApiService disciplineApiService,
            IAreaApiService areaApiService,
            IProjectImportService projectImportService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _disciplineApiService = disciplineApiService;
            _areaApiService = areaApiService;
            _projectImportService = projectImportService;
        }

        public async Task<Result<int>> Handle(CreateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectImportService.TryGetOrImportProjectAsync(request.ProjectName, cancellationToken);
            if (project == null)
            {
                return new NotFoundResult<int>($"Project with name {request.ProjectName} not found");
            }

            var areaTagToAdd = await CreateAreaTagAsync(request);

            if (!await SetAreaDataSuccessfullyAsync(areaTagToAdd, request.AreaCode, cancellationToken))
            {
                return new NotFoundResult<int>($"Area with code {request.AreaCode} not found");
            }

            if (!await SetDisciplineDataSuccessfullyAsync(areaTagToAdd, request.DisciplineCode, cancellationToken))
            {
                return new NotFoundResult<int>($"Discipline with code {request.DisciplineCode} not found");
            }

            project.AddTag(areaTagToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(areaTagToAdd.Id);
        }

        private async Task<bool> SetDisciplineDataSuccessfullyAsync(Tag tag, string disciplineCode, CancellationToken cancellationToken)
        {
            var discipline = await _disciplineApiService.TryGetDisciplineAsync(_plantProvider.Plant, disciplineCode, cancellationToken);
            if (discipline == null)
            {
                return false;
            }

            tag.SetDiscipline(disciplineCode, discipline.Description);
            return true;
        }

        private async Task<bool> SetAreaDataSuccessfullyAsync(Tag tag, string areaCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                return true;
            }

            var area = await _areaApiService.TryGetAreaAsync(_plantProvider.Plant, areaCode, cancellationToken);
            if (area == null)
            {
                return false;
            }

            tag.SetArea(areaCode, area.Description);
            return true;
        }

        private async Task<Tag> CreateAreaTagAsync(CreateAreaTagCommand request)
        {
            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            string purchaseOrderNo = null;
            string calloff = null;

            if (request.TagType == TagType.PoArea)
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderCalloffCode))
                {
                    throw new Exception(
                        $"Tags of type {TagType.PoArea} must have {nameof(request.PurchaseOrderCalloffCode)}");
                }

                var poParts = request.PurchaseOrderCalloffCode.Split('/');
                purchaseOrderNo = poParts[0].Trim();
                if (poParts.Length > 1)
                {
                    calloff = poParts[1].Trim();
                }
            }

            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            return new Tag(
                _plantProvider.Plant,
                request.TagType,
                Guid.NewGuid(),
                request.GetTagNo(),
                request.Description,
                step,
                requirements)
            {
                PurchaseOrderNo = purchaseOrderNo,
                Calloff = calloff,
                Remark = request.Remark,
                StorageArea = request.StorageArea
            };
        }
    }
}
