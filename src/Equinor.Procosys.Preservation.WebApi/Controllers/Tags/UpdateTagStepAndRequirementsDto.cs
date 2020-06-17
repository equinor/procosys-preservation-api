using System.Collections;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDto
    {
        public int StepId { get; set; }
        public IList<TagRequirementDto> NewRequirments { get; set; }
        public IList<UpdatedTagRequirementDto> updatedRequirements { get; set; }
        public string RowVersion { get; set; }
    }
}
