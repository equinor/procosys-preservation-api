﻿using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.TagFunctions
{
    public class UpdateRequirementsDto
    {
        public string TagFunctionCode { get; set; }
        public string RegisterCode { get; set; }
        public IEnumerable<TagFunctionRequirementDto> Requirements { get; set; }
    }
}
