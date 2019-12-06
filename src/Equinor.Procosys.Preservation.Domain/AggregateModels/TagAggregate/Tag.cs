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

        public Tag(string schema, string tagNo, string projectNo, Step step)
            : base(schema)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            TagNo = tagNo;
            ProjectNo = projectNo;
            Schema = schema;
            StepId = step.Id;
        }

        public void SetStep(Step step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            StepId = step.Id;
        }
    }
}
