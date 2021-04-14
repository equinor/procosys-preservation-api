using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class CreateTagsDto
    {
        public IEnumerable<string> TagNos { get; set; }
        public string ProjectName { get; set; }
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
