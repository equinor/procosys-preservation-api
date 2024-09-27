using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameRequirementTypeAttribute : EntityNameAttribute
{
    public EntityNameRequirementTypeAttribute() : base("PreservationRequirementType")
    {
    }
}
