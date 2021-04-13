using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateActionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
        public string RowVersion { get; set; }
    }
}
