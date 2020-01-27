using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Requirement : SchemaEntityBase
    {
        private readonly List<PreservationPeriod> _preservationPeriods = new List<PreservationPeriod>();

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
        public IReadOnlyCollection<PreservationPeriod> PreservationPeriods => _preservationPeriods.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved, PreservationPeriodStatus initialStatus)
        {
            var preservationPeriod =
                PreservationPeriods.Single(pp => pp.Status == PreservationPeriodStatus.ReadyToPreserve);

            preservationPeriod.Preserve(preservedAtUtc, preservedBy, bulkPreserved);
            AddNewPreservationPeriod(preservedAtUtc, initialStatus);
        }

        public virtual void StartPreservation(DateTime startedAtUtc, PreservationPeriodStatus initialStatus)
            => AddNewPreservationPeriod(startedAtUtc, initialStatus);

        private void AddNewPreservationPeriod(DateTime nextDueTimeUtc, PreservationPeriodStatus initialStatus)
        {
            NextDueTimeUtc = nextDueTimeUtc.AddWeeks(IntervalWeeks);
            var preservationPeriod = new PreservationPeriod(base.Schema, NextDueTimeUtc.Value, initialStatus);
            _preservationPeriods.Add(preservationPeriod);
        }
    }
}
