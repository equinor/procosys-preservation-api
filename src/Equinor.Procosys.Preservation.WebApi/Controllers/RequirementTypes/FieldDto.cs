using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class FieldDto
    {
        public int SortKey { get; set; }
        public FieldType FieldType { get; set; }
        public string Label { get; set; }
        public string Unit { get; set; }
        public bool ShowPrevious { get; set; }
    }
}
