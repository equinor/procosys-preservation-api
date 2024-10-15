using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameStepAttribute : EntityNameAttribute
{
    public EntityNameStepAttribute() : base("PreservationStep")
    {
    }
}
