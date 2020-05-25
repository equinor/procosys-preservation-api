namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class UpdateStepDto
    {
        public int ModeId { get; set; }
        public int ResponsibleId { get; set; }
        public string Title { get; set; }
        public string RowVersion { get; set; }
    }
}
