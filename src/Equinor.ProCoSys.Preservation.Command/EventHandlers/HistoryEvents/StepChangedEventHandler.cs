using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class StepChangedEventHandler : INotificationHandler<StepChangedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IJourneyRepository _journeyRepository;

        public StepChangedEventHandler(IHistoryRepository historyRepository, IJourneyRepository journeyRepository)
        {
            _historyRepository = historyRepository;
            _journeyRepository = journeyRepository;
        }

        public Task Handle(StepChangedEvent notification, CancellationToken cancellationToken)
        {
            var journeys = 
                _journeyRepository.GetJourneysByStepIdsAsync(new List<int>
                {
                    notification.FromStepId, notification.ToStepId
                }).Result;

            var fromJourney = journeys.First(j => j.Steps.Any(s => s.Id == notification.FromStepId));
            var toJourney = journeys.First(j => j.Steps.Any(s => s.Id == notification.ToStepId));
            var fromStep = fromJourney.Steps.Single(s => s.Id == notification.FromStepId);
            var toStep = toJourney.Steps.Single(s => s.Id == notification.ToStepId);
            
            EventType eventType;
            string description;
            if (fromJourney.Id == toJourney.Id)
            {
                eventType = EventType.StepChanged;
                description = $"{eventType.GetDescription()} - From '{fromStep.Title}' to '{toStep.Title}' in journey '{fromJourney.Title}'";
            }
            else
            {
                eventType = EventType.JourneyChanged;
                description = $"{eventType.GetDescription()} - From journey '{fromJourney.Title}' / step '{fromStep.Title}' to journey '{toJourney.Title}' / step '{toStep.Title}'";
            }
            
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
