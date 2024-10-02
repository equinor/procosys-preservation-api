using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameJourneyAttribute : EntityNameAttribute
{
    public EntityNameJourneyAttribute() : base("PreservationJourney")
    {
    }
}
