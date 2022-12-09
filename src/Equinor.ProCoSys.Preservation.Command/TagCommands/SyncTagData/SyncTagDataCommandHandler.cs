using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.SyncTagData
{
    public class SyncTagDataCommandHandler : IRequestHandler<SyncTagDataCommand, Result<Unit>>
    {
        private readonly ILogger<SyncTagDataCommand> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SyncTagDataCommandHandler(
            ILogger<SyncTagDataCommand> logger,
            IPlantProvider plantProvider,
            IProjectRepository projectRepository,
            ITagApiService tagApiService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _plantProvider = plantProvider;
            _projectRepository = projectRepository;
            _tagApiService = tagApiService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(SyncTagDataCommand request, CancellationToken cancellationToken)
        {
            var allProjects = await _projectRepository.GetProjectsWithTagsAsync();
            var count = 0;
            foreach (var project in allProjects)
            {
                var tagsToFill = project.Tags.Where(t => t.TagType == TagType.Standard && !t.ProCoSysGuid.HasValue).ToList();
                _logger.LogInformation($"SyncTagData: Found {tagsToFill.Count} in project {project.Name}, plant {_plantProvider.Plant}");
                if (tagsToFill.Count == 0)
                {
                    continue;
                }

                var tagsToFillTagNo = tagsToFill.Select(t => t.TagNo).ToList();
                var pcsTagDetails = await _tagApiService.GetTagDetailsAsync(_plantProvider.Plant, project.Name, tagsToFillTagNo, true);
                if (pcsTagDetails.Count == 0)
                {
                    continue;
                }

                var tagNos = string.Empty;
                foreach (var tag in tagsToFill)
                {
                    var pcsTagDetail = pcsTagDetails.SingleOrDefault(t => t.TagNo == tag.TagNo);
                    if (pcsTagDetail != null)
                    {
                        tag.ProCoSysGuid = pcsTagDetail.ProCoSysGuid;
                        tag.McPkgProCoSysGuid = pcsTagDetail.McPkgProCoSysGuid;
                        tag.CommPkgProCoSysGuid = pcsTagDetail.CommPkgProCoSysGuid;
                        tagNos += pcsTagDetail.TagNo + ", ";
                        count++;

                        if (pcsTagDetail.IsVoided != tag.IsVoidedInSource)
                        {
                            _logger.LogWarning($"SyncTagData: {tag.TagNo} in {project.Name} in {_plantProvider.Plant}. Setting IsVoidedInSource = {pcsTagDetail.IsVoided}");
                            tag.IsVoidedInSource = pcsTagDetail.IsVoided;
                        }
                    }
                    else if (!tag.IsDeletedInSource)
                    { 
                        _logger.LogWarning($"SyncTagData: Did not find {tag.TagNo} in {project.Name} in {_plantProvider.Plant}. Setting IsDeletedInSource");
                        tagNos += tag.TagNo + ", ";
                        tag.IsDeletedInSource = true;
                        count++;
                    }
                }
                _logger.LogInformation($"SyncTagData: Tags updated in {project.Name}: {tagNos.Trim(new char[] {' ',','})}");
            }

            if (request.SaveChanges && count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
