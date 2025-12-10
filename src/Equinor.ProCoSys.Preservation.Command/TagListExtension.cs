using System.Collections.Generic;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command
{
    public static class TagListExtension
    {
        public static List<IdAndRowVersion> CreateIdAndRowVersionList(this List<Tag> tags)
        {
            var tagsWithUpdatedRowVersion = new List<IdAndRowVersion>();
            foreach (var tag in tags)
            {
                tagsWithUpdatedRowVersion.Add(new IdAndRowVersion(tag.Id, tag.RowVersion.ConvertToString()));
            }

            return tagsWithUpdatedRowVersion;
        }
    }
}
