using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.TagFunctions
{
    public class TagFunctionDetailsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string RegisterCode { get; set; }
        public bool IsVoided { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }
        public string RowVersion { get; set; }
    }
}
