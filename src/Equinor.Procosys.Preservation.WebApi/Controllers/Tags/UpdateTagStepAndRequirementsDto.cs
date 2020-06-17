using System.Collections;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDto
    {
        public int StepId { get; set; }
        public IList<TagRequirementDto> NewRequirements { get; set; }
        public IList<UpdatedTagRequirementDto> updatedRequirements { get; set; }
        public string RowVersion { get; set; }
    }
}
