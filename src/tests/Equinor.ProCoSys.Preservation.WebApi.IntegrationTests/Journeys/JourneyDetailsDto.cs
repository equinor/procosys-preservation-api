using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class JourneyDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public bool IsInUse { get; set; }
        public IEnumerable<StepDetailsDto> Steps { get; set; }
        public string RowVersion { get; set; }
    }
}
