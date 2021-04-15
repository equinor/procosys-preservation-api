using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class CreateActionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
    }
}
