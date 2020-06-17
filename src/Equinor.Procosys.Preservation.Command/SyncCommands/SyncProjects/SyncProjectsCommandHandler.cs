using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects
{
    public class SyncProjectsCommandHandler : IRequestHandler<SyncProjectsCommand, Result<Unit>>
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionCache _permissionCache;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectApiService _projectApiService;
        private readonly ITagApiService _tagApiService;
        private readonly ILogger<SyncProjectsCommandHandler> _logger;

        public SyncProjectsCommandHandler(
            IPlantProvider plantProvider, 
            IUnitOfWork unitOfWork,
            IPermissionCache permissionCache,
            ICurrentUserProvider currentUserProvider,
            IProjectRepository projectRepository,
            IProjectApiService projectApiService,
            ITagApiService tagApiService,
            ILogger<SyncProjectsCommandHandler> logger)
        {
            _plantProvider = plantProvider;
            _unitOfWork = unitOfWork;
            _permissionCache = permissionCache;
            _currentUserProvider = currentUserProvider;
            _projectRepository = projectRepository;
            _projectApiService = projectApiService;
            _tagApiService = tagApiService;
            _logger = logger;
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
            var projects = await _projectRepository.GetAllProjectsOnlyAsync();
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            
            var projectsWhichUserCanAccess = await _permissionCache.GetProjectNamesForUserOidAsync(plant, currentUserOid);
            
            foreach (var project in projects)
            {
                if (!projectsWhichUserCanAccess.Contains(project.Name))
                {
                    _logger.LogWarning($"Current user with oid {currentUserOid} don't have access to project {project.Name}. Synchronization skipped ...");
                    continue;
                }

                var pcsProject = await _projectApiService.GetProjectAsync(plant, project.Name);
                project.IsClosed = pcsProject.IsClosed;
                project.Description = pcsProject.Description;

                await SyncTagData(plant, project.Name);
            }
        }

        private async Task SyncTagData(string plant, string projectName)
        {
            var standardTags = await _projectRepository.GetStandardTagsInProjectOnlyAsync(projectName);
            var allTagNos = standardTags.Select(t => t.TagNo).ToList();

            var page = 0;
            // Use relative small page size since TagNos are added to querystring of url and maxlength is 2000
            var pageSize = 50;
            IEnumerable<string> pageWithTagNos;
            do
            {
                pageWithTagNos = allTagNos.Skip(pageSize * page).Take(pageSize).ToList();

                if (pageWithTagNos.Any())
                {
                    var pcsTags = await _tagApiService.GetTagDetailsAsync(plant, projectName, pageWithTagNos);
                    foreach (var pcsTag in pcsTags)
                    {
                        var tag = standardTags.Single(t => t.TagNo == pcsTag.TagNo);

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

                page++;

            } while (pageWithTagNos.Count() == pageSize);
        }
    }
}
