using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Person;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations;

public class LocalPersonRepository : ILocalPersonRepository
{
    private readonly IReadOnlyContext _context;

    public LocalPersonRepository(IReadOnlyContext context) => _context = context;

    public async Task<bool> ExistsAsync(Guid userOid, CancellationToken cancellationToken)
    {
        var exists = await (from person in _context.QuerySet<Person>()
                            where person.Guid == userOid
                            select person).AnyAsync(cancellationToken);
        return exists;
    }

    public async Task<ProCoSysPerson> GetAsync(Guid userOid, CancellationToken cancellationToken)
        => await (from person in _context.QuerySet<Person>()
                  where person.Guid == userOid
                  select new ProCoSysPerson
                  {
                      AzureOid = person.Guid.ToString(),
                      FirstName = person.FirstName,
                      LastName = person.LastName,
                  }).SingleOrDefaultAsync(cancellationToken);
}
