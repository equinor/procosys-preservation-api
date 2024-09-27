using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameActionAttribute : EntityNameAttribute
{
    public EntityNameActionAttribute() : base("PreservationAction")
    {
    }
}
