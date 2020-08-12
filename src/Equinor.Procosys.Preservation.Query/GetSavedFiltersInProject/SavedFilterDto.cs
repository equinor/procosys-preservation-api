using System;

namespace Equinor.Procosys.Preservation.Query.GetSavedFiltersInProject
{
    public class SavedFilterDto
    {
        public SavedFilterDto(string title, string criteria, bool defaultFilter, DateTime createdAtUtc, string rowVersion)
        {
            Title = title;
            Criteria = criteria;
            DefaultFilter = defaultFilter;
            CreatedAtUtc = createdAtUtc;
            RowVersion = rowVersion;
        }

        public string Title { get; }
        public string Criteria { get; }
        public bool DefaultFilter { get; }
        public DateTime CreatedAtUtc { get; }
        public string RowVersion { get; }
    }
}
