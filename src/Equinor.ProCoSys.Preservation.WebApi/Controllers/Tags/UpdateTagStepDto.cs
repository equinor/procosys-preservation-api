using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepDto
    {
        public List<TagIdWithRowVersionDto> TagDtos { get; set; }
        public int StepId { get; set; }
    }
}
