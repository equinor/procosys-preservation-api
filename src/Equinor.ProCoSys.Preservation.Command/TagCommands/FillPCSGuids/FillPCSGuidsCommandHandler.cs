using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.FillPCSGuids
{
    public class FillPCSGuidsCommandHandler : IRequestHandler<FillPCSGuidsCommand, Result<IEnumerable<string>>>
    {
        private readonly ILogger<FillPCSGuidsCommand> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;
        private readonly IUnitOfWork _unitOfWork;

        public FillPCSGuidsCommandHandler(
            ILogger<FillPCSGuidsCommand> logger,
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

        public async Task<Result<IEnumerable<string>>> Handle(FillPCSGuidsCommand request, CancellationToken cancellationToken)
        {
            var allProjects = await _projectRepository.GetProjectsWithTagsAsync();
            var updates = new List<string>();
            foreach (var project in allProjects)
            {
                var tagsToFill = project.Tags.Where(t => t.TagType == TagType.Standard && !t.ProCoSysGuid.HasValue).ToList();
                _logger.LogInformation($"Found {tagsToFill.Count} in {project.Name} in {_plantProvider.Plant}");
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
                    }
                    else
                    {
                        var message = $"Did not find {tag.TagNo} in {project.Name} in {_plantProvider.Plant}";
                        _logger.LogWarning(message);
                        updates.Add(message);
                    }
                }
                updates.Add($"{project.Name}: {tagNos.Trim(new char[] {' ',','})}");
            }

            if (!request.DryRun && updates.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return new SuccessResult<IEnumerable<string>>(updates);
        }
    }
}
