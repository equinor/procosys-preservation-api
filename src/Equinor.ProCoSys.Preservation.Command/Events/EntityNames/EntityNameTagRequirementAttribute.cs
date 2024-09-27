using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameTagRequirementAttribute : EntityNameAttribute
{
    public EntityNameTagRequirementAttribute() : base("PreservationTagRequirement")
    {
    }
}
