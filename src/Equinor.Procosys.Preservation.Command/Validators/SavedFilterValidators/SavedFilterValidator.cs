using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators
{
    public class SavedFilterValidator : ISavedFilterValidator
    {
        private readonly IReadOnlyContext _context;
        private readonly IPersonRepository _personRepository;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IProjectRepository _projectRepository;

        public SavedFilterValidator(
            IReadOnlyContext context,
            IPersonRepository personRepository,
            ICurrentUserProvider currentUserProvider,
            IProjectRepository projectRepository)
        {
            _context = context;
            _personRepository = personRepository;
            _currentUserProvider = currentUserProvider;
            _projectRepository = projectRepository;
        }

        public async Task<bool> ExistsWithSameTitleForPersonInProjectAsync(string title, string projectName, CancellationToken token)
        {
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            var person = await (from p in _context.QuerySet<Person>()
                    .Include(p => p.SavedFilters)
                where p.Id == currentUser.Id
                select p).SingleOrDefaultAsync(token);

            var project = _projectRepository.GetProjectOnlyByNameAsync(projectName);

            return person.SavedFilters.Any(p => p.Title == title && p.ProjectId == project.Id);
        }
    }
}
