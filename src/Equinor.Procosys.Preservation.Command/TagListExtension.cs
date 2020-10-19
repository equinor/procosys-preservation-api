using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command
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
