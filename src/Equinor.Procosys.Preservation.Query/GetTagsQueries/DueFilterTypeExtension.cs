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
                DueFilterType.WeekPlusTwo => "Week +2",
                DueFilterType.WeekPlusThree => "Week +3",
                _ => string.Empty
            };
    }
}
