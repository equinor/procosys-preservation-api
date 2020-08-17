namespace Equinor.Procosys.Preservation.Query.GetTagsQueries
{
    public static class ActionStatusExtension
    {
        public static string GetDisplayValue(this ActionStatus actionStatus)
            => actionStatus switch
            {
                ActionStatus.HasOpen => "Has open action(s)",
                ActionStatus.HasClosed => "Has closed action(s)",
                ActionStatus.HasOverdue => "Has overdue action(s)",
                _ => string.Empty
            };

        public static string GetDisplayValue(this ActionStatus? actionStatus)
        {
            if (!actionStatus.HasValue)
            {
                return string.Empty;
            }

            return actionStatus.Value.GetDisplayValue();
        }
    }
}
