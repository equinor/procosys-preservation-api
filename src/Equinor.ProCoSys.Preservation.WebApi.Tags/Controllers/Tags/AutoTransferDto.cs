﻿using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class AutoTransferDto
    {
        public string ProjectName { get; set; }
        public string CertificateNo { get; set; }
        public string CertificateType { get; set; }
        public Guid ProCoSysGuid { get; set; }
    }
}
