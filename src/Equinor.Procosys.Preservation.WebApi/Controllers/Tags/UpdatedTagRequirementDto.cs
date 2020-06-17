namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdatedTagRequirementDto
    {
        public int RequirementDefinitionId { get; set; }
        public int IntervalWeeks { get; set; }

        public bool Voided { get; set; }
    }
}
