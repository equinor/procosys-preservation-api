namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : SchemaEntityBase, IAggregateRoot
    {
        public const int CodeLengthMax = 32;
        public const int TitleLengthMax = 64;

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

        public string Code { get; private set; }
        public string Title { get; private set; }
        public bool IsVoided { get; private set; }
        public int SortKey { get; private set; }
    }
}
