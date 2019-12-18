using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journey
{
    public class CreateJourneyDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
