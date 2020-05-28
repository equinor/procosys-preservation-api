namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
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
