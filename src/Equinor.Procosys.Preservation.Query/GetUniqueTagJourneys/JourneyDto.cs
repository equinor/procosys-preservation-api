namespace Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys
{
    public class JourneyDto
    {
        public JourneyDto(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
