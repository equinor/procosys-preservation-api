namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class PairedStepIdWithRowVersionDto
    {
        public StepIdWithRowVersionDto StepDtoA { get; set; }
        public StepIdWithRowVersionDto StepDtoB { get; set; }
    }
}
