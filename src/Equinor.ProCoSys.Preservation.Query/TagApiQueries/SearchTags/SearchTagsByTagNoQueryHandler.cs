using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagNoQueryHandler : IRequestHandler<SearchTagsByTagNoQuery, Result<List<PCSTagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;
        private readonly ILogger<SearchTagsByTagNoQueryHandler> _logger;

        public SearchTagsByTagNoQueryHandler(
            IReadOnlyContext context,
            ITagApiService tagApiService,
            IPlantProvider plantProvider,
            ILogger<SearchTagsByTagNoQueryHandler> logger)
        {
            _context = context;
            _tagApiService = tagApiService;
            _plantProvider = plantProvider;
            _logger = logger;
        }

        public async Task<Result<List<PCSTagDto>>> Handle(SearchTagsByTagNoQuery request, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            var apiTags = await _tagApiService
                .SearchTagsByTagNoAsync(_plantProvider.Plant, request.ProjectName, request.StartsWithTagNo)
                ?? new List<PCSTagOverview>();
            var msg = $"SearchTagsByTagNoQueryHandler: {stopWatch.Elapsed.TotalMilliseconds}ms elapsed getting {apiTags.Count} tags from Main.";

            var presTagNos = await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where p.Name == request.ProjectName
                select tag.TagNo).ToListAsync(cancellationToken);
            msg += $" {stopWatch.Elapsed.TotalMilliseconds}ms elapsed getting getting {presTagNos.Count} preservation tags.";

            // Join all tags from API with preservation tags on TagNo. If a tag is not in preservation scope, use default value (null).
            var combinedTags = apiTags
                .GroupJoin(presTagNos,
                    apiTag => apiTag.TagNo,
                    presTagNo => presTagNo,
                    (x, y) =>
                        new {ApiTag = x, PresTagNo = y})
                .SelectMany(x => x.PresTagNo.DefaultIfEmpty(),
                    (x, y) =>
                        new PCSTagDto(
                            x.ApiTag.TagNo,
                            x.ApiTag.Description,
                            x.ApiTag.PurchaseOrderTitle,
                            x.ApiTag.CommPkgNo,
                            x.ApiTag.McPkgNo,
                            x.ApiTag.TagFunctionCode,
                            x.ApiTag.RegisterCode,
                            x.ApiTag.MccrResponsibleCodes,
                            y != null))
                .ToList();

            msg += $" {stopWatch.Elapsed.TotalMilliseconds}ms creating DTO.";
            _logger.LogInformation(msg);
            return new SuccessResult<List<PCSTagDto>>(combinedTags);
        }
    }
}
