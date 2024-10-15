using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameResponsibleAttribute : EntityNameAttribute
{
    public EntityNameResponsibleAttribute() : base("PreservationResponsible")
    {
    }
}
