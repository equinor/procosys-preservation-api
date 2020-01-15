using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Requirement : SchemaEntityBase
    {
        private readonly List<PreservationRecord> _preservationRecords = new List<PreservationRecord>();

        protected Requirement()
            : base(null)
        {
        }

        public Requirement(string schema, int intervalWeeks, RequirementDefinition requirementDefinition)
            : base(schema)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            IntervalWeeks = intervalWeeks;
            RequirementDefinitionId = requirementDefinition.Id;
        }
        public int IntervalWeeks { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; set; }
        public IReadOnlyCollection<PreservationRecord> PreservationRecords => _preservationRecords.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void AddPreservationRecord(PreservationRecord preservationRecord)
        {
            if (preservationRecord == null)
            {
                throw new ArgumentNullException(nameof(preservationRecord));
            }

            _preservationRecords.Add(preservationRecord);
        }
    }
}
