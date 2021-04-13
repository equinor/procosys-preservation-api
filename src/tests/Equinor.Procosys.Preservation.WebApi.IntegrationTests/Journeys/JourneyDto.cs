using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class JourneyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public IEnumerable<StepDto> Steps { get; set; }
        public string RowVersion { get; set; }
    }
}
