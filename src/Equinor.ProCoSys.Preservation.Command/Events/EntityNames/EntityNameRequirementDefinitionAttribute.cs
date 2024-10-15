using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class EntityNameRequirementDefinitionAttribute : EntityNameAttribute
{
    public EntityNameRequirementDefinitionAttribute() : base("PreservationRequirementDefinition")
    {
    }
}
