using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class ModeEntityNameAttribute : EntityNameAttribute
{
    public ModeEntityNameAttribute() : base("PreservationModes")
    {
    }
}
