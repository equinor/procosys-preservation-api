using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class TagEntityNameAttribute : EntityNameAttribute
{
    public TagEntityNameAttribute() : base("PreservationTag")
    {
    }
}
