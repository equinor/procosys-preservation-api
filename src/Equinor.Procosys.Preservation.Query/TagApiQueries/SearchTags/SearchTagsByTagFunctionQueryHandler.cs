using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQueryHandler : IRequestHandler<SearchTagsByTagFunctionQuery, Result<List<ProcosysTagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;

        public SearchTagsByTagFunctionQueryHandler(IReadOnlyContext context, ITagApiService tagApiService, IPlantProvider plantProvider)
        {
            _context = context;
            _tagApiService = tagApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<ProcosysTagDto>>> Handle(SearchTagsByTagFunctionQuery request, CancellationToken cancellationToken)
        {
            var tagFunctionCodeRegisterCodePairs = await (from tagFunction in _context.QuerySet<TagFunction>().Include(tf => tf.Requirements)
                    where
                        !tagFunction.IsVoided && tagFunction.Requirements.Any()
                    select $"{tagFunction.Code}|{tagFunction.RegisterCode}")
                .ToListAsync(cancellationToken);
            
            if (!tagFunctionCodeRegisterCodePairs.Any())
            {
                return new NotFoundResult<List<ProcosysTagDto>>("No TagFunctions with preservation requirements found");
            }

            var apiTags = await _tagApiService
                .SearchTagsByTagFunctionsAsync(_plantProvider.Plant, request.ProjectName, tagFunctionCodeRegisterCodePairs)
                ?? new List<ProcosysTagOverview>();

            var presTagNos = await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where p.Name == request.ProjectName
                select tag.TagNo).ToListAsync(cancellationToken);

            // Join all tags from API with preservation tags on TagNo. If a tag is not in preservation scope, use default value (null).
            var combinedTags = apiTags
                .GroupJoin(presTagNos,
                    apiTag => apiTag.TagNo,
                    presTagNo => presTagNo,
                    (x, y) =>
                        new {ApiTag = x, PresTagNo = y})
                .SelectMany(x => x.PresTagNo.DefaultIfEmpty(),
                    (x, y) =>
                        new ProcosysTagDto(
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

            return new SuccessResult<List<ProcosysTagDto>>(combinedTags);
        }

    }
}
