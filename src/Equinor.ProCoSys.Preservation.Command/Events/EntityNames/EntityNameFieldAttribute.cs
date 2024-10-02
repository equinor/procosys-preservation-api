using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameFieldAttribute : EntityNameAttribute
{
    public EntityNameFieldAttribute() : base("PreservationField")
    {
    }
}
