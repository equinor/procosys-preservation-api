using System;

namespace Equinor.ProCoSys.Preservation.MainApi.Project
{
    public class ProCoSysProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
        public Guid ProjectProCoSysGuid { get; set; }
    }
}
