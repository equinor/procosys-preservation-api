using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using System.Threading.Tasks;
using System;
using System.Linq;
using Equinor.ProCoSys.Common;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations;

public class LocalPersonRepository : ILocalPersonRepository
{
    private readonly IReadOnlyContext _context;

    public LocalPersonRepository(IReadOnlyContext context) => _context = context;

    public async Task<bool> ExistsAsync(Guid userOid)
    {
        var exists = await (from person in _context.QuerySet<Person>()
            where person.Guid == userOid
            select person).AnyAsync();
        return exists;
    }
}
