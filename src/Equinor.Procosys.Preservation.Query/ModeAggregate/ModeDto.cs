namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class ModeDto
    {
        public ModeDto(int id, string title, ulong rowVersion)
        {
            Id = id;
            Title = title;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public ulong RowVersion { get; }
    }
}
