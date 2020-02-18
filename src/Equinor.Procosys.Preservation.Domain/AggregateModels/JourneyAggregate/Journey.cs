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
        
        public IOrderedEnumerable<Step> OrderedSteps()
            => Steps
                .Where(r => !r.IsVoided)
                .OrderBy(r => r.SortKey)
                .ThenBy(r => r.Id);

        public Step GetNextStep(int stepId)
        {
            var nextStep = OrderedSteps()
                .SkipWhile(s => s.Id == stepId)
                .Skip(1)
                .FirstOrDefault();
            return nextStep;
        }
    }
}
