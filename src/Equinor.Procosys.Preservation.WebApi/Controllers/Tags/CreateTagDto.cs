using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDto
    {
        public string TagNo { get; set; }
        public string ProjectNo { get; set; }
        public int JourneyId { get; set; }
        public int StepId { get; set; }
        public IEnumerable<int> RequirementDefinitionIds { get; set; }
    }
}
