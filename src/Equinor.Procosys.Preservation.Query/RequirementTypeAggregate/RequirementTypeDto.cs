namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementTypeDto
    {
        public RequirementTypeDto(int id, string code, string title, bool isVoided, int sortKey)
        {
            Id = id;
            Code = code;
            Title = title;
            IsVoided = isVoided;
            SortKey = sortKey;
        }

        public int Id { get; }
        public string Code { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
    }
}
