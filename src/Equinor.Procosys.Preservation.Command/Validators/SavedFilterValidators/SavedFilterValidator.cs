using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators
{
    public class SavedFilterValidator : ISavedFilterValidator
    {
        private readonly IReadOnlyContext _context;
        private readonly IPersonRepository _personRepository;
        private readonly ICurrentUserProvider _currentUserProvider;

        public SavedFilterValidator(
            IReadOnlyContext context,
            IPersonRepository personRepository,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _personRepository = personRepository;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<bool> ExistsWithSameTitleForPersonAsync(string title, CancellationToken token)
        {
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            var person = await (from p in _context.QuerySet<Person>()
                    .Include(p => p.SavedFilters)
                where p.Id == currentUser.Id
                select p).SingleOrDefaultAsync(token); 
            return person.SavedFilters.Any(p => p.Title == title);
        }
    }
}
