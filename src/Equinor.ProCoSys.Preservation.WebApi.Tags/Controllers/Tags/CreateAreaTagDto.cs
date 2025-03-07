﻿using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class CreateAreaTagDto : AreaTagDto
    {
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
