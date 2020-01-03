namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementDefinitionDto
    {
        public RequirementDefinitionDto(int id, string title, bool isVoided, int defaultInterval, int sortKey)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            DefaultInterval = defaultInterval;
            SortKey = sortKey;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int DefaultInterval { get; }
        public int SortKey { get; }
    }
}
