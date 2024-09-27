using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameTagAttribute : EntityNameAttribute
{
    public EntityNameTagAttribute() : base("PreservationTag")
    {
    }
}
