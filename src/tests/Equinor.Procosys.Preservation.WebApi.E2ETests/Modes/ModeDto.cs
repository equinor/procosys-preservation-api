namespace Equinor.Procosys.Preservation.WebApi.E2ETests.Modes
{
    public class ModeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public bool ForSupplier { get; set; }
        public bool InUse { get; set; }
        public string RowVersion { get; set; }
    }
}
