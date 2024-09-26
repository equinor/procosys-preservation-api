using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class JourneyEntityNameAttribute : EntityNameAttribute
{
    public JourneyEntityNameAttribute() : base("PreservationJourneys")
    {
    }
}
