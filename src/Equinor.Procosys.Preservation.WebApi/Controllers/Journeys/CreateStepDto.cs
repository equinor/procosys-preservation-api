namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class CreateStepDto
    {
        public string Title { get; set; }
        public int ModeId { get; set; }
        public int ResponsibleId { get; set; }
    }
}
