namespace Equinor.ProCoSys.Preservation.Query.GetPreservationRecord
{
    public class RequirementDefinitionDetailDto
    {
        public RequirementDefinitionDetailDto(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
