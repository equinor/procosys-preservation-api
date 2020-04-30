using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, bool isVoided, IEnumerable<StepDto> steps, ulong rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Steps = steps;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public IEnumerable<StepDto> Steps { get; }
        public ulong RowVersion { get; }
    }
}
