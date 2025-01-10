using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetAllJourneys
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title, bool isVoided, IEnumerable<StepDto> steps, JourneyProjectDetailsDto journeyProjectDetails, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Steps = steps;
            RowVersion = rowVersion;
            Project = journeyProjectDetails;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public IEnumerable<StepDto> Steps { get; }
        public string RowVersion { get; }
        public JourneyProjectDetailsDto Project { get; set; }
        public class JourneyProjectDetailsDto
        {
            public JourneyProjectDetailsDto(int id, string name, string description)
            {
                Id = id;
                Name = name;
                Description = description;
            }

            public int Id {get ; set;}
            public string Name {get ; set;}
            public string Description {get ; set;}
        }
    }
}
