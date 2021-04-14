namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class ModeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public bool ForSupplier { get; set; }
        public bool IsInUse { get; set; }
        public string RowVersion { get; set; }
    }
}
