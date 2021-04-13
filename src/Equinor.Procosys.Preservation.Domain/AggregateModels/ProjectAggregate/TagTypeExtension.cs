namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public static class TagTypeExtension
    {
        public static string GetTagNoPrefix(this TagType tagType)
        {
            switch (tagType)
            {
                case TagType.PreArea:
                    return "#PRE";
                case TagType.SiteArea:
                    return "#SITE";
                case TagType.PoArea:
                    return "#PO";
                default:
                    return string.Empty;
            }
        }
    }
}
