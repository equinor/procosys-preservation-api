using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;

        public EventDispatcher(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task DispatchAsync(IEnumerable<Entity> entities)
        {
            var domainEvents = entities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            entities
                .ToList()
                .ForEach(entity => entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await _mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
