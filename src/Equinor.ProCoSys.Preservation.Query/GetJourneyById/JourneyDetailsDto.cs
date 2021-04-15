using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetJourneyById
{
    public class JourneyDetailsDto
    {
        public JourneyDetailsDto(int id, string title, bool isInUse, bool isVoided, IEnumerable<StepDetailsDto> steps, string rowVersion)
        {
            Id = id;
            Title = title;
            IsInUse = isInUse;
            IsVoided = isVoided;
            Steps = steps;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public bool IsInUse { get; }
        public IEnumerable<StepDetailsDto> Steps { get; }
        public string RowVersion { get; }
    }
}
