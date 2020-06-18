﻿using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UpdateTagStepAndRequirementsDto
    {
        public int StepId { get; set; }
        public IList<TagRequirementDto> NewRequirements { get; set; }
        public IList<UpdatedTagRequirementDto> UpdatedRequirements { get; set; }
        public string RowVersion { get; set; }
    }
}