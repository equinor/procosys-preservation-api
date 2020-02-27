using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : SchemaEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        private readonly List<Step> _steps = new List<Step>();

        protected Journey()
            : base(null)
        {
        }

        public Journey(string schema, string title)
            : base(schema) => Title = title;

        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
        public string Title { get; private set; }
        public bool IsVoided { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void AddStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            _steps.Add(step);
        }

        public Step GetNextStep(int stepId)
        {
            var orderedSteps = _steps.OrderBy(r => r.SortKey).Where(r => !r.IsVoided).ToList();
            var stepIndex = orderedSteps.FindIndex(s => s.Id == stepId);
            if (stepIndex < orderedSteps.Count - 1)
            {
                return orderedSteps[stepIndex+1];
            }

            return null;
        }

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} is not UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }
    }
}
