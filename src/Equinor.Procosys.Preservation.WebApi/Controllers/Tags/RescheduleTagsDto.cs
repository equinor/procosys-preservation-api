using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.Events;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RescheduleTagsDto
    {
        public List<TagIdWithRowVersionDto> Tags { get; set; }
        public int Weeks { get; set; }
        public RescheduledDirection Direction { get; set; }
        public string Comment { get; set; }
    }
}
