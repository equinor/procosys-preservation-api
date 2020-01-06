using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : SchemaEntityBase, IAggregateRoot
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        private readonly List<Step> _steps = new List<Step>();

        private Journey()
            : base(null)
        {
        }

        public Journey(string schema, string title)
            : base(schema) => Title = title;

        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
        public string Title { get; private set; }

        public void AddStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            _steps.Add(step);
        }
    }
}
