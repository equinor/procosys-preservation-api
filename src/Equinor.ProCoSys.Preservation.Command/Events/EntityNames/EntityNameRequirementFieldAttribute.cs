using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameRequirementFieldAttribute : EntityNameAttribute
{
    public EntityNameRequirementFieldAttribute() : base("PreservationRequirementField")
    {
    }
}
