using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PreservationContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICurrentUserProvider _currentUserProvider;

        public UnitOfWork(PreservationContext context,
            IEventDispatcher eventDispatcher,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            await SetAuditData();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchEvents(CancellationToken cancellationToken = default)
        {
            var entities = _context
                .ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);
            await _eventDispatcher.DispatchAsync(entities, cancellationToken);
        }

        private async Task SetAuditData()
        {
            var addedEntries = _context
                .ChangeTracker
                .Entries<ICreationAuditable>()
                .Where(x => x.State == EntityState.Added)
                .ToList();
            var modifiedEntries = _context
                .ChangeTracker
                .Entries<IModificationAuditable>()
                .Where(x => x.State == EntityState.Modified)
                .ToList();

            if (addedEntries.Any() || modifiedEntries.Any())
            {
                var currentUserOid = _currentUserProvider.GetCurrentUser();
                var currentUser = await _context.Persons.SingleOrDefaultAsync(p => p.Oid == currentUserOid);

                foreach (var entry in addedEntries)
                {
                    entry.Entity.SetCreated(currentUser);
                }

                foreach (var entry in modifiedEntries)
                {
                    entry.Entity.SetModified(currentUser);
                }
            }
        }
    }
}
