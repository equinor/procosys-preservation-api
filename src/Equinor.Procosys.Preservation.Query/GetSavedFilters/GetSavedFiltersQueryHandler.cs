using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetSavedFilters
{
    public class GetSavedFiltersQueryHandler : IRequestHandler<GetSavedFiltersQuery, Result<List<SavedFilterDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetSavedFiltersQueryHandler(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<List<SavedFilterDto>>> Handle(GetSavedFiltersQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            var person = await
                (from p in _context.QuerySet<Person>()
                        .Include(p => p.SavedFilters)
                    where p.Oid == currentUserOid
                    select p).SingleOrDefaultAsync(cancellationToken);

            var savedFilters = person
                .SavedFilters
                .Select(savedFilter => new SavedFilterDto(
                    savedFilter.Title,
                    savedFilter.Criteria,
                    savedFilter.DefaultFilter,
                    savedFilter.CreatedAtUtc,
                    savedFilter.RowVersion.ConvertToString())).ToList();
            return new SuccessResult<List<SavedFilterDto>>(savedFilters);
        }
    }
}
