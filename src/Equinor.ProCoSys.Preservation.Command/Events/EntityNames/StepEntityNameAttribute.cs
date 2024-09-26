using MassTransit;

namespace Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

public class StepEntityNameAttribute : EntityNameAttribute
{
    public StepEntityNameAttribute() : base("PreservationSteps")
    {
    }
}
