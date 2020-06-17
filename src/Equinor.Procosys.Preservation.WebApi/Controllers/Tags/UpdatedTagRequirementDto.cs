namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdatedTagRequirementDto
    {
        public int RequirementId { get; set; }
        public int IntervalWeeks { get; set; }
        public bool Voided { get; set; }
        public string RowVersion { get; set; }
    }
}
