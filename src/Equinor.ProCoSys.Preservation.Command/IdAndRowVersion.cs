namespace Equinor.ProCoSys.Preservation.Command
{
    public class IdAndRowVersion
    {
        public IdAndRowVersion(int id, string rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string RowVersion { get; }
    }
}
