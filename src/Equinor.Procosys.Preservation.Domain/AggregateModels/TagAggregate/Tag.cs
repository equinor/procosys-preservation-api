using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        private readonly List<Requirement> _requirements = new List<Requirement>();

        public const int DescriptionLengthMax = 1000;
        public const int TagNoLengthMax = 255;
        public const int ProjectNoLengthMax = 255;

        public string Description { get; set; }
        public bool IsAreaTag { get; set; }
        public string ProjectNo { get; private set; }
        public string TagNo { get; private set; }
        public int StepId { get; set; }
        public IReadOnlyCollection<Requirement> Requirements => _requirements.AsReadOnly();

        protected Tag()
            : base(null)
        {
        }

        public Tag(string schema, string tagNo, string projectNo, Step step, IEnumerable<Requirement> requirements)
            : base(schema)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            var reqList = requirements.ToList();
            if (reqList.Count < 1)
            {
                throw new Exception("Must have at least one requirement");
            }

            TagNo = tagNo;
            ProjectNo = projectNo;
            StepId = step.Id;
            _requirements.AddRange(reqList);
        }

        public void SetStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            StepId = step.Id;
        }

        public void AddRequirement(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            _requirements.Add(requirement);
        }
    }
}
