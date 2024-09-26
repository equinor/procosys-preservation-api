using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class RequirementTypeEntityNameAttribute : EntityNameAttribute
{
    public RequirementTypeEntityNameAttribute() : base("PreservationRequirementType")
    {
    }
}
