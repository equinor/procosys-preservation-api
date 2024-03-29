﻿using System;
using System.Diagnostics;

namespace Equinor.ProCoSys.Preservation.MainApi.Tag
{
    [DebuggerDisplay("{TagNo}")]
    public class PCSTagDetails
    {
        public long Id { get; set; }
        public Guid ProCoSysGuid { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string CallOffNo { get; set; }
        public string CommPkgNo { get; set; }
        public Guid? CommPkgProCoSysGuid { get; set; }
        public string Description { get; set; }
        public string DisciplineCode { get; set; }
        public string DisciplineDescription { get; set; }
        public string McPkgNo { get; set; }
        public Guid? McPkgProCoSysGuid { get; set; }
        public string ProjectDescription { get; set; }
        public Guid ProjectProCoSysGuid { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string RegisterCode { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
        public bool IsVoided { get; set; }
    }
}
