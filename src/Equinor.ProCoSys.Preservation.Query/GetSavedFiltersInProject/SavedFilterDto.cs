using System;

namespace Equinor.ProCoSys.Preservation.Query.GetSavedFiltersInProject
{
    public class SavedFilterDto
    {
        public SavedFilterDto(
            int id,
            string title,
            string criteria,
            bool defaultFilter,
            DateTime createdAtUtc,
            string rowVersion)
        {
            Id = id;
            Title = title;
            Criteria = criteria;
            DefaultFilter = defaultFilter;
            CreatedAtUtc = createdAtUtc;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public string Criteria { get; }
        public bool DefaultFilter { get; }
        public DateTime CreatedAtUtc { get; }
        public string RowVersion { get; }
    }
}
