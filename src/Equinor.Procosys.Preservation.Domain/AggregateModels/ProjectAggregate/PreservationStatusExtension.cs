namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public static class PreservationStatusExtension
    {
        public static string GetDisplayValue(this PreservationStatus preservationStatus)
            => preservationStatus switch
            {
                PreservationStatus.NotStarted => "Not started",
                PreservationStatus.Active => "Active",
                PreservationStatus.Completed => "Completed",
                _ => string.Empty
            };
    }
}
