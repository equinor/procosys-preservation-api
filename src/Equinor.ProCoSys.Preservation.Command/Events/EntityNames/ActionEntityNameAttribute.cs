using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class ActionEntityNameAttribute : EntityNameAttribute
{
    public ActionEntityNameAttribute() : base("PreservationAction")
    {
    }
}
