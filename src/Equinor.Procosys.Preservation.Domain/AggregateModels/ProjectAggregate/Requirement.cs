using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Requirement : SchemaEntityBase
    {
        private readonly PreservationPeriodStatus _initialPreservationPeriodStatus;
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
            
            _initialPreservationPeriodStatus = requirementDefinition.NeedsUserInput
                ? PreservationPeriodStatus.NeedsUserInput
                : PreservationPeriodStatus.ReadyToBePreserved;
        }

        public int IntervalWeeks { get; private set; }
        public DateTime? NextDueTimeUtc { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public IReadOnlyCollection<PreservationPeriod> PreservationPeriods => _preservationPeriods.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public bool ReadyToBePreserved => PeriodReadyToBePreserved != null;

        public int? GetNextDueInWeeks(DateTime timeUtc)
        {
            if (!NextDueTimeUtc.HasValue)
            {
                return null;
            }

            return timeUtc.GetWeeksUntil(NextDueTimeUtc.Value);
        }
        
        public bool IsReadyToBeBulkPreserved(DateTime currentTimeUtc)
        {
            if (!ReadyToBePreserved)
            {
                return false;
            }

            var nextDueInWeeks = GetNextDueInWeeks(currentTimeUtc);

            return nextDueInWeeks.HasValue && nextDueInWeeks.Value <= 0;
        }

        public bool HasActivePeriod => ActivePeriod != null;

        public void StartPreservation(DateTime startedAtUtc)
        {
            if (_preservationPeriods.Any())
            {
                throw new Exception($"{nameof(Requirement)} {Id} do have {nameof(PreservationPeriod)}. Can't start");
            }
            AddNewPreservationPeriod(startedAtUtc);
        }

        public void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            var preservationPeriod = PeriodReadyToBePreserved;
            if (preservationPeriod == null)
            {
                throw new Exception($"{nameof(Requirement)} {Id} has not period {PreservationPeriodStatus.ReadyToBePreserved}");
            }

            preservationPeriod.Preserve(preservedAtUtc, preservedBy, bulkPreserved);
            AddNewPreservationPeriod(preservedAtUtc);
        }

        public PreservationPeriod ActivePeriod
            => PreservationPeriods.SingleOrDefault(pp => 
                pp.Status == PreservationPeriodStatus.ReadyToBePreserved ||
                pp.Status == PreservationPeriodStatus.NeedsUserInput);

        private PreservationPeriod PeriodReadyToBePreserved
            => PreservationPeriods.SingleOrDefault(pp => pp.Status == PreservationPeriodStatus.ReadyToBePreserved);

        private void AddNewPreservationPeriod(DateTime offsetTimeUtc)
        {
            NextDueTimeUtc = offsetTimeUtc.AddWeeks(IntervalWeeks);
            var preservationPeriod = new PreservationPeriod(base.Schema, NextDueTimeUtc.Value, _initialPreservationPeriodStatus);
            _preservationPeriods.Add(preservationPeriod);
        }
    }
}
