using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNamePreservationPeriodAttribute : EntityNameAttribute
{
    public EntityNamePreservationPeriodAttribute() : base("PreservationPeriod")
    {
    }
}
