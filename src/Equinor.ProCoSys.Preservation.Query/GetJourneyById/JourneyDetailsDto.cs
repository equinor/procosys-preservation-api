using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetJourneyById
{
    public class JourneyDetailsDto(
        int id,
        string title,
        bool isInUse,
        bool isVoided,
        IEnumerable<StepDetailsDto> steps,
        ProjectDetailsDto projectDetailsDto,
        string rowVersion)
    {
        public int Id { get; } = id;
        public string Title { get; } = title;
        public bool IsVoided { get; } = isVoided;
        public bool IsInUse { get; } = isInUse;
        public IEnumerable<StepDetailsDto> Steps { get; } = steps;
        public string RowVersion { get; } = rowVersion;
        public ProjectDetailsDto Project { get; set; } = projectDetailsDto;
    }
}
