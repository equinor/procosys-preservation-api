using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class FieldEntityNameAttribute : EntityNameAttribute
{
    public FieldEntityNameAttribute() : base("PreservationField")
    {
    }
}
