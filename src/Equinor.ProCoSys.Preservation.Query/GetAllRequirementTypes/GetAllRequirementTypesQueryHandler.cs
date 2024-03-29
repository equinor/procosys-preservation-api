﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes
{
    public class GetAllRequirementTypesQueryHandler : IRequestHandler<GetAllRequirementTypesQuery, Result<IEnumerable<RequirementTypeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllRequirementTypesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<RequirementTypeDto>>> Handle(GetAllRequirementTypesQuery request, CancellationToken cancellationToken)
        {
            var requirementTypes = await (from m in _context.QuerySet<RequirementType>()
                    .Include(rt => rt.RequirementDefinitions).ThenInclude(rd => rd.Fields)
                select m).ToListAsync(cancellationToken);

            var dtos =
                requirementTypes.Where(rt => !rt.IsVoided || request.IncludeVoided).Select(rt
                        => new RequirementTypeDto(rt.Id,
                            rt.Code,
                            rt.Title,
                            rt.Icon,
                            rt.IsVoided,
                            rt.SortKey,
                            rt.RequirementDefinitions.Where(rd => !rd.IsVoided || request.IncludeVoided).Select(rd
                                => new RequirementDefinitionDto(rd.Id,
                                    rd.Title,
                                    rd.IsVoided,
                                    rd.DefaultIntervalWeeks,
                                    rd.Usage,
                                    rd.SortKey,
                                    rd.NeedsUserInput,
                                    rd.OrderedFields(request.IncludeVoided).Select(f
                                        => new FieldDto(
                                            f.Id,
                                            f.Label,
                                            f.IsVoided,
                                            f.FieldType,
                                            f.SortKey,
                                            f.Unit,
                                            f.ShowPrevious,
                                            f.RowVersion.ConvertToString())),
                                    rd.RowVersion.ConvertToString())),
                            rt.RowVersion.ConvertToString()))
                    .OrderBy(rt => rt.SortKey);

            return new SuccessResult<IEnumerable<RequirementTypeDto>>(dtos);
        }
    }
}
