﻿using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class UpdateTagRequirementsDto
    {
        public string Description { get; set; }
        public IList<TagRequirementDto> NewRequirements { get; set; }
        public IList<UpdatedTagRequirementDto> UpdatedRequirements { get; set; }
        public IList<DeletedTagRequirementDto> DeletedRequirements { get; set; }
        public string RowVersion { get; set; }
    }
}
