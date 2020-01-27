using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Requirement : SchemaEntityBase
    {
        private readonly bool _requirementDefinitionNeedUserInput;
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
            _requirementDefinitionNeedUserInput = requirementDefinition.NeedsUserInput;
        }

        public int IntervalWeeks { get; private set; }
        public DateTime? NextDueTimeUtc { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public IReadOnlyCollection<PreservationPeriod> PreservationPeriods => _preservationPeriods.AsReadOnly();

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public bool ReadyToBePreserved => PreservationPeriods.Count(pp => pp.Status == PreservationPeriodStatus.ReadyToBePreserved) == 1;

        public void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            var preservationPeriod =
                PreservationPeriods.SingleOrDefault(pp => pp.Status == PreservationPeriodStatus.ReadyToBePreserved);

            if (preservationPeriod == null)
            {
                throw new Exception(
                    $"{nameof(Requirement)} do not have a {nameof(PreservationPeriod)} with {PreservationPeriodStatus.ReadyToBePreserved}. Can't preserve");
            }

            preservationPeriod.Preserve(preservedAtUtc, preservedBy, bulkPreserved);
            AddNewPreservationPeriod(preservedAtUtc);
        }

        public void StartPreservation(DateTime preservationStartedUtc)
        {
            if (_preservationPeriods.Any())
            {
                throw new Exception($"{nameof(Requirement)} do have {nameof(PreservationPeriod)}. Can't start");
            }
            AddNewPreservationPeriod(preservationStartedUtc);
        }

        private void AddNewPreservationPeriod(DateTime nextDueTimeUtc)
        {
            NextDueTimeUtc = nextDueTimeUtc.AddWeeks(IntervalWeeks);
            var initialStatus =  _requirementDefinitionNeedUserInput ? PreservationPeriodStatus.NeedUserInput : PreservationPeriodStatus.ReadyToBePreserved;
            var preservationPeriod = new PreservationPeriod(base.Schema, NextDueTimeUtc.Value, initialStatus);
            _preservationPeriods.Add(preservationPeriod);
        }
    }
}
