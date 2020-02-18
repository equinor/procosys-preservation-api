using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : SchemaEntityBase, IAggregateRoot
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
    }
}
