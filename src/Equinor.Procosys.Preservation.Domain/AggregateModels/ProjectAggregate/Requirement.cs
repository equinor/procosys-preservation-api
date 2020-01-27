using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
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
        public DateTime? NextDueTimeUtc { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public IReadOnlyCollection<PreservationRecord> PreservationRecords => _preservationRecords.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void Preserve(PreservationRecord preservationRecord)
        {
            if (preservationRecord == null)
            {
                throw new ArgumentNullException(nameof(preservationRecord));
            }

            _preservationRecords.Add(preservationRecord);
            NextDueTimeUtc = preservationRecord.PreservedAtUtc.AddWeeks(IntervalWeeks);
        }

        public void StartPreservation(DateTime currentTimeUtc)
            => NextDueTimeUtc = currentTimeUtc.AddWeeks(IntervalWeeks);

        public TimeSpan GetTimeUntilNextDueTime(DateTime timeUtc)
        {
            if (!NextDueTimeUtc.HasValue)
            {
                return default;
            }

            return NextDueTimeUtc.Value - timeUtc;
        }
    }
}
