using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using System.Threading.Tasks;
using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations;

public class LocalPersonRepository : ILocalPersonRepository
{
    private readonly IPersonRepository _personRepository;

    public LocalPersonRepository(IPersonRepository personRepository)
        => _personRepository = personRepository;

    public async Task<bool> ExistsAsync(Guid userOid)
        => await _personRepository.GetByOidAsync(userOid) != null;
}
