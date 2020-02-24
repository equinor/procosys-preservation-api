using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PreservationContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ITimeService _timeService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public UnitOfWork(PreservationContext context,
            IEventDispatcher eventDispatcher,
            ITimeService timeService,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            _timeService = timeService;
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
            var auditables = _context
                .ChangeTracker
                .Entries<IAuditable>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            if (auditables.Any())
            {
                var now = _timeService.GetCurrentTimeUtc();
                var currentUser = await _currentUserProvider.GetCurrentUserAsync();

                foreach (var entry in auditables)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.SetCreated(now, currentUser.Id);
                            break;
                        case EntityState.Modified:
                            entry.Entity.SetModified(now, currentUser.Id);
                            break;
                    }
                }
            }
        }
    }
}
