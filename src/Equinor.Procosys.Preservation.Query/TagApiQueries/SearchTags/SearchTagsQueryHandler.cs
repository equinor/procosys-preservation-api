using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsQueryHandler : IRequestHandler<SearchTagsQuery, Result<List<ProcosysTagDto>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;

        public SearchTagsQueryHandler(IProjectRepository projectRepository, ITagApiService tagApiService, IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _tagApiService = tagApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<ProcosysTagDto>>> Handle(SearchTagsQuery request, CancellationToken cancellationToken)
        {
            var apiTags = await _tagApiService
                .GetTags(_plantProvider.Plant, request.ProjectName, request.StartsWithTagNo)
                ?? new List<ProcosysTagOverview>();
            var presTags = await _projectRepository.GetAllTagsInProjectAsync(request.ProjectName)
                ?? new List<Tag>();

            // Join all tags from API with preservation tags on TagNo. If a tag is not in preservation scope, use default value (null).
            var combinedTags = apiTags
                .GroupJoin(presTags,
                    apiTag => apiTag.TagNo,
                    presTag => presTag.TagNo,
                    (x, y) =>
                        new { ApiTag = x, PresTag = y })
                .SelectMany(x =>
                    x.PresTag.DefaultIfEmpty(),
                    (x, y) =>
                        new ProcosysTagDto(x.ApiTag.TagNo, x.ApiTag.Description, x.ApiTag.PurchaseOrderNo, x.ApiTag.CommPkgNo, x.ApiTag.McPkgNo, y != null))
                .ToList();

            return new SuccessResult<List<ProcosysTagDto>>(combinedTags);
        }
    }
}
