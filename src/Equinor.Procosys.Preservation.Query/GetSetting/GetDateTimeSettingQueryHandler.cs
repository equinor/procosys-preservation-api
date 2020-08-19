using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.SettingAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetSetting
{
    public class GetDateTimeSettingQueryHandler : IRequestHandler<GetDateTimeSettingQuery, Result<DateTime?>>
    {
        private readonly IReadOnlyContext _context;

        // unit test
        public GetDateTimeSettingQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<DateTime?>> Handle(GetDateTimeSettingQuery request, CancellationToken cancellationToken)
        {
            var setting = await (from s in _context.QuerySet<Setting>()
                where s.Code == request.SettingCode
                select s).SingleOrDefaultAsync(cancellationToken);

            if (setting == null)
            {
                return new NotFoundResult<DateTime?>($"Setting with code {request.SettingCode} not found");
            }
            
            return new SuccessResult<DateTime?>(setting.DateTimeUtc);

        }
    }
}
