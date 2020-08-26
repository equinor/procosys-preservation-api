namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class JourneyDetailsDto
    {
        public JourneyDetailsDto(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
