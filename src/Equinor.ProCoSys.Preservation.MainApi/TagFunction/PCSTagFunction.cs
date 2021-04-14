using System.Diagnostics;

namespace Equinor.ProCoSys.Preservation.MainApi.TagFunction
{
    [DebuggerDisplay("{Code}/{RegisterCode}")]
    public class PCSTagFunction
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string RegisterCode { get; set; }
    }
}
