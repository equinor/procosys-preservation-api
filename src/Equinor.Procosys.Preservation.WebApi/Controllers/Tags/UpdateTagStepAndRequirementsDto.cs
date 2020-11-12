using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDto
    {
        public string Description { get; set; }
        public int StepId { get; set; }
        public IList<TagRequirementDto> NewRequirements { get; set; } = new List<TagRequirementDto>();
        public IList<UpdatedTagRequirementDto> UpdatedRequirements { get; set; } = new List<UpdatedTagRequirementDto>();
        public string RowVersion { get; set; }
    }
}
