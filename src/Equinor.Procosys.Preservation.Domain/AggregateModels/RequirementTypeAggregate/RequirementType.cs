namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : SchemaEntityBase, IAggregateRoot
    {
        public const int CodeMax = 32;
        public const int TitleMax = 64;

        protected RequirementType()
            : base(null)
        {
        }

        public RequirementType(string schema, string code, string title, bool isVoided, int sortKey)
            : base(schema)
        {
            Code = code;
            Title = title;
            IsVoided = isVoided;
            SortKey = sortKey;
        }

        public string Code { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
    }
}
