namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class PairedStepIdWithRowVersionDto
    {
        public StepIdWithRowVersionDto StepA { get; set; }
        public StepIdWithRowVersionDto StepB { get; set; }
    }
}
