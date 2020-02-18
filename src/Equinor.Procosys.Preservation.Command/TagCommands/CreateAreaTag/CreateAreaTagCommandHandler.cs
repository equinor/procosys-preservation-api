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
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
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

        public CreateAreaTagCommandHandler(
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
            _areaApiService = areaApiService;
            _disciplineApiService = disciplineApiService;
        }

        public async Task<Result<int>> Handle(CreateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);

            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs =
                await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var project = await _projectRepository.GetByNameAsync(request.ProjectName);
            
            if (project == null)
            {
                // todo Must get project from main here to be able to create it with Description. Later PBI 71457 since we don't have that endpoint in MainApi yet");
                return new NotFoundResult<int>($"Project {request.ProjectName} not found in preservation");
                //project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription);
                //_projectRepository.Add(project);
            }

            var disciplines = await _disciplineApiService.GetDisciplines(_plantProvider.Plant);
            var discipline = disciplines.SingleOrDefault(d => d.Code == request.DisciplineCode);
            if (discipline == null)
            {
                return new NotFoundResult<int>($"Discipline with code {request.DisciplineCode} not found");
            }

            if (!string.IsNullOrEmpty(request.AreaCode))
            {
                var areas = await _areaApiService.GetAreas(_plantProvider.Plant);

                var area = areas.SingleOrDefault(a => a.Code == request.AreaCode);

                if (area == null)
                {
                    return new NotFoundResult<int>($"Area with code {request.AreaCode} not found");
                }
            }

            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            var tagToAdd = new Tag(
                _plantProvider.Plant,
                request.TagType,
                request.GetTagNo(),
                request.Description,
                request.AreaCode,
                null,
                request.DisciplineCode,
                null,
                null,
                null,
                request.Remark,
                null,
                step,
                requirements);
            
            project.AddTag(tagToAdd);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(tagToAdd.Id);
        }
    }
}
