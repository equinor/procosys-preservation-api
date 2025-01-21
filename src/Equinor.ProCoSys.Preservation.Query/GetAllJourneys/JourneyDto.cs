using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetAllJourneys
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, bool isVoided, IEnumerable<StepDto> steps,
            ProjectDetailsDto projectDetails, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Steps = steps;
            RowVersion = rowVersion;
            ProjectDetails = projectDetails;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public IEnumerable<StepDto> Steps { get; }
        public string RowVersion { get; }
        public ProjectDetailsDto ProjectDetails { get; set; }
    }
}
