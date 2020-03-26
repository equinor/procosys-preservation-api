namespace Equinor.Procosys.Preservation.Query.GetUniqueTagModes
{
    public class ModeDto
    {
        public ModeDto(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
