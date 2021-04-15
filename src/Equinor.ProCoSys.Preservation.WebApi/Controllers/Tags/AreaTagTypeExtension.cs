using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public static class AreaTagTypeExtension
    {
        public static TagType ConvertToTagType(this AreaTagType areaTagType)
        {
            switch (areaTagType)
            {
                case AreaTagType.PreArea:
                    return TagType.PreArea;
                case AreaTagType.SiteArea:
                    return TagType.SiteArea;
                case AreaTagType.PoArea:
                    return TagType.PoArea;
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaTagType), areaTagType, null);
            }
        }
    }
}
