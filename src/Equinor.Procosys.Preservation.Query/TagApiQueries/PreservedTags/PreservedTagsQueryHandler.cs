using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.PreservedTags
{
    public class PreservedTagsQueryHandler : IRequestHandler<PreservedTagsQuery, Result<List<ProcosysPreservedTagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ITagApiService _tagApiService;
        private readonly IPlantProvider _plantProvider;

        public PreservedTagsQueryHandler(IReadOnlyContext context, ITagApiService tagApiService, IPlantProvider plantProvider)
        {
            _context = context;
            _tagApiService = tagApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<ProcosysPreservedTagDto>>> Handle(PreservedTagsQuery request, CancellationToken cancellationToken)
        {
            var apiTags = await _tagApiService
                .GetPreservedTagsAsync(_plantProvider.Plant, request.ProjectName)
                ?? new List<ProcosysPreservedTag>();

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
                        new ProcosysPreservedTagDto(
                            x.ApiTag.Id,
                            x.ApiTag.TagNo,
                            x.ApiTag.Description,
                            x.ApiTag.PurchaseOrderTitle,
                            x.ApiTag.CommPkgNo,
                            x.ApiTag.McPkgNo,
                            x.ApiTag.TagFunctionCode,
                            x.ApiTag.RegisterCode,
                            x.ApiTag.MccrResponsibleCodes,
                            x.ApiTag.PreservationRemark,
                            x.ApiTag.StorageArea,
                            x.ApiTag.ModeCode,
                            x.ApiTag.Heating,
                            x.ApiTag.Special,
                            x.ApiTag.NextUpcommingDueTime,
                            x.ApiTag.StartDate,
                            y != null))
                .ToList();

            return new SuccessResult<List<ProcosysPreservedTagDto>>(combinedTags);
        }
    }
}
