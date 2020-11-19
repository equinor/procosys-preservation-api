using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsVoided { get; set; }
        public int SortKey { get; set; }
        public FieldType FieldType { get; set; }
        public string Unit { get; set; }
        public bool? ShowPrevious { get; set; }
        public string RowVersion { get; set; }
    }
}
