using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, bool isVoided, IEnumerable<StepDto> steps)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Steps = steps;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public IEnumerable<StepDto> Steps { get; }
    }
}
