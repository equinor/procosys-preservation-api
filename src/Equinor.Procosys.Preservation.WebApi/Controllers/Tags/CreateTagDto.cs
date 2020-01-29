using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagDto
    {
        public IEnumerable<string> TagNos { get; set; }
        public string ProjectName { get; set; }
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
        public string Remark { get; set; }
    }
}
