namespace Equinor.ProCoSys.Preservation.Query.GetTagDetails
{
    public class ModeDetailsDto
    {
        public ModeDetailsDto(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
