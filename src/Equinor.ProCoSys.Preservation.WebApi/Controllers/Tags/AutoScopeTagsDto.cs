using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class AutoScopeTagsDto
    {
        public IEnumerable<string> TagNos { get; set; }
        public string ProjectName { get; set; }
        public int StepId { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
