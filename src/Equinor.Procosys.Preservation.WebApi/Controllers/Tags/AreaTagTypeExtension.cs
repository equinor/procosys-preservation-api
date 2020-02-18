using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(areaTagType), areaTagType, null);
            }
        }
    }
}
