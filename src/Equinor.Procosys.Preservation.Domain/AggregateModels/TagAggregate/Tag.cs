using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        public string Description { get; set; }
        public bool IsAreaTag { get; private set; }
        public string ProjectNo { get; private set; }
        public string TagNo { get; private set; }
        public int StepId { get; set; }

        protected Tag()
        {
        }

        public Tag(string tagNo, string projectNo, string schema)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            Schema = schema;
        }
    }
}
