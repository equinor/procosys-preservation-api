namespace Equinor.Procosys.Preservation.Query.GetModes
{
    public class ModeDto
    {
        public ModeDto(int id, string title, string rowVersion)
        {
            Id = id;
            Title = title;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
