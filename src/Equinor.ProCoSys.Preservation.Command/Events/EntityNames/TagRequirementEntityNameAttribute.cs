using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class TagRequirementEntityNameAttribute : EntityNameAttribute
{
    public TagRequirementEntityNameAttribute() : base("PreservationTagRequirement")
    {
    }
}
