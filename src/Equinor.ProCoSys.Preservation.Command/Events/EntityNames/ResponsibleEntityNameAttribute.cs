using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class ResponsibleEntityNameAttribute : EntityNameAttribute
{
    public ResponsibleEntityNameAttribute() : base("PreservationResponsible")
    {
    }
}
