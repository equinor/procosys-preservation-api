using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Requirement : SchemaEntityBase
    {
        private readonly bool _requirementDefinitionNeedsUserInput;
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
            _requirementDefinitionNeedsUserInput = requirementDefinition.NeedsUserInput;
        }

        public int IntervalWeeks { get; private set; }
        public DateTime? NextDueTimeUtc { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public IReadOnlyCollection<PreservationPeriod> PreservationPeriods => _preservationPeriods.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public bool ReadyToBePreserved => ReadyToBePreservationPeriod != null;

        public void StartPreservation(DateTime startedAtUtc)
        {
            if (_preservationPeriods.Any())
            {
                throw new Exception($"{nameof(Requirement)} do have {nameof(PreservationPeriod)}. Can't start");
            }
            AddNewPreservationPeriod(startedAtUtc);
        }

        public void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            var preservationPeriod = ReadyToBePreservationPeriod;
            if (preservationPeriod == null)
            {
                throw new Exception($"{nameof(Requirement)} is not {PreservationPeriodStatus.ReadyToBePreserved}");
            }

            preservationPeriod.Preserve(preservedAtUtc, preservedBy, bulkPreserved);
            AddNewPreservationPeriod(preservedAtUtc);
        }

        private void AddNewPreservationPeriod(DateTime nextDueTimeUtc)
        {
            NextDueTimeUtc = nextDueTimeUtc.AddWeeks(IntervalWeeks);
            var initialStatus =  _requirementDefinitionNeedsUserInput ? PreservationPeriodStatus.NeedsUserInput : PreservationPeriodStatus.ReadyToBePreserved;
            var preservationPeriod = new PreservationPeriod(base.Schema, NextDueTimeUtc.Value, initialStatus);
            _preservationPeriods.Add(preservationPeriod);
        }

        private PreservationPeriod ReadyToBePreservationPeriod
            => PreservationPeriods.SingleOrDefault(pp => pp.Status == PreservationPeriodStatus.ReadyToBePreserved);
    }
}
