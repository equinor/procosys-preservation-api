using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDto
    {
        public string TagNo { get; set; }
        public string ProjectName { get; set; }
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
    }
}
