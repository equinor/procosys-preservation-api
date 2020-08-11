using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAllSavedFilters
{
    public class GetAllSavedFiltersQueryHandler : IRequestHandler<GetAllSavedFiltersQuery, Result<List<SavedFilterDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetAllSavedFiltersQueryHandler(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider,
            IProjectRepository projectRepository)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<List<SavedFilterDto>>> Handle(GetAllSavedFiltersQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            var savedFilters = await (from s in _context.QuerySet<SavedFilter>()
                join p in _context.QuerySet<Person>() on EF.Property<int>(s, "PersonId") equals p.Id
                join pr in _context.QuerySet<Project>() on s.ProjectId equals pr.Id
                where pr.Name == request.ProjectName
                      && p.Oid == currentUserOid
                select p.SavedFilters).SingleOrDefaultAsync(cancellationToken);

            var savedFilterDtos = savedFilters
                    .Select(savedFilter => new SavedFilterDto(
                    savedFilter.Title,
                    savedFilter.Criteria,
                    savedFilter.DefaultFilter,
                    savedFilter.CreatedAtUtc,
                    savedFilter.RowVersion.ConvertToString())).ToList();

            return new SuccessResult<List<SavedFilterDto>>(savedFilterDtos);
        }
    }
}
