using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameResponsible]
public class ResponsibleDeleteEvent : DeleteEvent<Responsible>
{
    public ResponsibleDeleteEvent(Guid guid, string plant) : base(guid, plant, null) { }
}
