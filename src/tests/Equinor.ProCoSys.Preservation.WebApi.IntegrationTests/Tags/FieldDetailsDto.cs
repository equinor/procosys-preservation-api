using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class FieldDetailsDto
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public FieldType FieldType { get; set; }
        public string Unit { get; set; }
        public bool ShowPrevious { get; set; }
        public object CurrentValue { get; set; }
        public object PreviousValue { get; set; }
    }
}
