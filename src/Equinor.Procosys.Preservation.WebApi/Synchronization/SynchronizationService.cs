using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Equinor.Procosys.Preservation.Query.GetTags;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly ILogger<SynchronizationService> _logger;
        private readonly IMediator _mediator;
        private readonly ITagApiService _tagApiService;

        public SynchronizationService(
            ILogger<SynchronizationService> logger,
            IMediator mediator,
            ITagApiService tagApiService)
        {
            _logger = logger;
            _mediator = mediator;
            _tagApiService = tagApiService;
        }

        public async Task Synchronize(CancellationToken cancellationToken)
        {
            await SynchronizeProjects();
            await SynchronizeResponsibles();
            await SynchronizeTagFunctions();
            await SynchronizeTags();
        }

        private async Task SynchronizeProjects()
        {
            await Task.CompletedTask;
        }

        private async Task SynchronizeResponsibles()
        {
            await Task.CompletedTask;
        }

        private async Task SynchronizeTagFunctions()
        {
            await Task.CompletedTask;
        }

        private async Task SynchronizeTags()
        {
            string plant = "PCS$TROLL_A";
            string projectName = "abc";
            var tags = await _mediator.Send(new GetTagsQuery(projectName));
            if (tags.ResultType == ServiceResult.ResultType.Ok)
            {
                var tagDetails = await _tagApiService.GetTagDetailsAsync(plant, projectName, tags.Data.Tags.Select(t => t.TagNo));
                foreach (var tag in tags.Data.Tags)
                {
                    var tagDetail = tagDetails.FirstOrDefault(t => t.TagNo == tag.TagNo);
                    if (tagDetail != null)
                    {
                        // TODO: Send update command
                    }
                    else
                    {
                        _logger.LogError($"TagNo: {tag.TagNo} could not be synchronized because the remote server did not provide any details for it.");
                    }
                }
            }
        }
    }
}
