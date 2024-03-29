﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQueryHandler : IRequestHandler<GetAllModesQuery, Result<IEnumerable<ModeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllModesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<ModeDto>>> Handle(GetAllModesQuery request, CancellationToken cancellationToken)
        {
            var modes = await (from m in _context.QuerySet<Mode>()
                where request.IncludeVoided || !m.IsVoided
                select m).ToListAsync(cancellationToken);

            return new SuccessResult<IEnumerable<ModeDto>>(modes.Select(mode
                => new ModeDto(
                    mode.Id,
                    mode.Title,
                    mode.IsVoided,
                    mode.ForSupplier,
                    IsInUseAsync(mode.Id, cancellationToken).Result,
                    mode.RowVersion.ConvertToString())));
        }

        private async Task<bool> IsInUseAsync(int modeId, CancellationToken cancellationToken) 
            => await (from s in _context.QuerySet<Step>()
                where s.ModeId == modeId
                select s).AnyAsync(cancellationToken);
    }
}
