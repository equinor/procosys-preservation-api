using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, IEnumerable<StepDto> steps)
        {
            Id = id;
            Title = title;
            Steps = steps;
        }

        public int Id { get; }
        public string Title { get; }

        public IEnumerable<StepDto> Steps { get; }
    }
}
