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
        private readonly IProjectRepository _projectRepository;

        public GetAllSavedFiltersQueryHandler(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider,
            IProjectRepository projectRepository)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
            _projectRepository = projectRepository;
        }

        public async Task<Result<List<SavedFilterDto>>> Handle(GetAllSavedFiltersQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            var person = await
                (from p in _context.QuerySet<Person>()
                        .Include(p => p.SavedFilters)
                    where p.Oid == currentUserOid
                    select p).SingleOrDefaultAsync(cancellationToken);

            var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);

            var savedFilters = person
                .SavedFilters
                .Where(sf => sf.ProjectId == project.Id)
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
