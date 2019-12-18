using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tag
{
    public class CreateTagDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string TagNo { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string ProjectNo { get; set; }

        [Required]
        public int JourneyId { get; set; }

        [Required]
        public int StepId { get; set; }
    }
}
