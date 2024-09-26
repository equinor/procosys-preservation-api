using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class PreservationPeriodEntityNameAttribute : EntityNameAttribute
{
    public PreservationPeriodEntityNameAttribute() : base("PreservationPeriod")
    {
    }
}
