using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json.Linq;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters
{
    public class TagCreatedEventConverter : IDomainToIntegrationEventConverter<TagCreatedEvent>
    {
        public IEnumerable<IIntegrationEvent> Convert(TagCreatedEvent domainEvent)
        {
            foreach (var tagRequirement in domainEvent.Entity.Requirements)
            {
                yield return new TagRequirementEvent
                {
                    ProCoSysGuid = tagRequirement.Guid,
                    Plant = tagRequirement.Plant,
                    IntervalWeeks = tagRequirement.IntervalWeeks,
                    Usage = tagRequirement.Usage.ToString(),
                    NextDueTimeUtc = tagRequirement.NextDueTimeUtc,
                    IsVoided = tagRequirement.IsVoided,
                    IsInUse = tagRequirement.IsInUse,
                    CreatedAtUtc = tagRequirement.CreatedAtUtc,
                    ModifiedAtUtc = tagRequirement.ModifiedAtUtc,
                    ReadyToBePreserved = tagRequirement.ReadyToBePreserved
                };
            }
        }
    }
}
