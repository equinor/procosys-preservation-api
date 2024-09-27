using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameModeAttribute : EntityNameAttribute
{
    public EntityNameModeAttribute() : base("PreservationModes")
    {
    }
}
