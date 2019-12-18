using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tag
{
    public class SetStepDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int JourneyId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StepId { get; set; }
    }
}
