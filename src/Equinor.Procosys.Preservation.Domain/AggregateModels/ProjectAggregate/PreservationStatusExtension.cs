namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public static class PreservationStatusExtension
    {
        public static string GetDisplayValue(this PreservationStatus preservationStatus)
        {
            switch (preservationStatus)
            {
                case PreservationStatus.NotStarted:
                    return "Not started";
                case PreservationStatus.Active:
                    return "Active";
                case PreservationStatus.Completed:
                    return "Completed";
                default:
                    return string.Empty;
            }
        }
    }
}
