using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery
{
    public class AllAvailableTagsQueryHandler : IRequestHandler<AllAvailableTagsQuery, Result<List<ProcosysTagDto>>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;

        public AllAvailableTagsQueryHandler(ITagRepository tagRepository, ITagApiService tagApiService, IPlantProvider plantProvider)
        {
            _tagRepository = tagRepository;
            _tagApiService = tagApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<ProcosysTagDto>>> Handle(AllAvailableTagsQuery request, CancellationToken cancellationToken)
        {
            var apiTags = await _tagApiService.GetTags(_plantProvider.Plant, request.ProjectName, request.StartsWithTagNo);
            var presTags = await _tagRepository.GetAllAsync();

            var combinedTags = apiTags
                .GroupJoin(presTags, apiTag => apiTag.TagNo, presTag => presTag.TagNumber, (x, y) => new { ApiTag = x, PresTag = y })
                .SelectMany(x => x.PresTag.DefaultIfEmpty(), (x, y) =>
                    new ProcosysTagDto(x.ApiTag.TagNo, x.ApiTag.Description, x.ApiTag.PurchaseOrderNo, x.ApiTag.CommPkgNo, x.ApiTag.McPkgNo, y != null))
                .ToList();

            return new SuccessResult<List<ProcosysTagDto>>(combinedTags);
        }
    }
}
