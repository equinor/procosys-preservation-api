namespace Equinor.Procosys.Preservation.MainApi.Permission
{
    public class ProcosysProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasAccess { get; set; }
        public bool IsClosed { get; set; }
    }
}
