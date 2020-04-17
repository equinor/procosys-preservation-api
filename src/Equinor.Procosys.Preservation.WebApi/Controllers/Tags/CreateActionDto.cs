using System;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateActionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
    }
}
