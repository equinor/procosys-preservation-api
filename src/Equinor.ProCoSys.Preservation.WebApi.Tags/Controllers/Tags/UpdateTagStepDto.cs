using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class UpdateTagStepDto
    {
        public List<TagIdWithRowVersionDto> TagDtos { get; set; }
        public int StepId { get; set; }
    }
}
