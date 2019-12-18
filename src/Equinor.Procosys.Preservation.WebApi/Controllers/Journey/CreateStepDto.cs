using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journey
{
    public class CreateStepDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int ModeId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ResponsibleId { get; set; }
    }
}
