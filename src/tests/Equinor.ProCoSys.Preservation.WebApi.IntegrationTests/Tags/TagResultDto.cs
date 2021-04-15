using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagResultDto
    {
        public int MaxAvailable { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
