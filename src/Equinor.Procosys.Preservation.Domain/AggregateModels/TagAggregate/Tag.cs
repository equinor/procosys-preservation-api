using System;
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
            : base(null)
        {
        }

        public Tag(string schema, string tagNo, string projectNo)
            : base(schema)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            Schema = schema;
        }

        public void SetStep(Step step)
        {
            if (step == null)
                throw new ArgumentNullException($"{nameof(step)} cannot be null");
            StepId = step.Id;
        }
    }
}
