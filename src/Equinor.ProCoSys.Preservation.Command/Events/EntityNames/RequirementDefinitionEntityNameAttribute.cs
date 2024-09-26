using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class RequirementDefinitionEntityNameAttribute : EntityNameAttribute
{
    public RequirementDefinitionEntityNameAttribute() : base("PreservationRequirementDefinition")
    {
    }
}
