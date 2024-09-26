using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class RequirementFieldEntityNameAttribute : EntityNameAttribute
{
    public RequirementFieldEntityNameAttribute() : base("PreservationRequirementField")
    {
    }
}
