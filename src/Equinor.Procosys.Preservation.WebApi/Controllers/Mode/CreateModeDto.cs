using System.ComponentModel.DataAnnotations;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Mode
{
    public class CreateModeDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
