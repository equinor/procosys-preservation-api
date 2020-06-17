using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects
{
    public class SyncProjectsCommandHandler : IRequestHandler<SyncProjectsCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IProjectApiService _projectApiService;
        private readonly ITagApiService _tagApiService;

        public SyncProjectsCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider, 
            IProjectApiService projectApiService,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _projectApiService = projectApiService;
            _tagApiService = tagApiService;
        }

        public async Task<Result<Unit>> Handle(SyncProjectsCommand request, CancellationToken cancellationToken)
        {
            var plant = _plantProvider.Plant;

            await SyncProjectData(plant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task SyncProjectData(string plant)
        {
            var projects = await _projectRepository.GetAllProjectsWithTagsOnlyAsync();

            foreach (var project in projects)
            {
                var pcsProject = await _projectApiService.GetProjectAsync(plant, project.Name);
                if (pcsProject != null)
                {
                    project.IsClosed = pcsProject.IsClosed;
                    project.Description = pcsProject.Description;

                    await SyncTagData(plant, project.Name, project.Tags);
                }
            }
        }

        private async Task SyncTagData(string plant, string projectName, IReadOnlyCollection<Tag> tags)
        {
            var tagNos = tags.Select(t => t.TagNo);
            var pcsTags = await _tagApiService.GetTagDetailsAsync(plant, projectName, tagNos);

            // todo implement "paging" ... take 1000 and 1000
            foreach (var tag in tags)
            {
                var pcsTag = pcsTags.SingleOrDefault(t => t.TagNo == tag.TagNo);

                if (pcsTag != null)
                {
                    tag.SetArea(pcsTag.AreaCode, pcsTag.Description);
                    tag.SetDiscipline(pcsTag.DisciplineCode, pcsTag.DisciplineDescription);
                    tag.Calloff = pcsTag.CallOffNo;
                    tag.PurchaseOrderNo = pcsTag.PurchaseOrderNo;
                    tag.CommPkgNo = pcsTag.CommPkgNo;
                    tag.Description = pcsTag.Description;
                    tag.McPkgNo = pcsTag.McPkgNo;
                    tag.TagFunctionCode = pcsTag.TagFunctionCode;
                }
            }
        }
    }
}
