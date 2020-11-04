using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagDto
    {
        public int Id { get; set; }
        public string TagNo { get; set; }
        public TagType TagType { get; set; }
        public bool ReadyToBeDuplicated { get; set; }
        public string RowVersion { get; set; }
    }
}
