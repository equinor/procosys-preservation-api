namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class AbstractAreaTag
    {
        public abstract TagType TagType { get; }
        public abstract string DisciplineCode { get; }
        public abstract string AreaCode { get; }
        public abstract string TagNoSuffix { get; }

        public string GetTagNo()
        {
            var tagNo = $"{TagType.GetTagNoPrefix()}-{DisciplineCode}";
            if (!string.IsNullOrEmpty(AreaCode))
            {
                tagNo += $"-{AreaCode}";
            }

            if (!string.IsNullOrEmpty(TagNoSuffix))
            {
                tagNo += $"-{TagNoSuffix}";
            }

            return tagNo.ToUpperInvariant();
        }
    }
}
