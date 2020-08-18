namespace Equinor.Procosys.Preservation.Query.GetTagsQueries
{
    public static class DueFilterTypeExtension
    {
        public static string GetDisplayValue(this DueFilterType dueFilterType)
            => dueFilterType switch
            {
                DueFilterType.Overdue => "Overdue",
                DueFilterType.ThisWeek => "This week",
                DueFilterType.NextWeek => "Next week",
                _ => string.Empty
            };
    }
}
