using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class CreateStepDto
    {
        public int ModeId { get; set; }
        public int ResponsibleId { get; set; }
    }
}
