using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        private readonly List<Step> _steps = new List<Step>();

        protected Journey()
            : base(null)
        {
        }

        public Journey(string schema, string title)
            : base(schema)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            Title = title;
        }

        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
        public string Title { get; set; }
        public bool IsVoided { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
        
        public override string ToString() => Title;

        public void AddStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }
            
            if (step.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {step.Plant} to item in {Plant}");
            }

            _steps.Add(step);
            step.SortKey = _steps.Count;
        }

        public void SwapSteps(int stepId1, int stepId2)
        {
            if (!AreAdjacentSteps(stepId1, stepId2))
            {
                throw new Exception($"{nameof(Step)} {stepId1} and {stepId1} in {nameof(Journey)} {Title} are not adjacent and can't be swapped");
            }

            var step1 = _steps.Single(s => s.Id == stepId1);
            var step2 = _steps.Single(s => s.Id == stepId2);
            var tmp = step1.SortKey;
            step1.SortKey = step2.SortKey;
            step2.SortKey = tmp;
        }

        public bool AreAdjacentSteps(int stepId1, int stepId2)
        {
            var orderedSteps = OrderedSteps().ToList();

            var stepIndex1 = orderedSteps.FindIndex(s => s.Id == stepId1);
            if (stepIndex1 == -1)
            {
                throw new Exception($"{nameof(Step)} {stepId1} not found in {nameof(Journey)} {Title}");
            }

            var stepIndex2 = orderedSteps.FindIndex(s => s.Id == stepId2);
            if (stepIndex2 == -1)
            {
                throw new Exception($"{nameof(Step)} {stepId2} not found in {nameof(Journey)} {Title}");
            }

            return Math.Abs(stepIndex1 - stepIndex2) == 1;
        }

        public Step GetNextStep(int stepId)
        {
            var orderedSteps = OrderedSteps().Where(r => !r.IsVoided).ToList();
            var stepIndex = orderedSteps.FindIndex(s => s.Id == stepId);
            if (stepIndex < orderedSteps.Count - 1)
            {
                return orderedSteps[stepIndex+1];
            }

            return null;
        }

        public IOrderedEnumerable<Step> OrderedSteps() => _steps.OrderBy(r => r.SortKey);

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }
    }
}
