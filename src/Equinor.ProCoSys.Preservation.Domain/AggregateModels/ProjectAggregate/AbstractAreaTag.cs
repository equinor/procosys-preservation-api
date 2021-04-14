namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class AbstractAreaTag
    {
        public abstract TagType TagType { get; }
        public abstract string DisciplineCode { get; }
        public abstract string AreaCode { get; }
        public abstract string PurchaseOrderCalloffCode { get; }
        public abstract string TagNoSuffix { get; }

        public string GetTagNo()
        {
            var tagNo = $"{TagType.GetTagNoPrefix()}-{DisciplineCode}";
            if ((TagType == TagType.PreArea || TagType == TagType.SiteArea) && !string.IsNullOrEmpty(AreaCode))
            {
                tagNo += $"-{AreaCode}";
            }
            else if (TagType == TagType.PoArea && !string.IsNullOrEmpty(PurchaseOrderCalloffCode))
            {
                tagNo += $"-{PurchaseOrderCalloffCode}";
            }

            if (!string.IsNullOrEmpty(TagNoSuffix))
            {
                tagNo += $"-{TagNoSuffix}";
            }

            return tagNo.ToUpperInvariant();
        }
    }
}
