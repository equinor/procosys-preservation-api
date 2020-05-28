using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetJourneys
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, bool isVoided, IEnumerable<StepDto> steps, string rowVersion)
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
        public string RowVersion { get; }
    }
}
