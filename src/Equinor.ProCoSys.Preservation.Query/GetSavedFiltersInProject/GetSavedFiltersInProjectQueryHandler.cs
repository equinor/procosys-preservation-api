using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetSavedFiltersInProject
{
    public class GetSavedFiltersInProjectQueryHandler : IRequestHandler<GetSavedFiltersInProjectQuery, Result<List<SavedFilterDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetSavedFiltersInProjectQueryHandler(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<List<SavedFilterDto>>> Handle(GetSavedFiltersInProjectQuery request,
            CancellationToken cancellationToken)
        {
            var project = await (from p in _context.QuerySet<Project>()
                where p.Name == request.ProjectName
                select p).SingleOrDefaultAsync(cancellationToken);

            if (project == null)
            {
                return new SuccessResult<List<SavedFilterDto>>(new List<SavedFilterDto>());
            }
            
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var person = await (from p in _context.QuerySet<Person>().Include(p => p.SavedFilters)
                where p.Oid == currentUserOid
                select p).SingleAsync(cancellationToken);

            var savedFilterDtos = person.SavedFilters.Where(sf => sf.ProjectId == project.Id)
                    .Select(savedFilter => new SavedFilterDto(
                    savedFilter.Id,
                    savedFilter.Title,
                    savedFilter.Criteria,
                    savedFilter.DefaultFilter,
                    savedFilter.CreatedAtUtc,
                    savedFilter.RowVersion.ConvertToString())).ToList();

            return new SuccessResult<List<SavedFilterDto>>(savedFilterDtos);
        }
    }

}
